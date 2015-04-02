using System;

namespace UAV.Simulation
{
    public class Simulation
    {
        private int steps;

        public WorldState State { get; set; }
        private int MaxSimulationSteps { get; set; }
        public AbstractDirectionModifier InputGenerator { get; set; }

        public Simulation()
        {
            steps = 0;
        }

        public void Run()
        {
            while (!State.IsFinished && steps < MaxSimulationSteps)
            {
                // Compute input movement direction.
                Vector2D dir_input = InputGenerator.ComputeNewDirection(State);

                // Send state to INTELLIGENCE, receive alternate movement vector.
                //Vector2D dir_intelligence = SPARKLES_SPARKLES_EVERYWHERE(UNICORN);

                // Combine movement vectors, compute new location.


                steps++;
            }
        }
    }
}

