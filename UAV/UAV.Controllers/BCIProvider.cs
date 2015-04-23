using System;
using UAV.Common;
using FieldTrip.Buffer;

namespace UAV.Controllers
{
    public class BCIProvider : ICommandProvider
    {
        public BCIProvider()
        {
        }

        #region ICommandProvider implementation

        public void Initialize()
        {

        }

        public event EventHandler<CommandEventArgs> CommandReceived;

        public Vector2D LastCommand { get; private set; }

        #endregion
    }
}

