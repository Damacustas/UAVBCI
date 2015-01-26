using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UAV.Joystick;

namespace UAV
{
    class Program
    {
        static void Main(string[] args)
        {
            var js = new Joystick.Joystick();
            js.Initialize("/dev/input/js0");
            js.InputReceived += Js_InputReceived;

            while (true)
            {
                js.ProcessChanges();
            }
        }

        private static void Js_InputReceived(object sender, JoystickEventArgs e)
        {
            if(e.IsButtonEvent)
            {
                Console.WriteLine("Button{0}: {1}", e.Button, e.IsPressed);
            }
            else
            {
                Console.WriteLine("Axis{0}: {1}", e.Axis, e.Value);
            }
        }
    }
}
