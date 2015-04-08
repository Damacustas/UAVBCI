using System;
using System.Linq;
using UAV.Prediction;
using UAV.Common;

namespace UAV.Simulation
{
    public class FitIntelligence : IIntelligence
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UAV.Simulation.FitIntelligence"/> class.
        /// </summary>
        /// <param name="historyLength">History length.</param>
        /// <param name="functionDegree">Function degree.</param>
        public FitIntelligence(int historyLength, int functionDegree)
        {
            HistoryLength = historyLength;
            FitFunctionDegree = functionDegree;
        }

        /// <summary>
        /// Gets or sets the fit function degree.
        /// </summary>
        /// <value>The fit function degree.</value>
        public int FitFunctionDegree { get; set; }

        /// <summary>
        /// Gets or sets the length of the history.
        /// </summary>
        /// <value>The length of the history.</value>
        public int HistoryLength { get; set; }

        #region IIntelligence implementation

        public string IntelligenceName
        {
            get
            {
                return "FitIntelligence";
            }
        }

        public Vector2D ComputeCommand(WorldState worldState, Vector2D baseCommand)
        {
            // Create a range for t values.
            var ts = new Range(0, Math.Min(HistoryLength, worldState.InputHistory.Count), 1);

            // Fit a function on input for x-axis.
            Func<double, double> funcX = 
                Fitters.GeneratePolynomialFit(
                    ts,
                    (from l in worldState.InputHistory.TakeLast(HistoryLength)
                        select l.Value.X).ToList(),
                    FitFunctionDegree
                );

            // Fit a function on input fo y-axis.
            Func<double, double> funcY = 
                Fitters.GeneratePolynomialFit(
                    ts,
                    (from l in worldState.InputHistory.TakeLast(HistoryLength)
                        select l.Value.Y).ToList(),
                    FitFunctionDegree
                );

            // Predict next value and return.
            double time = HistoryLength + 1;
            var ret = new Vector2D(funcX(time), funcY(time));

            if (double.IsNaN(ret.X))
                ret.X = 0.0;
            if (double.IsNaN(ret.Y))
                ret.Y = 0.0;

            return ret;
        }

        #endregion
    }
}

