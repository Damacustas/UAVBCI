using System;
using System.Collections.Generic;
using UAV.Common;

namespace UAV.Simulation
{
	public class WorldState
	{
        public List<Tuple<double, Vector2D>> LocationHistory { get; private set; }
        public List<Tuple<double, Vector2D>> InputHistory { get; private set; }

        /// <summary>
        /// Gets the current target.
        /// </summary>
        /// <value>The current target.</value>
        public Vector2D CurrentTarget { get; set; }

        public Vector2D DroneLocation
        {
            get
            {
                return LocationHistory[LocationHistory.Count - 1].Item2;
            }
        }

        public WorldState()
        {
            LocationHistory = new List<Tuple<double, Vector2D>>();
            InputHistory = new List<Tuple<double, Vector2D>>();
        }

        public void MoveDrone(Vector2D location, Vector2D input, double time)
        {
            LocationHistory.Add(new Tuple<double, Vector2D>(time, location));
            InputHistory.Add(new Tuple<double, Vector2D>(time, input));
        }
	}
}

