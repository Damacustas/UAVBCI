using System;

namespace UAV.Simulation
{
	public class BaseDirectionCreator : AbstractDirectionModifier
	{
		public Vector2D DirectionDeviation { get; private set; }

		public BaseDirectionCreator (Vector2D directionDeviation) : base(null)
		{
			this.DirectionDeviation = directionDeviation;
		}

		#region implemented abstract members of AbstractDirectionModifier

		public override Vector2D ComputeNewDirection (WorldState state)
		{
			Vector2D dir = state.CurrentTarget - state.DroneLocation;
			dir = dir.GetNormalized ();

			dir += DirectionDeviation;

			return dir;
		}

		#endregion
	}
}

