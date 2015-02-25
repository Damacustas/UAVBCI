using System;
using System.Threading;
using UAV.Joystick;
using AR.Drone.Client;

namespace UAV
{
	public class JoystickFlightController
	{
		private int cmds;
		private Thread thread;
		public DroneClient Client {get;set;}

		public void Start()
		{
			cmds = 0;
            thread = new Thread(Run);
			thread.Start();
		}

		private void Run()
		{
			var js = new Joystick.JoystickDevice();
			js.Initialize("/dev/input/js0");
			js.InputReceived += Js_InputReceived;

			Client.ResetEmergency();

			var dt = DateTime.Now;
			var dt2 = DateTime.Now;
			while (true)
			{
				js.ProcessEvents();

				if (DateTime.Now - dt2 > new TimeSpan(0, 0, 0, 0, 100))
				{
					float gaz = 0.0f;
					if (js.ButtonStates[9])
						gaz = 1.0f;
					else if (js.ButtonStates[10])
						gaz = -1.0f;

					Client.Progress(AR.Drone.Client.Command.FlightMode.Progressive,
						roll: js.AxisValues[0],			// X-axis
						pitch: js.AxisValues[1],		// Y-axis
						yaw: js.AxisValues[3] * 0.5f,	// Z-axis
						gaz: gaz);	// Throttle, inverted.
					dt2 = DateTime.Now;
					cmds++;
				}
			}
		}

		private void Js_InputReceived(object sender, JoystickEventArgs e)
		{
			if (e.IsButtonEvent)
			{
				if (e.Button == 0 && e.IsPressed) // Front button
				{
					Client.Hover();
					cmds++;
				}
				else if (e.Button == 1 && e.IsPressed) // Pad-2 button
				{
					Client.Emergency();
					cmds++;
				}
				else if(e.Button == 2 && e.IsPressed) // Pad-3 button
				{
					Client.Takeoff();
					cmds++;
				}
				else if(e.Button == 4 && e.IsPressed) // Pad-5 button
				{
					Client.Land();
					cmds++;
				}
			}
		}
	}
}

