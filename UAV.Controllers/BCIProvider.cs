using System;

namespace UAV.Controllers
{
    public class BCIProvider : ICommandProvider
    {
        public BCIProvider()
        {
        }

        #region ICommandProvider implementation

        public event EventHandler<CommandEventArgs> CommandReceived;

        public object LastCommand { get; }

        #endregion
    }
}

