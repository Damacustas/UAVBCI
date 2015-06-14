using System;
using MathNet.Numerics.LinearAlgebra;
using System.Collections.Generic;
using UAV.Common;

namespace UAV.Prediction
{
    public class Fitters
    {
        /// <summary>
        /// Creates a polynomial function delegate from the given t and y values. 
        /// </summary>
        /// <param name="ts">The t-values (x-axis)</param>
        /// <param name="ys">The y-values (y-axis)</param>
        /// <param name="degree">The degree of the polynomial function to create.</param>
        /// <returns>A delegate accepting and t value and returning a y value. f: double -> double</returns>
        public static Func<double, double> GeneratePolynomialFit(IReadOnlyCollection<double> ts, IReadOnlyCollection<double> ys, int degree)
        {
            Matrix<double> coefficients = FitPolynomialCoefficients(ts, ys, degree);
            return GeneratePolynomialFit(coefficients);
        }

        /// <summary>
        /// Creates a polynomial function delegate from the given coefficients.
        /// </summary>
        /// <param name="coefficients">The coefficients to use in the polynomial function.</param>
        /// <returns>A delegate accepting and t value and returning a y value. f: double -> double</returns>
        public static Func<double, double> GeneratePolynomialFit(Matrix<double> coefficients)
        {
            return (x) =>
            {
                double v = 0.0f;

                for (int i = 0; i < coefficients.RowCount; i++)
                {
                    v += coefficients.At(i, 0) * Math.Pow(x, i);
                }

                return v;
            };
        }

        /// <summary>
        /// Computes the coefficients for an n-degree polynomial such that the least squares error is minimal.
        /// </summary>
        /// <param name="ts">The t-values (x-axis).</param>
        /// <param name="ys">The y-values (y-axis).</param>
        /// <param name="n">The degree of the polynomial to fit.</param>
        /// <returns>A (n+1, 1) matrix of coefficients, sorted from lowest degree to highest degree.</returns>
        public static Matrix<double> FitPolynomialCoefficients(IReadOnlyCollection<double> ts, IReadOnlyCollection<double> ys, int n)
        {
            if (n <= 0)
                throw new ArgumentException("degree needs to be 1 or higher.", "degree");

            if (ts.Count != ys.Count)
                throw new ArgumentException("ts and ys need to be of equal length.");

            // Allocate matrices.
            var m = n + 1;
            var A = Matrix<double>.Build.Dense(m, m);
            var B = Matrix<double>.Build.Dense(m, 1);


            // Generate values for A.
            List<double> l = new List<double>();
            for (int i = 0; i < (n * 2) + 1; i++)
            {
                var v = MathTools.Sum(MathTools.Pow(ts, i));
                l.Add(v);
            }

            // Fill A.
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    // A_ij = \sum{m=i}^n t_m^{i+j}
                    int idx = i + j;
                    var v = l[idx];
                    A.At(i, j, v);
                }
            }

            // Fill B.
            for (int i = 0; i < m; i++)
            {
                // B_i = \sum_{j=1}^n y_j \cdot t_j^i
                var temp = MathTools.Pow(ts, i);
                B.At(i, 0, MathTools.Sum(MathTools.Mul(ys, temp)));
            }

            var InvA = A.Inverse();

            // X = A^-1 * B.
            return InvA * B;
        }

        [Obsolete]
        public static Matrix<double> FitQuadraticCoefficients(IReadOnlyCollection<double> t, IReadOnlyCollection<double> y)
        {
            return FitPolynomialCoefficients(t, y, 2);
        }

        [Obsolete]
        public static Matrix<double> FitLinearLinear(IReadOnlyCollection<double> x, IReadOnlyCollection<double> y)
        {
            return FitPolynomialCoefficients(x, y, 1);
        }
    }
}

