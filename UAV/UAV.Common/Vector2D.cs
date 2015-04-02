using System;

namespace UAV.Common
{
	public class Vector2D
	{
		public double X { get; set; }
		public double Y { get; set; }

		public Vector2D (double x = 0, double y = 0)
		{
			this.X = x;
			this.Y = y;
		}

		public double Distance(Vector2D other)
		{
			return
				Math.Sqrt (
					Math.Pow (other.X - X, 2) + Math.Pow (other.Y - Y, 2)
				);
		}

		public static Vector2D operator +(Vector2D left, Vector2D right)
		{
			return new Vector2D(left.X + right.X, left.Y + right.Y);
		}

		public static Vector2D operator -(Vector2D left, Vector2D right)
		{
			return new Vector2D(left.X - right.X, left.Y - right.Y);
		}

		public Vector2D GetNormalized()
		{
			double len = Math.Sqrt (Math.Pow (X, 2) + Math.Pow (Y, 2));
			return new Vector2D (X / len, Y / len);
		}
	}
}

