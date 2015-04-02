using AR.Drone.Client;
using AR.Drone.Client.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace UAV
{
    public class CompositeFlightController
    {
        private Thread controllerThread;

        public List<IFlightBehavior> FlightBehaviors { get; private set; }

        public event Action ControlCycleStarting;

        public DroneClient Drone { get; set; }

        public CompositeFlightController()
        {
            FlightBehaviors = new List<IFlightBehavior>();
        }

        public void Start()
        {
            controllerThread = new Thread(ControllerMainLoop);
            controllerThread.Start();
        }

        private void ControllerMainLoop()
		{
			float lastPitch, lastRoll, lastYaw, lastGaz;
			lastPitch = lastRoll = lastYaw = lastGaz = 0;

            while (true)
            {
                if(ControlCycleStarting != null)
                {
                    ControlCycleStarting();
                }

                float pitch, roll, yaw, gaz;
                pitch = roll = yaw = gaz = 0;

                foreach (var behavior in FlightBehaviors)
                {
                    var result = behavior.ComputeBehavior(Drone);
                    pitch += result.Pitch;
                    yaw += result.Yaw;
                    roll += result.Roll;
                    gaz += result.Gaz;
                }

				//Console.WriteLine("cmd: pitch={0}, roll={1}, yaw={2}, gaz={3}", pitch, roll, yaw, gaz);

				/*
				if (roll == lastRoll ||
					gaz == lastGaz ||
					yaw == lastYaw ||
					pitch == lastPitch)
				{
					// Do nothing.
				}
				else
				{
				*/
					Drone.Progress(
						FlightMode.Progressive,
						roll: roll,
						pitch: pitch,
						yaw: yaw,
						gaz: gaz);

					roll = lastRoll;
					gaz = lastGaz;
					yaw = lastYaw;
					pitch = lastPitch;
				//}
                Thread.Sleep(50);
            }
        }
    }
}
