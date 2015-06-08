using System;
using UAV.Common;
using FieldTrip.Buffer;
using System.Threading;

namespace UAV.Controllers
{
    public class BCIProvider : ICommandProvider
    {
        private BufferClientClock clock;
        private int lastEvent;

        #region ICommandProvider implementation

        /// <summary>
        /// Initializes the connection to the buffer_bci framework.
        /// </summary>
        public void Initialize()
        {
            clock = new BufferClientClock();
            Header hdr = null;

            while (hdr == null)
            {
                try
                {
                    Console.Write("Connecting to 'localhost:1972'...");
                    clock.Connect("localhost", 1972);
                    Console.WriteLine(" done");

                    if (clock.IsConnected)
                    {
                        Console.WriteLine("GETHEADER");
                        hdr = clock.GetHeader();
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

            new Thread(RecvThread).Start();
        }

        public event EventHandler<CommandEventArgs> CommandReceived;

        public Vector2D LastCommand { get; private set; }

        #endregion

        private void OnCommandReceived(Vector2D cmd)
        {
            if (CommandReceived != null)
            {
                var eventArgs = new CommandEventArgs();
                eventArgs.Command = cmd;
                CommandReceived(this, eventArgs);
            }
        }

        /// <summary>
        /// Handles receiving data from blackboard.
        /// </summary>
        private void RecvThread()
        {
            while (true)
            {
                var sec = clock.WaitForEvents(lastEvent, 5000);
                if (sec.NumEvents > lastEvent)
                {
                    BufferEvent[] events = clock.GetEvents(lastEvent, sec.NumEvents - 1);

                    lastEvent = sec.NumEvents;

                    foreach (var evt in events)
                    {
                        string evttype = evt.Type.ToString();
                        if (evttype == "classifier.prediction")
                        {
                            try
                            {
                                double val = double.Parse(evt.Value.ToString());

                                Console.WriteLine(">> " + val);

                                if (val > 0)
                                {
                                    OnCommandReceived(new Vector2D(0, 0.2));
                                }
                                else
                                {
                                    OnCommandReceived(new Vector2D(0, -0.2));
                                }
                            }
                            catch (Exception)
                            {
                                Console.WriteLine("Couldn't convert '{0}' to double.", evt.Value);
                            }
                        }
                        else
                        {
                            Console.WriteLine("   " + evt);
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

