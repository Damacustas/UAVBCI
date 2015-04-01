using System;

namespace UAV.Controllers
{
	public interface ICommandProvider
	{
		event EventHandler<CommandEventArgs> CommandReceived;
		object LastCommand { get; }
	}
}

