using System;
using System.Threading;
using AR.Drone.Client;
using System.Collections.Generic;
using AR.Drone.Data;

namespace UAV.Controllers
{
    public class Program
    {
        public static void Main(string[] rawargs)
        {
            var args = new List<string>(rawargs);
			
            if (args.Contains("--unshared"))
            {
                Console.Write("Connecting to drone... ");
                DroneClient drone = new DroneClient();
                drone.Start();
                Thread.Sleep(2000);
                drone.FlatTrim();
                Console.WriteLine("done.");

                if (args.Contains("--enable-video"))
                {
                    StartVideo(drone);
                }

                var provider = new BCIProvider();
                var controller = new PasstroughController(provider);
                controller.Drone = drone;
                controller.StartController();
                while (true)
                    Thread.Yield();
            }
            else if (args.Contains("--shared"))
            {
                // TODO: Implement.

//                if (args.Contains("--enable-video"))
//                {
//                    StartVideo(drone);
//                }
            }
            else if (args.Contains("--help"))
            {
                Console.WriteLine("Options:");
                Console.WriteLine("\t--help\tShows this.");
                Console.WriteLine("\t--shared\tPuts drone under shared control.");
                Console.WriteLine("\t--unshared\tPuts drone under direct control.");
            }
            else
            {
                Console.WriteLine("Run with --help for options.");
            }
        }

        static void StartVideo(DroneClient client)
        {
            var sender = new VideoPacketSender();
            sender.Start();

            client.VideoPacketAcquired += sender.EnqueuePacket;
        }
    }
}

