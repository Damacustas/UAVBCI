using System;
using System.Collections.Generic;
using UAV.Common;

namespace UAV.Simulation
{
	public class WorldState
	{
        public List<HistoryItem> LocationHistory { get; private set; }
        public List<HistoryItem> InputHistory { get; private set; }

        /// <summary>
        /// Gets the current target.
        /// </summary>
        /// <value>The current target.</value>
        public Vector2D CurrentTarget { get; set; }

        public Vector2D DroneLocation
        {
            get
            {
                return LocationHistory[LocationHistory.Count - 1].Value;
            }
        }

        public WorldState()
        {
            LocationHistory = new List<HistoryItem>();
            InputHistory = new List<HistoryItem>();
        }

        public void MoveDrone(Vector2D location, Vector2D input, double time)
        {
            LocationHistory.Add(new HistoryItem(location, time));
            InputHistory.Add(new HistoryItem(input, time));
        }
	}
}

