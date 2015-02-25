using AR.Drone.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UAV
{
    public class HeightMaintainer : IFlightBehavior
    {
        private static readonly float MaxDeviance = 10; // cm

        public float TargetHeight { get; set; }

        public HeightMaintainer()
        {
            TargetHeight = 100; // cm
        }

        public MovementCommand ComputeBehavior(DroneClient drone)
        {
            if(drone.NavigationData.Altitude >= TargetHeight + MaxDeviance )
            {
                // Incorrect height, go downwards.
                return new MovementCommand()
                {
                    Gaz = -0.2f
                };
            }
            else if(drone.NavigationData.Altitude <= TargetHeight - MaxDeviance)
            {
                // Incorrect height, go upwards.
                return new MovementCommand()
                {
                    Gaz = 0.2f
                };
            }
            else
            {
                return new MovementCommand()
                {
                    Gaz = 0.0f
                };
            }
        }
    }
}
