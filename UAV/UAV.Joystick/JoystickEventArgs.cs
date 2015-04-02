using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV.Joystick
{
    public class JoystickEventArgs : EventArgs
    {
        public bool IsButtonEvent { get; internal set; }

        public float Value { get; internal set; }
        public byte Axis { get; internal set; }

        public bool IsPressed { get; internal set; }
        public byte Button { get; internal set; }
    }

    public delegate void JoystickInputDelegate(object sender, JoystickEventArgs e);
}
