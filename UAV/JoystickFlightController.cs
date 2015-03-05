using System;
using System.Threading;
using UAV.Joystick;
using AR.Drone.Client;

namespace UAV
{
	public class JoystickFlightController
	{
		private Thread thread;
		public DroneClient Client {get;set;}

		public void Start()
		{
            thread = new Thread(Run);
			thread.Start();
		}

		private void Run()
		{
			var js = new Joystick.JoystickDevice();
			js.Initialize("/dev/input/js0");
			js.InputReceived += Js_InputReceived;

			Client.ResetEmergency();

			float lastRoll, lastPitch, lastYaw, lastGaz;
			lastRoll = lastPitch = lastYaw = lastGaz = 0.0f;

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

					if (lastGaz == gaz &&
					    lastPitch == js.AxisValues[1] &&
					    lastRoll == js.AxisValues[0] &&
					    lastYaw == js.AxisValues[3])
					{
						// Do nothing if the values remain unchanged.
					}
					else
					{
						Client.Progress(AR.Drone.Client.Command.FlightMode.Progressive,
							roll: js.AxisValues[0],			// X-axis
							pitch: js.AxisValues[1],		// Y-axis
							yaw: js.AxisValues[3] * 0.6f,	// Z-axis
							gaz: gaz);	// Throttle, inverted.

						lastGaz = gaz;
						lastPitch = js.AxisValues[1];
						lastRoll = js.AxisValues[0];
						lastYaw = js.AxisValues[3];

						dt2 = DateTime.Now;
					}
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
				}
				else if (e.Button == 1 && e.IsPressed) // Pad-2 button
				{
					Client.Emergency();
				}
				else if(e.Button == 2 && e.IsPressed) // Pad-3 button
				{
					Client.Takeoff();
				}
				else if(e.Button == 4 && e.IsPressed) // Pad-5 button
				{
					Client.Land();
				}
			}
		}
	}
}

