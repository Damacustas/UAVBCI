using System;

namespace UAV.Controllers
{
	public class Program
	{
		public static void Main(string[] args)
		{
			if (args[0] == "--shared")
			{
				// TODO: Implement.
			}
			else if (args[0] == "--unshared")
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

