using System;
using UAV;
using UAV.Common;

namespace UAV.Controllers
{
	public interface ICommandProvider
	{
		event EventHandler<CommandEventArgs> CommandReceived;
		Vector2D LastCommand { get; }
	}
}

