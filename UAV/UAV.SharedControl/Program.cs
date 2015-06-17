using System;
using System.Linq;
using FieldTrip.Buffer;
using System.Threading;
using System.Collections.Generic;
using UAV.Prediction;

namespace UAV.SharedControll
{
    class MainClass
    {
        static BufferClientClock bci_client;
        static int lastEvent;
        static bool running;

        static Queue<Tuple<int, double>> datapoints = new Queue<Tuple<int, double>>();
        static int dataN = 0;

        static bool isLSF, isABA;
        static double targetHeight;
        static double currentHeight;

        public static void Main(string[] args)
        {
            if (args[1] == "--LSF")
            {
                isLSF = true;
            }
            else if (args[1] == "--ABA")
            {
                isABA = true;

                targetHeight = double.Parse(args[2]);
            }
            else
            {
                Console.WriteLine("Unknown assistance method: {0}.", args[1]);
            }

            ConnectBufferBCI(args[0]);

            while (running)
            {
                var sec = bci_client.WaitForEvents(lastEvent, 5000);
                if (sec.NumEvents > lastEvent)
                {
                    var events = bci_client.GetEvents(lastEvent, sec.NumEvents - 1);
                    lastEvent = sec.NumEvents;

                    foreach (var evt in events)
                    {
                        if (isLSF && evt.Type.ToString() == "classifier.prediction")
                        {
                            ApplyLSF(evt.Value.ToString());
                        }
                        else if (isABA)
                        {
                            if (evt.Type.ToString() == "classifier.prediction")
                            {
                                ApplyABA(evt.Value.ToString());
                            }
                            else if (evt.Type.ToString() == "drone.altitude")
                            {
                                UpdateABA(evt.Value.ToString());
                            }
                        }
                    }
                }
            }
        }

        static void ApplyABA(string predVal)
        {
            var needGoUp = currentHeight > targetHeight; // Should the drone go upwards?
            var goingUp = predVal > 0; // Is the drone going upwards?

            if (needGoUp && goingUp)
            {
                bci_client.PutEvent(new BufferEvent("shrdcontrol.prediction", 2, -1));
            }
            else if (!needGoUp && !goingUp)
            {
                bci_client.PutEvent(new BufferEvent("shrdcontrol.prediction", -2, -1));
            }
            else if (needGoUp && !goingUp)
            {
                // What to do when need to go up, but going down?
                //bci_client.PutEvent(new BufferEvent("shrdcontrol.prediction", 0, -1));
            }
            else if (!needGoUp && goingUp)
            {
                // What if need to go down, but going up?
                //bci_client.PutEvent(new BufferEvent("shrdcontrol.prediction", 0, -1));
            }
        }

        static void UpdateABA(string altitude)
        {
            currentHeight = altitude;
        }

        static void ApplyLSF(string predVal)
        {
            datapoints.Enqueue(new Tuple<int, double>(dataN, double.Parse(predVal)));
            if (datapoints.Count > 20)
            {
                datapoints.Dequeue();
            }

            if (datapoints.Count >= 2)
            {
                var func = Fitters.GeneratePolynomialFit(
                               datapoints.Select(dp => (double)dp.Item1).ToArray(),
                               datapoints.Select(dp => dp.Item2).ToArray(),
                               1
                           );

                var pred = func(dataN++);

                bci_client.PutEvent(new BufferEvent("shrdcontrol.prediction", pred, -1));
            }
        }

        static void ConnectBufferBCI(string hostname)
        {
            bci_client = new BufferClientClock();
            Header hdr = null;

            while (hdr == null)
            {
                try
                {
                    Console.Write("Connecting to '" + hostname + "'...");
                    bci_client.Connect(hostname, 1972);
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
