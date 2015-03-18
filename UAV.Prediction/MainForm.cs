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

        private const double maxDeviance = 1.0;

        public MainForm()
        {
            InitializeComponent();


            double startX = -1;
            double endX = 3;

            for(double x = startX; x < endX; x+= 0.1)
            {
                double rv = (r.NextDouble() * 2 - 1) * maxDeviance;

                X.Add(x);
                Y.Add(Math.Pow(x, 3) - 2 * Math.Pow(x, 2) + 1 + rv);
            }


            // Create plots and add to plotview.
            var scatter = new ScatterSeries();
            for (int i = 0; i < X.Count; i++)
            {
                scatter.Points.Add(new ScatterPoint(X[i], Y[i]));
            }

            var model = new PlotModel();
            model.Title = "Plot";
            model.LegendPosition = LegendPosition.TopLeft;
            
            model.Series.Add(new FunctionSeries(CreateNewPlot(1), startX, endX, 0.05, "f1"));
            model.Series.Add(new FunctionSeries(CreateNewPlot(2), startX, endX, 0.05, "f2"));
            model.Series.Add(new FunctionSeries(CreateNewPlot(3), startX, endX, 0.05, "f3"));
            

            for (int i = 0, j = 1; i < X.Count - 9; i += 10, j++)
            {
                model.Series.Add(new FunctionSeries(CreateFitSubset(i, i + 9), X[i], X[i + 9], 0.05, "partial " + j));
            }

            model.Series.Add(scatter);
            plotView.Model = model;
        }

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
