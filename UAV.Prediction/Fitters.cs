using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;

namespace UAV.Prediction
{
	public class Fitters
	{
		public static Matrix<double> FitLinear(ICollection<double> x, ICollection<double> y)
		{
			// Compute the various fields of the matrices created below.
			double sumX = MathTools.Sum(x);
			double sumY = MathTools.Sum(y);
			double sumXY = MathTools.Sum(MathTools.Mul(x, y));
			double sumX2 = MathTools.Sum(MathTools.Pow(x, 2));
			double n = x.Count;

			// Build the A matrix.
			var A = Matrix<double>.Build.DenseOfArray(
				new double[2,2]
				{
					{n, sumX},
					{sumX, sumX2}
				});

			// Build the B matrix.
			var B = Matrix<double>.Build.DenseOfArray(
				new double[2, 1]
				{
					{ sumY },
					{ sumXY }
				});

			// Compute the inverse of the A matrix.
			var AInv = A.Inverse();

			// Compute the coefficients.
			var coef = AInv * B;

			// Return the coefficients.
			return coef;
		}

		public static Matrix<double> FitQuadratic(ICollection<double> t, ICollection<double> y)
		{
			/*
			A = [a11 a12 a13
				a21 a22 a23
				a31 a32 a33]

				a11 = n
				a12 = sum(t)
				a13 = sum(t^2)

				a21 = sum(t)
				a22 = sum(t^2)
				a23 = sum(t^3)

				a31 = sum(t^2)
				a32 = sum(t^3)
				a33 = sum(t^4)

			B = [b1; b2; b3]
				b1 = sum(y)
				b2 = sum(y*t)
				b3 = sum(y*t^2)
			*/

			double n = t.Count;
			double sumT = MathTools.Sum(t);
			double sumT2 = MathTools.Sum(MathTools.Pow(t, 2));
			double sumT3 = MathTools.Sum(MathTools.Pow(t, 3));
			double sumT4 = MathTools.Sum(MathTools.Pow(t, 4));
			double sumY = MathTools.Sum(y);
			double sumYT = MathTools.Sum(MathTools.Mul(y, t));
			double sumYT2 = MathTools.Sum(MathTools.Mul(y, MathTools.Pow(t, 2)));

			var A = Matrix<double>.Build.DenseOfArray(new double[3, 3] {
				{n, sumT, sumT2},
				{sumT,sumT2,sumT3},
				{sumT2,sumT3,sumT4}
			});

			var B = Matrix<double>.Build.DenseOfArray(new double[3,1] {
				{sumY},
				{sumYT},
				{sumYT2}
			});

			var InvA = A.Inverse();

			var coeff = InvA * B;

			return coeff;
		}
	}
}

