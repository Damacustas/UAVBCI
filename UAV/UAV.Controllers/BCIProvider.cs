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
                    clock.connect("localhost", 1972);
                    Console.WriteLine(" done");

                    if(clock.isConnected())
                    {
                        Console.WriteLine("GETHEADER");
                        hdr = clock.getHeader();
                        lastEvent = hdr.nEvents;
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
				var sec = clock.waitForEvents(lastEvent, 5000);
				if (sec.nEvents > lastEvent)
				{
					BufferEvent[] events = clock.getEvents(lastEvent, sec.nEvents - 1);

					foreach (var evt in events)
					{

						string evttype = evt.getType().toString();
						if (evttype == "keyboard")
						{
							string val = evt.getValue().toString();

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

