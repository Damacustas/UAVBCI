package com.DroneSimulator;

import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.Dictionary;
import java.util.Hashtable;

import org.apache.commons.cli.BasicParser;
import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.GnuParser;
import org.apache.commons.cli.HelpFormatter;
import org.apache.commons.cli.Options;
import org.apache.commons.cli.ParseException;
import org.apache.commons.cli.PosixParser;

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
		
		if(arguments.hasOption("help"))
		{
			HelpFormatter formatter = new HelpFormatter();
			formatter.printHelp("DroneSimulator", options);
		}

		// Create some objects, set some values.

		// Create and show window.
		new Simulation();
	}

	private static CommandLine parseArguments(String[] args)
	{
		try
		{
			options = new Options();
			options.addOption("duration", true, "The duration of a trial in seconds.");
			options.addOption("devianceX", true, "The maximum deviance from the x-axis.");
			options.addOption("devianceXy", true, "The maximum deviance from the y-axis.");
			options.addOption("initialWidth", true, "The initial width of the target.");
			options.addOption("initialHeight", true, "The initial height of the target.");
			options.addOption("velocity", true, "The velocity of the simulated drone.");
			options.addOption("help", false, "Shows this help");
			
			return new GnuParser().parse(options, args);
		}
		catch(ParseException ex)
		{
			System.out.println(ex.getMessage());
			return null;
		}
	}
	
	private static Options options;
}
