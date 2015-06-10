using System;
using System.Threading;
using AR.Drone.Client;
using System.Collections.Generic;
using AR.Drone.Data;
using FieldTrip.Buffer;
using AR.Drone.Client.Command;
using AR.Drone.Client.Configuration;
using UAV.Joystick;

namespace UAV.Controllers
{
    public class Program
    {
        static BufferClientClock bci_client;
        static int lastEvent = 0;
        static string lastMovement = "0";
        static DroneClient drone;
        static List<string> args;
        static bool flying;

        public static void Main(string[] rawargs)
        {
            args = new List<string>(rawargs);

            Console.WriteLine("Started with args: " + string.Join(" ", rawargs));
			
            if (args.Contains("--no-control"))
            {
                ConnectToDrone();

                while (true)
                    Thread.Yield();
            }
            else if (args.Contains("--unshared"))
            {
                ConnectToDrone();
                //ConnectBufferBCI();
                double targetHeight = 1.0f;

                JoystickDevice js = new JoystickDevice();
                js.Initialize("/dev/input/js0");
                js.InputReceived += (sender, e) =>
                {
                    if (e.IsButtonEvent && e.IsPressed)
                    {
                        if (e.Button == 0)
                        {
                            flying = false;
                        }
                        else if (e.Button == 9)
                        {
                            targetHeight += 0.1;
                        }
                        else if (e.Button == 10)
                        {
                            targetHeight -= 0.1;
                        }

                    }
                };

                double pitch = -0.1, gaz = 0.0, roll = 0.0;
                flying = true;
                drone.NavigationDataAcquired += data =>
                {
                    Console.WriteLine("X: {0}, Y: {1}, Z: {2}", data.Velocity.X.ToString("0.00").PadLeft(5), data.Velocity.Y.ToString("0.00").PadLeft(5), data.Velocity.Z.ToString("0.00").PadLeft(5));
                    if (data.Velocity.X > 1.05)
                    {
                        pitch = -Math.Abs(data.Pitch) * 1.10;
                        if (pitch > -0.1)
                        {
                            pitch = -0.1;
                        }
                    }
                    else if (data.Velocity.X < 0.95)
                    {
                        pitch = -Math.Abs(data.Pitch) * 1.10;
                        if (pitch > -0.1)
                        {
                            pitch = -0.1;
                        }
                    }

                    if (data.Altitude >= targetHeight + 0.15)
                    {
                        gaz = -Math.Min(drone.NavigationData.Altitude - targetHeight, 1.0f);
                    }
                    else if (data.Altitude <= targetHeight - 0.15)
                    {
                        gaz = Math.Min(targetHeight - drone.NavigationData.Altitude, 1.0f);
                    }

                    drone.Progress(FlightMode.Progressive, pitch: (float)pitch, gaz: (float)gaz);
                };

                drone.Takeoff();
                drone.Progress(FlightMode.Progressive, pitch: 0.2f);

                while (flying)
                {
                    js.ProcessEvents();
                }
                drone.Land();
                js.Deinitialize();
                drone.Stop();

//                flying = true;
//                var fly_thread = new Thread(
//                                     () =>
//                    {
//                        var velocityMaintainer = new VelocityMaintainer();
//                        velocityMaintainer.TargetVelocity = new Velocity{ Forward = -5.0f, Left = 0.0f, TurnLeft = 0.0f };
//                        velocityMaintainer.MaxDeviance = 1;
//                        drone.Takeoff();
//                        while (flying)
//                        {
//                            var cmd = velocityMaintainer.ComputeBehavior(drone);
//                            drone.Progress(FlightMode.Progressive, cmd.Roll, cmd.Pitch, cmd.Yaw, cmd.Gaz);
//                            Thread.Sleep(500);
//                        }
//                        drone.Land();
//                        Console.WriteLine("Done with flying.");
//                        return;
//                    }
//                                 );
//                fly_thread.Start();
//
//                while (flying)
//                    js.ProcessEvents();
//                fly_thread.Join();
//                js.Deinitialize();
//                drone.Stop();
//                Console.WriteLine("done.");

//
//                while (true)
//                {
//                    var sec = bci_client.WaitForEvents(lastEvent, 5000);
//                    if (sec.NumEvents > lastEvent)
//                    {
//                        BufferEvent[] events = bci_client.GetEvents(lastEvent, sec.NumEvents - 1);
//
//                        lastEvent = sec.NumEvents;
//
//                        foreach (var evt in events)
//                        {
//                            string evttype = evt.Type.ToString();
//
//                            Console.WriteLine("{0}: {1}", evttype, evt.Value);
//
//                            if (evttype == "classifier.prediction")
//                            {
//                                double val = double.Parse(evt.Value.ToString());
//                                if (val > 0)
//                                {
//                                    drone.Progress(FlightMode.Progressive, pitch: 0.1f);
//                                }
//                                else if (val < 0)
//                                {
//                                    drone.Progress(FlightMode.Progressive, pitch: -0.1f);
//                                }
//                            }
//                            if (evttype == "Joystick")
//                            {
//                                if (evt.Value.ToString() == "Button0")
//                                {
//                                    if (flying)
//                                    {
//                                        Console.WriteLine("landing...");
//                                        drone.Land();
//                                    
//                                    }
//                                    else
//                                    {
//                                        Console.WriteLine("taking off...");
//                                        drone.ResetEmergency();
//                                        drone.Takeoff();
//                                    }
//                                    
//                                    flying = !flying;
//                                }
////                                else
////                                {
////                                    if (evt.Value.ToString() == "0" && lastMovement != "0")
////                                    {
////                                        drone.Hover();
////                                    }
////                                    else
////                                    {
////                                        double val = double.Parse(evt.Value.ToString());
////                                        drone.Progress(FlightMode.Progressive, pitch: (float)val * 0.3f);
////                                    }
////
////                                    lastMovement = evt.Value.ToString();
////                                }
//                                                
//                            }
//                            else
//                            {
//                                Console.WriteLine("Timeout while waiting for events.");
//                            }
//                        }
//                    }
//
//                }
//
////                ICommandProvider provider = null;
////                if (args.Contains("--joystick"))
////                {
////                }
////                else if (args.Contains("--bci"))
////                {
////                    provider = new BCIProvider();
////                }
////                else
////                {
////                    Console.WriteLine("Did not specify command source.");
////                    return;
////                }
////
////                var controller = new PasstroughController(provider);
////                controller.Drone = drone;
////                controller.StartController();
////                while (true)
////                    Thread.Yield();
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

        static void ConnectToDrone()
        {
            Console.Write("Connecting to drone... ");
            drone = new DroneClient();
            drone.Start();
            Thread.Sleep(2000);
            Console.WriteLine("done.");
            drone.ResetEmergency();
            drone.FlatTrim();

            if (args.Contains("--enable-video"))
            {
                StartVideo(drone);
            }
        }

        static void ConnectBufferBCI()
        {
            bci_client = new BufferClientClock();
            Header hdr = null;

            while (hdr == null)
            {
                try
                {
                    Console.Write("Connecting to '192.168.0.109:1972'...");
                    bci_client.Connect("192.168.0.109", 1972);
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

