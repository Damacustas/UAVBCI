using System;
using System.Collections.Generic;

namespace UAV.Simulation
{
	public class WorldState
	{
		/// <summary>
		/// The current location of the drone.
		/// </summary>
		/// <value>The drone location.</value>
		public Vector2D DroneLocation { get; set; }

		/// <summary>
		/// The list of targets the drone is to reach (in order).
		/// </summary>
		/// <value>The targets.</value>
		public List<Vector2D> Targets { get; set; }

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

		/// <summary>
		/// Determines whether the given given location is at the target.
		/// </summary>
		/// <returns><c>true</c> if the given location is at the current target; otherwise, <c>false</c>.</returns>
		/// <param name="l">L.</param>
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

