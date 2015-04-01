using System;
using System.Collections.Generic;

namespace UAV.Simulation
{
	public class State
	{
		public Location DroneLocation { get; set; }
		public List<Location> Targets { get; set; }

		public double MaxTargetDeviation { get; set; }

		public Location NextTarget
		{
			get
			{
				return Targets[0];
			}
		}

		public bool IsPointAtTarget(Location l)
		{
			return Math.Sqrt(
		}
	}
}

