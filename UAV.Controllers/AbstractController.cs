using System;
using AR.Drone.Client;

namespace UAV.Controllers
{
	public abstract class AbstractController
	{
		public DroneClient Drone { get; set; }
		public ControllerState State { get; private set; }

		protected AbstractController ()
		{
		}

		public void StartController()
		{
			State = ControllerState.Control;
			Drone.Takeoff ();
		}

		public void StopController()
		{
			State = ControllerState.Hover;
			Drone.Land ();
		}

		public void EnterHoverState()
		{
			State = ControllerState.Hover;
			Drone.Hover ();
		}

		public void LeaveHoverState()
		{
			State = ControllerState.Control;
		}

		protected void SendFlightCommand(float pitch, float roll, float yaw, float gaz)
		{
			if (State == ControllerState.Control)
			{
				Drone.Progress (AR.Drone.Client.Command.FlightMode.Progressive, roll, pitch, yaw, gaz);
			}
		}


		public enum ControllerState
		{
			Hover,
			Control
		}
	}
}

