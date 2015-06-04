using System;
using UAV.Joystick;
using UAV.Common;
using System.Threading;

namespace UAV.Controllers
{
    public class JoystickController : ICommandProvider
    {
        private JoystickDevice joystick;

        public JoystickController()
        {
        }

        #region ICommandProvider implementation

        public event EventHandler<CommandEventArgs> CommandReceived;

        public void Initialize()
        {
            joystick = new JoystickDevice();
            joystick.Initialize("/dev/input/js0");
            joystick.InputReceived +=
                (sender, e) =>
            {
                //LastCommand = new Vector2D(e.Axis[(int)0], e.Axis[(int)1]);

                if (CommandReceived != null)
                {
                    CommandReceived(this, new CommandEventArgs{ Command = LastCommand });
                }
            };

            new Thread(() =>
                {
                    joystick.ProcessEvents();
                }).Start();
        }

        public Vector2D LastCommand
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion
    }
}

