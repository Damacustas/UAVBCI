using System;

using UAV.Common;

namespace UAV.Simulation
{
    /// <summary>
    /// An intelligence that has no intelligence at all and just says what the input says.
    /// </summary>
    public class NoIngelligence : IIntelligence
    {
        public NoIngelligence()
        {
        }

        #region IIntelligence implementation

        public string IntelligenceName
        {
            get
            {
                return "NoIntelligence";
            }
        }

        public Vector2D ComputeCommand(WorldState worldState, Vector2D baseCommand)
        {
            return baseCommand;
        }

        #endregion
    }
}

