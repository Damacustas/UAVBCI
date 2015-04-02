using System;
using UAV;

namespace UAV.Controllers
{
	public interface ICommandProvider
	{
		event EventHandler<CommandEventArgs> CommandReceived;
		Vector2D LastCommand { get; }
	}
}

