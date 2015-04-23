using System;
using UAV.Common;

namespace UAV.Simulation
{
    public class InputGenerator
    {
        public double InputAccuracy { get; private set; }
        private static readonly Random noiseGenerator = new Random();

        public InputGenerator(double inputAccuracy)
        {
            InputAccuracy = inputAccuracy;
        }

        public Vector2D ComputeNewDirection(WorldState state)
        {
            Vector2D dir = state.CurrentTarget - state.DroneLocation;
            dir = dir.GetNormalized();

            Vector2D noise = new Vector2D(
                                 (noiseGenerator.NextDouble() * 2) - 1,
                                 (noiseGenerator.NextDouble() * 2) - 1
                             );
            noise = noise.GetNormalized();

            int v = noiseGenerator.Next(0, 100);

            if (v < (int)(InputAccuracy * 100.0d))
            {
                return dir;
            }
            else
            {
                return noise;
            }
        }
    }
}

