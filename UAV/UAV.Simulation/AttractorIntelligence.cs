using System;
using UAV.Common;

namespace UAV.Simulation
{
    /// <summary>
    /// The Attractor Intelligence.
    /// 
    /// Implemented somewhat simplistic since the simulations do not have multiple different targtes.
    /// </summary>
    public class AttractorIntelligence : IIntelligence
    {
        #region IIntelligence implementation

        public Vector2D ComputeCommand(WorldState worldState, Vector2D baseCommand)
        {
            return (worldState.CurrentTarget - worldState.DroneLocation).GetNormalized();
        }

        public string IntelligenceName
        {
            get
            {
                return "AttractorIntelligence";
            }
        }

        #endregion
    }
}

