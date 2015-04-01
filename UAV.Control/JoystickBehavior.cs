using System;
using UAV.Joystick;

namespace UAV
{
	public class JoystickBehavior : IFlightBehavior
	{
		private float yaw, roll;
		private JoystickDevice j;

		public JoystickBehavior (JoystickDevice j)
		{
			yaw = roll = 0;
			j.InputReceived += Joystick_InputReceived;
			this.j = j;
		}

		void Joystick_InputReceived (object sender, JoystickEventArgs e)
		{
			if (j.ButtonStates.Count <= 8 || j.AxisValues.Count <= 1)
				return;

			if (j.ButtonStates[7])
			{
				yaw = 0.5f;
			}
			else if (j.ButtonStates[8])
			{
				yaw = -0.5f;
			}
			else
			{
				yaw = 0;
			}

			roll = j.AxisValues[0];
		}

		#region IFlightBehavior implementation

		public MovementCommand ComputeBehavior (AR.Drone.Client.DroneClient drone)
		{
			return new MovementCommand() {
				Yaw = yaw,
				Roll = roll
			};
		}

		#endregion
	}
}

