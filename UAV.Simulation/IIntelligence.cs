using System;
using UAV.Common;

namespace UAV.Simulation
{
    public interface IIntelligence
    {
        Vector2D ComputeCommand(WorldState worldState, Vector2D baseCommand);
    }
}

