using System;
using System.Threading;
using AR.Drone.Client;
using System.Collections.Generic;
using AR.Drone.Data;
using FieldTrip.Buffer;
using AR.Drone.Client.Command;

namespace UAV.Controllers
{
    public class Program
    {
        BufferClientClock bci_client;

        public static void Main(string[] rawargs)
        {
            var args = new List<string>(rawargs);

            Console.WriteLine("Started with args: " + string.Join(" ", rawargs));
			
            if (args.Contains("--no-control"))
            {
                Console.Write("Connecting to drone... ");
                DroneClient drone = new DroneClient();
                drone.Start();
                Thread.Sleep(2000);
                Console.WriteLine("done.");

                if (args.Contains("--enable-video"))
                {
                    StartVideo(drone);
                }

                while (true)
                    Thread.Yield();
            }
            else if (args.Contains("--unshared"))
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

                ConnectBufferBCI();

                int lastEvent = 0;
                bool flying = false;

                while (true)
                {
                    var sec = bci_client.WaitForEvents(lastEvent, 5000);
                    if (sec.NumEvents > lastEvent)
                    {
                        BufferEvent[] events = bci_client.GetEvents(lastEvent, sec.NumEvents - 1);

                        lastEvent = sec.NumEvents;

                        foreach (var evt in events)
                        {
                            string evttype = evt.Type.ToString();

                            if (evttype == "Joystick")
                            {
                                if (evt.Value.ToString() == "Button0")
                                {
                                    if (flying)
                                        drone.Land();
                                    else
                                        drone.Takeoff();
                                    flying = !flying;
                                }
                                else
                                {
                                    double val = double.Parse(evt.Value.ToString());
                                    drone.Progress(FlightMode.Progressive, pitch: val * 0.3);
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine("Timeout while waiting for events.");
                    }
                }

//                ICommandProvider provider = null;
//                if (args.Contains("--joystick"))
//                {
//                }
//                else if (args.Contains("--bci"))
//                {
//                    provider = new BCIProvider();
//                }
//                else
//                {
//                    Console.WriteLine("Did not specify command source.");
//                    return;
//                }
//
//                var controller = new PasstroughController(provider);
//                controller.Drone = drone;
//                controller.StartController();
//                while (true)
//                    Thread.Yield();
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
            Console.Write("Starting video...");
            var sender = new VideoPacketSender();
            sender.Start();
            Console.WriteLine("done.");

            Console.Write("Connecting VideoPacketAcquired event...");
            client.VideoPacketAcquired += sender.EnqueuePacket;
            Console.WriteLine("done.");
        }

        static void ConnectBufferBCI()
        {
            bci_client = new BufferClientClock();
            Header hdr = null;

            while (hdr == null)
            {
                try
                {
                    Console.Write("Connecting to 'localhost:1972'...");
                    bci_client.Connect("localhost", 1972);
                    Console.WriteLine(" done");

                    if (bci_client.IsConnected)
                    {
                        Console.WriteLine("GETHEADER");
                        hdr = bci_client.GetHeader();
                        lastEvent = hdr.NumEvents;
                    }
                    else
                    {
                        Console.WriteLine("Not connected!");
                    }
                }
                catch
                {
                    hdr = null;
                }

                if (hdr == null)
                {
                    Console.WriteLine("Couldn't read header. Waiting.");
                    Thread.Sleep(5000);
                }
            }
        }
    }
}

