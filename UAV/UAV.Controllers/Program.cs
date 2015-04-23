using System;
using System.Threading;

namespace UAV.Controllers
{
	public class Program
	{
		public static void Main(string[] args)
		{
			
            if (args.Length == 0 || args[0] == "--unshared")
			{
                BCIProvider provider = new BCIProvider();
                PasstroughController controller = new PasstroughController(provider);
                controller.StartController();
                while (true)
                    Thread.Yield();
			}
            else if (args[0] == "--shared")
            {
                // TODO: Implement.
            }
			else if (args[0] == "--help")
			{
				Console.WriteLine("Options:");
				Console.WriteLine("\t--help\tShows this.");
				Console.WriteLine("\t--shared\tPuts drone under shared control.");
				Console.WriteLine("\t--unshared\tPuts drone under direct control.");
			}
			else
			{
				Console.WriteLine("Run with --help for options.");
			}
		}
	}
}

