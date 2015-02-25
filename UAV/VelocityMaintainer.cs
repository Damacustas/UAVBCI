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
            return new MovementCommand()
            {
                Yaw = ComputeCorrection(drone.NavigationData.Yaw, TargetVelocity.X, drone.NavigationData.Velocity.X),
                Roll = ComputeCorrection(drone.NavigationData.Roll, TargetVelocity.Y, drone.NavigationData.Velocity.Y),
                Pitch = ComputeCorrection(drone.NavigationData.Pitch, TargetVelocity.Z, drone.NavigationData.Velocity.Z)
            };
        }

        private float ComputeCorrection(float value, float targetVelocity, float currentVelocity)
        {
            if(currentVelocity > targetVelocity + MaxDeviance)
            {
                // Too fast, correct value down.
                return value - 0.1f;
            }
            else if(currentVelocity < targetVelocity - MaxDeviance)
            {
                // Too slow, correct value up.
                return value + 0.1f;
            }
            else
            {
                return value;
            }
        }
    }
}
