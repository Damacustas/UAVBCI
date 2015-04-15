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



            //Console.WriteLine("n_x: {0}, n_y: {1}", x_noise, y_noise);

            return new Vector2D(
                x: noise.X * (1.0 - InputAccuracy) + dir.X * InputAccuracy,
                y: noise.Y * (1.0 - InputAccuracy) + dir.Y * InputAccuracy
            ).GetNormalized();
        }
    }
}

