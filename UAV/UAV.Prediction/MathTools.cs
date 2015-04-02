using System;
using System.Linq;
using System.Collections.Generic;

namespace UAV.Prediction
{
	public static class MathTools
	{
        public static IReadOnlyCollection<double> Mul(IReadOnlyCollection<double> a, IReadOnlyCollection<double> b)
		{
			List<double> s = new List<double>();

			if (a.Count != b.Count)
			{
				throw new ArgumentException("Collections a and b are not of equal length.");
			}

			var enumA = a.GetEnumerator();
			var enumB = b.GetEnumerator();

			while (enumA.MoveNext())
			{
				enumB.MoveNext();

				s.Add(enumA.Current * enumB.Current);
			}

			return s;
		}

		public static double Sum(IEnumerable<double> l)
		{
			return l.Aggregate((a,b) => a + b);
		}

		public static IReadOnlyCollection<double> Pow(IEnumerable<double> a, double power)
		{
			return (from x in a
			        select Math.Pow(x, power)).ToList();
		}
	}
}

