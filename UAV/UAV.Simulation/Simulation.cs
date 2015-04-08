using System;
using UAV.Common;
using System.Collections.Generic;

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
        public InputGenerator InputGenerator { get; set; }

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

        /// <summary>
        /// Gets or sets the maximal target deviation.
        /// </summary>
        /// <value>The max target deviation.</value>
        public static double MaxTargetDeviation { get; set; }

        /// <summary>
        /// The list of targets the drone is to reach (in order).
        /// </summary>
        /// <value>The targets.</value>
        public List<Vector2D> Targets { get; set; }

        /// <summary>
        /// Sets the starting location of the drone.
        /// </summary>
        /// <value>The start location.</value>
        public Vector2D StartLocation { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UAV.Simulation.Simulation"/> class.
        /// </summary>
        public Simulation()
        {
            time = 0;
        }


        /// <summary>
        /// Run this instance of the simulation.
        /// </summary>
        public void Run()
        {
            State.MoveDrone(StartLocation, new Vector2D(0,0), 0);
            State.CurrentTarget = Targets[0];

            while (!IsFinished() && time < (double)MaxSimulationSteps)
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
                State.MoveDrone(newPos, dir_input, time);

                // Advance to next target if required.
                if (AtTarget())
                {
                    if (!IsFinished())
                    {
                        State.CurrentTarget = Targets[Targets.IndexOf(State.CurrentTarget) + 1];
                    }
                }
            }
        }

        /// <summary>
        /// Determines whether the given given location is at the target.
        /// </summary>
        /// <returns><c>true</c> if the given location is at the current target; otherwise, <c>false</c>.</returns>
        private bool AtTarget()
        {
            return State.CurrentTarget.Distance(State.DroneLocation) < MaxTargetDeviation;
        }

        /// <summary>
        /// Determines whether this simulation is finished.
        /// </summary>
        /// <returns><c>true</c> if this instance is finished; otherwise, <c>false</c>.</returns>
        public bool IsFinished()
        {
            return AtTarget() && Targets.IndexOf(State.CurrentTarget) == Targets.Count - 1;
        }
    }
}

