using System;
using UAV.Common;

namespace UAV.Simulation
{
    public class InputGenerator
    {
        public Vector2D DirectionDeviation { get; private set; }
        public double NoiseRatio { get; private set; }
        private static readonly Random noiseGenerator = new Random();

        public InputGenerator(double noise) : this(noise, new Vector2D())
        {
        }

        public InputGenerator(double noise, Vector2D deviance)
        {
            DirectionDeviation = deviance;
            NoiseRatio = noise;
        }

        public Vector2D ComputeNewDirection(WorldState state)
        {
            Vector2D dir = state.CurrentTarget - state.DroneLocation;
            dir = dir.GetNormalized();
            dir += DirectionDeviation;

            double x_noise = (noiseGenerator.NextDouble() * 2) - 1;
            double y_noise = (noiseGenerator.NextDouble() * 2) - 1;

            return new Vector2D(
                x: x_noise * NoiseRatio + dir.X * (1 - NoiseRatio),
                y: y_noise * NoiseRatio + dir.Y * (1 - NoiseRatio)
            ).GetNormalized();
        }
    }
}

