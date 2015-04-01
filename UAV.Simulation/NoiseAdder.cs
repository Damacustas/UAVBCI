using System;

namespace UAV.Simulation
{
	public class NoiseAdder : AbstractDirectionModifier
	{
		public NoiseAdder(AbstractDirectionModifier parent, double noiseRange) : base(parent)
		{
		}

		#region implemented abstract members of AbstractDirectionModifier
		public override Vector2D ComputeNewDirection (WorldState state)
		{
			throw new NotImplementedException ();
		}
		#endregion
	}
}

