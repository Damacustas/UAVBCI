using System;
using UAV.Common;

namespace UAV.Simulation
{
	public abstract class AbstractDirectionModifier
	{
		protected AbstractDirectionModifier Parent { get; private set; }

		protected AbstractDirectionModifier(AbstractDirectionModifier parent)
		{
			this.Parent = parent;
		}

		public abstract Vector2D ComputeNewDirection(WorldState state);
	}
}

