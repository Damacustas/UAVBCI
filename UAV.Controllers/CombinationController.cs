using System;
using System.Collections.Generic;

namespace UAV.Controllers
{
	public class CombinationController : AbstractController
	{
		public Dictionary<ICommandProvider, float> CommandProviders { get; set; }

		public CombinationController ()
		{
		}

		public ICommandProvider MasterCommandProvider {
			set {
				value.CommandReceived += HandleCommandReceived;
			}
		}

		void HandleCommandReceived (object sender, CommandEventArgs e)
		{
			float powerSum = 0.0f;
			float gaz = 0.0f;
			float roll = 0.0f; 

			foreach (var kvp in CommandProviders)
			{
				powerSum += kvp.Value;
				//gaz += kvp.Key.LastCommand.Gaz;
				//roll += kvp.Key.LastCommand.Roll;
			}

			gaz /= powerSum;
			roll /= powerSum;

			SendFlightCommand (
				roll: roll,
				gaz: gaz
			);
		}
	}
}

