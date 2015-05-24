using OxyPlot;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace UAV.Prediction
{
    public partial class MainForm : Form
    {
        List<double> X = new List<double>();
        List<double> Y = new List<double>();
        Random r = new Random(42);

        double startX = 0;
        double endX = 5;

        private const double maxDeviance = 1.0;

        public MainForm()
        {
            InitializeComponent();

            // Generate X and Y data.
            for (double x = startX; x < endX; x += 0.1)
            {
                double rv = (r.NextDouble() * 2 - 1) * maxDeviance;

                X.Add(x);
                Y.Add(Math.Pow(x, 2) + rv);
            }


            // Create plots and add to plotview.
            var scatter = new ScatterSeries();
            for (int i = 0; i < X.Count; i++)
            {
                scatter.Points.Add(new ScatterPoint(X[i], Y[i]));
            }

            // Create model.
            var model = new PlotModel();
            model.Title = "Plot";
            model.LegendPosition = LegendPosition.TopLeft;
            
            // Add 3 fitted functions: linear, quadratic and cubic.
            model.Series.Add(new FunctionSeries(CreateNewPlot(1), startX, endX + 4, 0.05, "1-polynomial"));
            model.Series.Add(new FunctionSeries(CreateNewPlot(2), startX, endX + 4, 0.05, "2-polynomial"));

            // Add the original function what was to be fitted.
            model.Series.Add(new FunctionSeries(
                    x => Math.Pow(x, 2),
                    startX, endX + 4,
                    0.05, "original")
            );
            
            /*
            for (int i = 0, j = 1; i < X.Count - 9; i += 10, j++)
            {
                model.Series.Add(new FunctionSeries(CreateFitSubset(i, i + 9), X[i], X[i + 9], 0.05, "partial " + j));
            }
            */

            model.Series.Add(scatter);
            plotView.Model = model;
        }

        /// <summary>
        /// Creates a polynomial function of the power-th order that fits the globally defined X/Y data.
        /// </summary>
        /// <returns>The fitted plot.</returns>
        /// <param name="power">The order of the function.</param>
        private Func<double, double> CreateNewPlot(int power)
        {
            return Fitters.GeneratePolynomialFit(X, Y, power);
        }

        private Func<double, double> CreateFitSubset(int startIdx, int endIdx)
        {
            var dataX = X.GetRange(startIdx, endIdx - startIdx);
            var dataY = Y.GetRange(startIdx, endIdx - startIdx);

            return Fitters.GeneratePolynomialFit(dataX, dataY, 2);
        }
    }
}
