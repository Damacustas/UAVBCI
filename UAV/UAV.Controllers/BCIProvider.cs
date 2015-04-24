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
                if (sec.nEvents > lastEvent)
                {
                    BufferEvent[] events = clock.GetEvents(lastEvent, sec.nEvents - 1);

                    lastEvent = sec.nEvents;

                    foreach (var evt in events)
                    {
                        string evttype = evt.Type.ToString();
                        if (evttype == "keyboard")
                        {
                            string val = evt.Value.ToString();

                            switch (val)
                            {
                                case "w":
                                    OnCommandReceived(new Vector2D(0, 1));
                                    break;

                                case "a":
                                    OnCommandReceived(new Vector2D(-1, 0));
                                    break;

                                case "s":
                                    OnCommandReceived(new Vector2D(0, -1));
                                    break;

                                case "d":
                                    OnCommandReceived(new Vector2D(1, 0));
                                    break;

                            }

                            Console.WriteLine(">> " + evt);
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

