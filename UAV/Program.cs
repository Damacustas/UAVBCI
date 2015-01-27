using AR.Drone.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAV.Joystick;
using System.Windows.Forms;

namespace UAV
{
    class Program
    {
        internal static DroneClient client = new DroneClient();

		static int cmds = 0;

        static void Main(string[] args)
        {
			new Thread(() =>
			{
				var js = new Joystick.Joystick();
				js.Initialize("/dev/input/js0");
				js.InputReceived += Js_InputReceived;

				Console.WriteLine("Activating drone...");
				client.Start();
				Console.WriteLine("Drone activated!");

				Thread.Sleep(1000);

				var dt = DateTime.Now;
				var dt2 = DateTime.Now;
				while (true)
				{
					js.ProcessEvents();

					if (DateTime.Now - dt2 > new TimeSpan(0, 0, 0, 0, 100))
					{
						client.Progress(AR.Drone.Client.Command.FlightMode.Progressive,
							roll: js.AxisValues[0],			// X-axis
							pitch: js.AxisValues[1],		    // Y-axis
							yaw: js.AxisValues[3] * 0.25f,	// Z-axis
							gaz: js.AxisValues[2] * -1.0f);	// Throttle, inverted.
						dt2 = DateTime.Now;
						cmds++;
					}

					if (DateTime.Now - dt > new TimeSpan(0, 0, 1))
					{
						Console.WriteLine("Send {0} commands in the last second.", cmds);
						cmds = 0;
						dt = DateTime.Now;
					}
				}
			}).Start();

			Application.Run(new VideoForm());
        }

        private static void Js_InputReceived(object sender, JoystickEventArgs e)
        {
            if (e.IsButtonEvent)
            {
                if (e.Button == 0 && e.IsPressed) // Front button
                {
                    client.Hover();
					cmds++;
                }
                else if (e.Button == 1 && e.IsPressed) // Pad-2 button
                {
					client.Emergency();
					cmds++;
                }
                else if(e.Button == 2 && e.IsPressed) // Pad-3 button
                {
					client.Takeoff();
					cmds++;
                }
                else if(e.Button == 4 && e.IsPressed) // Pad-5 button
                {
					client.Land();
					cmds++;
                }
            }
        }
    }
}
