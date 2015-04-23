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
                    Console.Write("Connecting to 'localhost:1972...");
                    clock.connect("localhost", 1972);
                    Console.WriteLine(" done");

                    if(clock.isConnected())
                    {
                        hdr = clock.getHeader();
                        lastEvent = hdr.nEvents;
                    }
                }
                catch
                {
                    hdr = null;
                }

                if(hdr == null)
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

        /// <summary>
        /// Handles receiving data from blackboard.
        /// </summary>
        private void RecvThread()
        {
            var sec = clock.waitForEvents(lastEvent, 5000);
            if (sec.nEvents > lastEvent)
            {
                BufferEvent[] events = clock.getEvents(lastEvent, sec.nEvents - 1);

                foreach (var evt in events)
                {
                    Console.WriteLine(evt);

                    // TODO: Process events of interest.
                }
            }
            else
            {
                Console.WriteLine("Timeout while waiting for events.");
            }
        }
    }
}

