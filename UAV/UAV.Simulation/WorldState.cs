using System;
using System.Collections.Generic;
using UAV.Common;

namespace UAV.Simulation
{
	public class WorldState
	{
		/// <summary>
		/// The list of targets the drone is to reach (in order).
		/// </summary>
		/// <value>The targets.</value>
		public List<Vector2D> Targets { get; set; }

        public List<Tuple<double, Vector2D>> LocationHistory { get; private set; }

		/// <summary>
		/// Gets or sets the maximal target deviation.
		/// </summary>
		/// <value>The max target deviation.</value>
		public static double MaxTargetDeviation { get; set; }

		/// <summary>
		/// Gets the current target.
		/// </summary>
		/// <value>The current target.</value>
		public Vector2D CurrentTarget
		{
			get
			{
				return Targets[0];
			}
		}

        public Vector2D DroneLocation
        {
            get
            {
                return LocationHistory[LocationHistory.Count - 1].Item2;
            }
        }

        public void MoveDrone(Vector2D location, double time)
        {
            LocationHistory.Add(new Tuple<double, Vector2D>(time, location));
        }

		/// <summary>
		/// Determines whether the given given location is at the target.
		/// </summary>
		/// <returns><c>true</c> if the given location is at the current target; otherwise, <c>false</c>.</returns>
		public bool AtTarget
		{
			get
			{
				return CurrentTarget.Distance(CurrentTarget) < MaxTargetDeviation;
			}
		}

		public bool IsFinished
		{
			get
			{
				return AtTarget && Targets.IndexOf(CurrentTarget) == Targets.Count - 1;
			}
		}
	}
}

