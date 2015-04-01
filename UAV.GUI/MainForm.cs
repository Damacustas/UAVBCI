using AR.Drone.Client;
using AR.Drone.Video;
using System;
using System.Windows.Forms;
using AR.Drone.Data;
using System.Drawing;
using UAV.Joystick;

namespace UAV.GUI
{
    public partial class MainForm : Form
    {
        // Video related members.
        private uint frameNumber;
        private Bitmap frameBitmap;
        private VideoPacketDecoderWorker videoDecoder;
        private VideoFrame frame;

        // The drone.
        private DroneClient drone;

        // Controll related members.
        private JoystickDevice joystick;
        private HeightMaintainer heightMaintainer;
        private VelocityMaintainer velocityMaintainer;
        private CompositeFlightController controller;

        public MainForm()
        {
            InitializeComponent();

            // innitialize video packet decoder.
            videoDecoder = new VideoPacketDecoderWorker(PixelFormat.BGR24, true, OnFrameDecoded);
            videoDecoder.UnhandledException += VideoDecoder_UnhandledException;
            videoDecoder.Start();

            // Initalize drone.
            drone = new DroneClient();
            drone.VideoPacketAcquired += OnVideoPacketAqcuired;
            drone.Start();
			drone.ResetEmergency();
            drone.FlatTrim();

            
            JoystickFlightController controller = new JoystickFlightController();
            controller.Client = drone;
            controller.Start();

			/*
            // Initialize joystick.
            joystick = new JoystickDevice();
            joystick.InputReceived += Joystick_InputReceived;
            joystick.Initialize("/dev/input/js0");

            // Initialize height maintainer.
            heightMaintainer = new HeightMaintainer();
			heightMaintainer.TargetHeight = 0.25f; //meter

            // Initialize velocity maintainer.
            velocityMaintainer = new VelocityMaintainer();
			velocityMaintainer.TargetVelocity.Forward = -1.0f;
            velocityMaintainer.TargetVelocity.Left = 0.0f;
            velocityMaintainer.TargetVelocity.TurnLeft = 0.0f;

            // Initialize flight controller.
            controller = new CompositeFlightController();
            controller.Drone = drone;
            controller.FlightBehaviors.Add(velocityMaintainer);
            controller.FlightBehaviors.Add(heightMaintainer);
			controller.FlightBehaviors.Add(new JoystickBehavior(joystick));
            controller.ControlCycleStarting += () => joystick.ProcessEvents();

            controller.Start();
			*/

            videoTimer.Enabled = true;
        }

        private void Joystick_InputReceived(object sender, JoystickEventArgs e)
        {
            if (e.Button == 0 && e.IsPressed) // Front button
            {
                drone.Hover();
            }
            else if (e.Button == 1 && e.IsPressed) // Pad-2 button
            {
                drone.Emergency();
            }
            else if (e.Button == 2 && e.IsPressed) // Pad-3 button
            {
                drone.Takeoff();
            }
            else if (e.Button == 4 && e.IsPressed) // Pad-5 button
            {
                drone.Land();
            }
            else if(e.Button == 9 && e.IsPressed) // Throttle-10 button
            {
                heightMaintainer.TargetHeight += 0.25f; // meter
            }
            else if(e.Button == 10 && e.IsPressed) // Throttle-11 button
            {
                heightMaintainer.TargetHeight -= 0.25f; // meter
            }
        }

        private void OnFrameDecoded(VideoFrame frame)
        {
            this.frame = frame;
        }

        private void VideoDecoder_UnhandledException(object arg1, Exception arg2)
        {
            Console.WriteLine(arg2);
        }

        private void OnVideoPacketAqcuired(VideoPacket packet)
        {
            // If the video decoder is alive, enqueue the packet to be decoded.
            if (videoDecoder.IsAlive)
            {
                videoDecoder.EnqueuePacket(packet);
            }
        }

        private void VideoTimer_Tick(object sender, EventArgs e)
        {
            // If there is no (new) frame, ignore.
            if (frame == null || frameNumber == frame.Number)
                return;

            frameNumber = frame.Number;

            // If we already have a bitmap, update it, else, create new one.
            if (frameBitmap == null)
                frameBitmap = VideoHelper.CreateBitmap(ref frame);
            else
                VideoHelper.UpdateBitmap(ref frameBitmap, ref frame);

            // Set image.
            videoPictureBox.Image = frameBitmap;
        }

        private void MainForm2_KeyUp(object sender, KeyEventArgs e)
        {
            // If escape is pressed, quit the application.
			if (e.KeyCode == Keys.Escape)
			{
				drone.Land();
				drone.Stop();

				Application.Exit();
			}
			else if (e.KeyCode == Keys.Space)
			{
				drone.Land();
			}
        }
    }
}
