using AR.Drone.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAV.Joystick;

namespace UAV
{
    class Program
    {
        static DroneClient client = new DroneClient();

        static void Main(string[] args)
        {
            var js = new Joystick.Joystick();
            js.Initialize("/dev/input/js0");
            js.InputReceived += Js_InputReceived;

            Thread.Sleep(500); // 500ms should be enough to open the file and read config.

            if(js.AxisValues[2] != 0)
            {
                Console.WriteLine("gaz control != 0, not starting!");
                return;
            }

            client.Start();

            while (true)
            {
				js.ProcessEvents();
            }
        }

        private static void Js_InputReceived(object sender, JoystickEventArgs e)
        {
            if (e.IsButtonEvent)
            {
                if (e.Button == 0 && e.IsPressed)
                {
                    client.Hover();
                }
                else if (e.Button == 1 && e.IsPressed)
                {
                    client.Emergency();
                }
                else if(e.Button == 2 && e.IsPressed)
                {
                    client.Takeoff();
                }
                else if(e.Button == 3 && e.IsPressed)
                {
                    client.Land();
                }
            }
            else
            {
                if (e.Axis == 0)
                {
                    client.Progress(AR.Drone.Client.Command.FlightMode.Progressive, roll: 0.05f * e.Value);
                }
                else if (e.Axis == 1)
                {
                    client.Progress(AR.Drone.Client.Command.FlightMode.Progressive, pitch: 0.05f * e.Value);
                }
                else if(e.Axis == 2)
                {
                    client.Progress(AR.Drone.Client.Command.FlightMode.Progressive, gaz: 0.25f * e.Value);
                }
                else if(e.Axis == 3)
                {
                    client.Progress(AR.Drone.Client.Command.FlightMode.Progressive, yaw: 0.05f * e.Value);
                }
            }
        }
    }
}
