using System;
using UAV.Common;

namespace UAV.Simulation
{
    public class InputGenerator
    {
        public Vector2D DirectionDeviation { get; private set; }
        public double NoisePercentage { get; private set; }

        public InputGenerator()
        {
        }

        public Vector2D ComputeNewDirection(WorldState state)
        {
            Vector2D dir = state.CurrentTarget - state.DroneLocation;
            dir = dir.GetNormalized();
            dir += DirectionDeviation;

            // Apply some noise.
            return dir;
        }
    }
}

