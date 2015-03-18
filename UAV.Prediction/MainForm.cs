using System;
using System.Windows.Forms;
using System.Drawing;

using OxyPlot.WindowsForms;
using OxyPlot.Series;
using OxyPlot;

namespace UAV.Prediction
{
	public class MainForm : Form
	{
		private PlotView view;

		//private double[] X = new double[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
		//private double[] Y = new double[] { 0.2, 1.8, 4.3, 5.7, 8.3, 9.7, 12.4, 13.6, 16.3 };
		private double[] X = new double[] {0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0,
			1.1, 1.2, 1.3, 1.4, 1.5, 1.6, 1.7, 1.8, 1.9, 2.0
		};
		private double[] Y = new double[] {0.1, 0.3, 0.5, 0.8, 1.3, 2.0, 2.5, 3.0, 4.0, 5.0,
			6,   7,   8, 9.5,  11, 12.5,  11,  19,  17, 22
		};


		private Func<double, double> GenerateFunction()
		{
			var coeff = Fitters.FitQuadratic(X, Y);
			return (x) =>
			{
				return coeff.At(0, 0) + coeff.At(1, 0) * x + coeff.At(2, 0) * x * x;
			};
		}

		public MainForm()
		{
			this.InitalizeComponent();

			var scatter = new ScatterSeries();
			for (int i = 0; i < X.Length; i++)
			{
				scatter.Points.Add(new ScatterPoint(X[i], Y[i]));
			}

			var model = new PlotModel();
			model.Title = "Plot";
			model.Series.Add(new FunctionSeries(GenerateFunction(), 0, 2, 0.1, "f"));
			model.Series.Add(scatter);
			view.Model = model;

		}

		private void InitalizeComponent()
		{
			this.SuspendLayout();
			view = new PlotView();
			view.Dock = DockStyle.Fill;
			view.PanCursor = Cursors.Hand;

			this.Text = "Plotview";
			this.Size = new Size(640, 360);
			this.Controls.Add(view);

			this.ResumeLayout(false);
		}
	}

}
