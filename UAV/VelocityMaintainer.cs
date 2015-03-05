using AR.Drone.Data.Navigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AR.Drone.Client;

namespace UAV
{
    public class VelocityMaintainer : IFlightBehavior
    {
        public float MaxDeviance { get; set; }

        public Velocity TargetVelocity { get; set; }

        public VelocityMaintainer()
        {
            TargetVelocity = new Velocity();
            MaxDeviance = 0.1f;
        }

        public MovementCommand ComputeBehavior(DroneClient drone)
        {
			Console.WriteLine("Velocity forward: {0} Velocity sideways: {1}",
			drone.NavigationData.Velocity.X,
			drone.NavigationData.Velocity.Y);

			// Forward/Backward
			Console.Write("Pitch: ");
			var pitch = ComputeCorrection(drone.NavigationData.Pitch, TargetVelocity.Forward, drone.NavigationData.Velocity.Y, 0.025f);

			// Left/Right
			Console.Write("Roll: ");
			var roll = 0;
			Console.WriteLine("0");
			//var roll = ComputeCorrection(drone.NavigationData.Roll, TargetVelocity.Left, drone.NavigationData.Velocity.X, 0.1f);

			// Turn left/turn right
			Console.Write("Yaw: ");
			var yaw = 0;
			Console.WriteLine("0");
			//var yaw = ComputeCorrection(drone.NavigationData.Yaw, TargetVelocity.TurnLeft, drone.NavigationData.Velocity.Z, 0.01f);
            

			return new MovementCommand()
			{
				Pitch = pitch,
				Roll = roll,
				Yaw = yaw
			};
		}

		private float ComputeCorrection(float value, float targetVelocity, float currentVelocity, float correctionValue)
        {
			float ret;
			string action = "";

            if(currentVelocity > targetVelocity + MaxDeviance)
            {
                // Too fast, correct value down.
				ret = value - correctionValue;
				action = "decr";
            }
            else if(currentVelocity < targetVelocity - MaxDeviance)
            {
                // Too slow, correct value up.
				ret = value + correctionValue;
				action = "incr";
            }
            else
            {
                ret = value;
				action = "keep";
            }

			Console.WriteLine("{2}: {0} -> {1}", value, ret, action);

			if (ret > 1.0f)
				return 1.0f;
			else if (ret < -1.0f)
				return -1.0f;
			else
				return ret;
        }
    }
}
