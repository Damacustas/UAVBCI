using System;
using UAV.Common;

namespace UAV.Simulation
{
    public class Simulation
    {
        private double time;

        /// <summary>
        /// Represents the state of the simulation.
        /// </summary>
        /// <value>The state.</value>
        public WorldState State { get; set; }

        /// <summary>
        /// The maximum amount of simulation steps.
        /// </summary>
        /// <value>The max simulation steps.</value>
        public int MaxSimulationSteps { get; set; }

        /// <summary>
        /// The input commandstream generator.
        /// </summary>
        /// <value>The input generator.</value>
        public AbstractDirectionModifier InputGenerator { get; set; }

        /// <summary>
        /// The Intelligence aiding the input.
        /// </summary>
        /// <value>The intelligence.</value>
        public IIntelligence Intelligence { get; set; }

        /// <summary>
        /// The amount [0,1] of control the inteligence has.
        /// </summary>
        /// <value>The intelligence factor.</value>
        public double IntelligenceFactor { get; set; }

        public Vector2D StartLocation
        {
            set
            {
                State.LocationHistory.Clear();
                State.MoveDrone(value, 0);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UAV.Simulation.Simulation"/> class.
        /// </summary>
        public Simulation()
        {
            time = 0;
        }

        public void Run()
        {
            while (!State.IsFinished && time < (double)MaxSimulationSteps)
            {
                // Compute input movement direction.
                Vector2D dir_input = InputGenerator.ComputeNewDirection(State);

                // Send state to INTELLIGENCE, receive alternate movement vector.
                Vector2D dir_intelligence = Intelligence.ComputeCommand(State, dir_input);

                // Combine movement vectors.
                double x = dir_intelligence.X * IntelligenceFactor + dir_input.X * (1 - IntelligenceFactor);
                double y = dir_intelligence.Y * IntelligenceFactor + dir_input.Y * (1 - IntelligenceFactor);

                // Apply command.
                Vector2D newPos = State.DroneLocation + new Vector2D(x, y);

                // Go to next step.
                time++;
                State.MoveDrone(newPos, time);

            }
        }
    }
}

