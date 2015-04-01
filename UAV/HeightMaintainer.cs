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
        private static readonly float MaxDeviance = 0.10f; // meter
		private float targetHeight;

        public float TargetHeight
		{
			get { return targetHeight; }
			set
			{
				targetHeight = (float)Math.Max(value, 0.25f);
			}
		}

        public HeightMaintainer()
        {
            TargetHeight = 1.0f; // meter
        }

        public MovementCommand ComputeBehavior(DroneClient drone)
        {
            if(drone.NavigationData.Altitude >= TargetHeight + MaxDeviance )
            {
				//Console.WriteLine("Too high: {0}, target: {1}, margin:{2}",
				//	drone.NavigationData.Altitude,
				//	TargetHeight,
				//	MaxDeviance);

				var gaz = -Math.Min(drone.NavigationData.Altitude - TargetHeight, 1.0f);
				Console.WriteLine("Delta-gaz: {0}", gaz);

                // Incorrect height, go downwards.
                return new MovementCommand()
                {
					Gaz = gaz
                };
            }
            else if(drone.NavigationData.Altitude <= TargetHeight - MaxDeviance)
			{
				//Console.WriteLine("Too low: {0}, target: {1}, margin:{2}",
				//	drone.NavigationData.Altitude,
				//	TargetHeight,
				//	MaxDeviance);
                
				var gaz = Math.Min(TargetHeight - drone.NavigationData.Altitude, 1.0f);
				Console.WriteLine("Delta-gaz: {0}", gaz);

				// Incorrect height, go upwards.
                return new MovementCommand()
                {
					Gaz = gaz
                };
            }
            else
			{
				//Console.WriteLine("Correct height: {0}, target: {1}, margin:{2}",
				//	drone.NavigationData.Altitude,
				//	TargetHeight,
				//	MaxDeviance);

                return new MovementCommand()
                {
                    Gaz = 0.0f
                };
            }
        }
    }
}
