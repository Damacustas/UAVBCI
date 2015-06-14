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
        static DroneClient drone;
        static List<string> args;

        public static void Main(string[] rawargs)
        {
            args = new List<string>(rawargs);
            Console.WriteLine("Started with args: " + string.Join(" ", rawargs));
			
            if (args.Contains("--no-control"))
            {
                ConnectDrone();

                while (true)
                    Thread.Yield();
            }
            else if (args.Contains("--unshared"))
            {
                ConnectDrone();

                if (args.Contains("--joystick"))
                {
                    FlyDroneJoystick();
                }
                else if (args.Contains("--buffer-joystick"))
                {
                    FlyDroneBufferJoystick();
                }
                else if (args.Contains("--bci"))
                {
                    FlyDroneBuffer("classifier.prediction");
                }
                else
                {
                    Console.WriteLine("Unknown input source.");
                }
            }
            else if (args.Contains("--shared"))
            {
                ConnectDrone();
                FlyDroneBuffer("shrdcontrol.prediction");
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


        static void ConnectDrone()
        {
            Console.WriteLine("Connecting to drone... ");
            drone = new DroneClient();
            drone.Start();
            Thread.Sleep(2000);
            Console.WriteLine("Done connecting to drone...");
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

        static void FlyDroneJoystick()
        {
            var joystick = new JoystickDevice();
            joystick.Initialize("/dev/input/js0");

            while (true)
            {
                joystick.ProcessEvents();
                var pitch = -joystick.AxisValues[0];
                var roll = joystick.AxisValues[1];
                var gaz = joystick.AxisValues[2];
                var yaw = joystick.AxisValues[3];

                drone.Progress(FlightMode.Progressive, roll, pitch, yaw, gaz);
                Thread.Sleep(200);
            }
        }

        static void FlyDroneBuffer(string heightEvtType)
        {
            ConnectBufferBCI();
            bool flying = false;
            bool running = true;
            float targetHeight = 1, targetVelocity = 2;
            float maxHeight = 3, minHeight = 0.1f;

            float gaz, roll, pitch, yaw;
            gaz = roll = pitch = yaw = 0;

            // Thread to read events from the buffer.
            new Thread(
                () =>
                {
                    while (running)
                    {
                        var sec = bci_client.WaitForEvents(lastEvent, 5000);
                        if (sec.NumEvents > lastEvent)
                        {
                            var events = bci_client.GetEvents(lastEvent, sec.NumEvents - 1);
                            lastEvent = sec.NumEvents;

                            foreach (var evt in events)
                            {
                                string evttype = evt.Type.ToString();
                                Console.WriteLine("{0}: {1}", evttype, evt.Value);

                                // BCI event type.
                                if (evttype == heightEvtType)
                                {
                                    var val = double.Parse(evt.Value.ToString());

                                    if (val > 0 && targetHeight < maxHeight)
                                    {
                                        targetHeight += 0.1f;
                                    }
                                    else if (val < 0 && targetHeight < minHeight)
                                    {
                                        targetHeight -= 0.1f;
                                    }
                                }

                                // Joystick
                                else if (evttype == "joystick")
                                {
                                    var raw_val = evt.Value.ToString();

                                    if (raw_val == "Button0")
                                    {
                                        if (flying)
                                        {
                                            drone.Land();
                                        }
                                        else
                                        {
                                            drone.Takeoff();
                                        }

                                        flying = !flying;
                                    }
                                }

                                // exit
                                else if (evttype == "exit")
                                {
                                    running = false;
                                }
                            }
                        }
                    }
                }
            ).Start();

            // Event handler (probably called from another thread) to receive navigation data from the drone.
            drone.NavigationDataAcquired +=
                navData =>
            {
                // Correction for height.
                if (navData.Altitude < targetHeight - 0.1)
                {
                    gaz = Math.Min(targetHeight - drone.NavigationData.Altitude, 1.0f);
                }
                else if (navData.Altitude > targetHeight + 0.1)
                {
                    gaz = -Math.Min(drone.NavigationData.Altitude - targetHeight, 1.0f);
                }
                else
                {
                    gaz = 0;
                }

                // Correction for velocity.
                if (navData.Velocity.X > targetVelocity + 0.5)
                {
                    pitch = navData.Pitch - 0.025f;
                }
                else if (navData.Velocity.X < targetHeight - 0.5)
                {
                    pitch = navData.Pitch + 0.025f;
                }
                else
                {
                    pitch = navData.Pitch;
                }
            };

            // Infinite loop to send data to the drone.
            while (running)
            {
                if (flying)
                {
                    drone.Progress(FlightMode.Hover, roll, pitch, yaw, gaz);
                }

                Thread.Sleep(250);
            }

            // Land when done, if required.
            if (flying)
                drone.Land();
        }

        static void FlyDroneBufferJoystick()
        {
            ConnectBufferBCI();
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

                        Console.WriteLine("{0}: {1}", evttype, evt.Value);

                        if (evttype == "Joystick")
                        {
                            if (evt.Value.ToString() == "Button0")
                            {
                                if (flying)
                                {
                                    drone.Land();
                                }
                                else
                                {
                                    drone.Takeoff();
                                }

                                flying = !flying;
                            }
                            else
                            {
                                double val = double.Parse(evt.Value.ToString());
                                drone.Progress(FlightMode.Progressive, pitch: (float)val * 0.3f);
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Timeout while waiting for events.");
                }
            }
        }
    }
}

