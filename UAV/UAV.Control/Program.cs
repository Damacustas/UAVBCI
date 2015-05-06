using AR.Drone.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UAV.Joystick;
using AR.Drone.Data.Navigation;

namespace UAV
{
    class Program
    {

        static void Main(string[] args)
        {
            DroneClient drone = new DroneClient();

            drone.NavigationDataAcquired += HandleNavigationDataAcquired;

            JoystickFlightController controller = new JoystickFlightController();
            controller.Client = drone;
            drone.Start();
            drone.FlatTrim();
            controller.Start();
        }

        static void HandleNavigationDataAcquired(NavigationData data)
        {
            if (data.Velocity.X > 0.4)
                Console.WriteLine("Slow down!");
        }

    }
}
