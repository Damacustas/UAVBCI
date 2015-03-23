package com.DroneSimulator;

import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.Dictionary;
import java.util.Hashtable;

import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.GnuParser;
import org.apache.commons.cli.Options;
import org.apache.commons.cli.ParseException;

public class Program {
	/**
	 * The entry point of the application.
	 * 
	 * @param args
	 */
	public static void main(String[] args) {
		CommandLine arguments = parseArguments(args);
		if (arguments == null) {
			return;
		}

		// Create some objects, set some values.

		// Create and show window.
	}

	private static CommandLine parseArguments(String[] args)
	{
		try
		{
			Options options = new Options();
			options.addOption("duration", true, "The duration of a trial in seconds.");
			options.addOption("deviance-x", true, "The maximum deviance from the x-axis.");
			options.addOption("deviance-y", true, "The maximum deviance from the y-axis.");
			options.addOption("initial-width", true, "The initial width of the target.");
			options.addOption("initial-height", true, "The initial height of the target.");
			options.addOption("velocity", true, "The velocity of the simulated drone.");
			
			return new GnuParser().parse(options, args);
		}
		catch(ParseException ex)
		{
			System.out.println(ex.getMessage());
			return null;
		}
	}
}
