using AR.Drone.Client;
using System;

namespace UAV
{
    public interface IFlightBehavior
    {
        // Pitch, Yaw, Roll, Gaz.
        MovementCommand ComputeBehavior(DroneClient drone);
    }
}