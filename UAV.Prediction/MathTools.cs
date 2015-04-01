using System;
using System.Linq;
using System.Collections.Generic;

namespace UAV.Prediction
{
	public static class MathTools
	{
		public static ICollection<double> Mul(ICollection<double> a, ICollection<double> b)
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

		public static ICollection<double> Pow(IEnumerable<double> a, double power)
		{
			return (from x in a
			        select Math.Pow(x, power)).ToList();
		}
	}
}

