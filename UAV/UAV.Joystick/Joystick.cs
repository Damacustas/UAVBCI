using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace UAV.Joystick
{
    public class JoystickDevice
    {
        public Dictionary<byte, bool> ButtonStates { get; private set; }

        public Dictionary<byte, float> AxisValues { get; private set; }

        public event JoystickInputDelegate InputReceived;

        private bool running = false;
        private string deviceFile = null;
        private Thread joystickInputThread = null;

        private ConcurrentQueue<JoystickEventArgs> changes = new ConcurrentQueue<JoystickEventArgs>();

        public JoystickDevice()
        {
            ButtonStates = new Dictionary<byte, bool>();
            AxisValues = new Dictionary<byte, float>();
        }

        /// <summary>
        /// Intializes the joystick instance to respond to the given device file.
        /// </summary>
        /// <param name="deviceFile">The file that represents the joystick.</param>
        public void Initialize(string deviceFile)
        {
            this.deviceFile = deviceFile;
            running = true;
            joystickInputThread = new Thread(JoystickInputThread);
            joystickInputThread.Start();
        }

        /// <summary>
        /// Closes the joystick device file and stops the internal input thread.
        /// </summary>
        /// <param name="clearPendingEvents">If <c>true</c>, clears the pending events not yet processed.</param>
        /// <param name="clearState">If <c>true</c>, clears the joystick state.</param>
        public void Deinitialize(bool clearPendingEvents = true, bool clearState = true)
        {
            deviceFile = null;
            running = false;
            joystickInputThread.Join();
            
            if (clearPendingEvents)
            {
                changes = new ConcurrentQueue<JoystickEventArgs>();
            }
            
            if (clearState)
            {
                AxisValues.Clear();
                ButtonStates.Clear();
            }
        }

        /// <summary>
        /// Processes events received from the joystick I/O thread.
        /// </summary>
        public void ProcessEvents()
        {
            JoystickEventArgs e = null;
            while (!changes.IsEmpty)
            {
                if (changes.TryDequeue(out e))
                {
                    UpdateState(e);

                    // If there are subscriptions to the event, raise the event.
                    if (InputReceived != null)
                    {
                        InputReceived(this, e);
                    }
                }
            }
        }

        /// <summary>
        /// Updates the Joystick state by applying the given event.
        /// </summary>
        /// <param name="e">The event to apply to the state.</param>
        private void UpdateState(JoystickEventArgs e)
        {
            if (e.IsButtonEvent)
            {
                if (ButtonStates.ContainsKey(e.Button))
                {
                    ButtonStates[e.Button] = e.IsPressed;
                }
                else
                {
                    ButtonStates.Add(e.Button, e.IsPressed);
                }
            }
            else
            {
                if (AxisValues.ContainsKey(e.Axis))
                {
                    AxisValues[e.Axis] = e.Value;
                }
                else
                {
                    AxisValues.Add(e.Axis, e.Value);
                }
            }
        }

        /// <summary>
        /// Reads from the joystick input stream and enques events to the internal event queue.
        /// </summary>
        private void JoystickInputThread()
        {
            using (FileStream inputStream = new FileStream(deviceFile, FileMode.Open))
            {
                // Read and process in blocks of 8 bytes.
                byte[] buff = new byte[8];
                while (running)
                {
                    inputStream.Read(buff, 0, 8);
                    JoystickEventArgs joystickEvent = DecodeJoystickEvent(buff);
                    if (joystickEvent != null)
                    {
                        changes.Enqueue(joystickEvent);
                    }
                }
            }
        }

        /// <summary>
        /// Decodes input to an event.
        /// </summary>
        /// <param name="buff">The buffer to decode.</param>
        /// <returns>The decoded event.</returns>
        private JoystickEventArgs DecodeJoystickEvent(byte[] buff)
        {
            // Configuration is there to indicate what inputs there are on the joystick.
            if (checkBit(buff[6], (byte)MODE.CONFIGURATION))
            {
                if (checkBit(buff[6], (byte)TYPE.AXIS))
                {
                    return new JoystickEventArgs() { Axis = buff[7], IsButtonEvent = false, Value = 0 };
                }
                else if (checkBit(buff[6], (byte)TYPE.BUTTON))
                {
                    return new JoystickEventArgs() { Button = buff[7], IsButtonEvent = true, IsPressed = false };
                }
                else
                {
                    return null;
                }
            }
            else // Everything else is a value.
            {
                if (checkBit(buff[6], (byte)TYPE.AXIS))
                {
                    short rawValue = BitConverter.ToInt16(buff, 4);
                    float value = ((float)rawValue) / (float)ushort.MaxValue;
                    value *= 2;
                    return new JoystickEventArgs() { Axis = buff[7], IsButtonEvent = false, Value = value };
                }
                else if (checkBit(buff[6], (byte)TYPE.BUTTON))
                {
                    bool value = buff[4] == (byte)STATE.PRESSED;
                    return new JoystickEventArgs() { Button = buff[7], IsButtonEvent = true, IsPressed = value };
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// The state of a button.
        /// </summary>
        private enum STATE : byte
        {
            PRESSED = 0x01,
            RELEASED = 0x00

        }

        // The type of input.
        private enum TYPE : byte
        {
            AXIS = 0x02,
            BUTTON = 0x01

        }

        // The mode of the buffer input.
        private enum MODE : byte
        {
            CONFIGURATION = 0x80,
            VALUE = 0x00

        }

        /// <summary>
        /// Helper function to check if value has bit flag set to 1.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <param name="flag">The flag to check in value.</param>
        /// <returns><c>True</c> if flag was set if value, <c>false</c> if not.</returns>
        bool checkBit(byte value, byte flag)
        {
            byte c = (byte)(value & flag);
            return c == (byte)flag;
        }
    }

}