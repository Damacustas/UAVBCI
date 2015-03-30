package com.DroneSimulator;
import org.apache.commons.cli.CommandLine;
import org.apache.commons.cli.GnuParser;
import org.apache.commons.cli.HelpFormatter;
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
		
		if(arguments.hasOption("help"))
		{
			HelpFormatter formatter = new HelpFormatter();
			formatter.printHelp("DroneSimulator", options);
		}
		
		double duration, devianceX, devianceY, initialWidth, initialHeight;
		
		if(arguments.hasOption("duration"))
		{
			duration = Double.parseDouble(arguments.getOptionValue("duration"));
		}
		else
		{
			duration = 10.0;
		}
		
		if(arguments.hasOption("devianceX"))
		{
			devianceX = Double.parseDouble(arguments.getOptionValue("devianceX"));
		}
		else
		{
			devianceX = 50; // px.
		}
		
		if(arguments.hasOption("devianceY"))
		{
			devianceY = Double.parseDouble(arguments.getOptionValue("devianceY"));
		}
		else
		{
			devianceY = 50; // px.
		}
		
		if(arguments.hasOption("initialWidth"))
		{
			initialWidth = Double.parseDouble(arguments.getOptionValue("initialWidth"));
		}
		else
		{
			initialWidth = 50; // px.
		}
		
		if(arguments.hasOption("initialHeight"))
		{
			initialHeight = Double.parseDouble(arguments.getOptionValue("initialHeight"));
		}
		else
		{
			initialHeight = 50; //px.
		}

		// Create some objects, set some values.
		Simulation sim = new Simulation();
		sim.setDevianceX(devianceX);
		sim.setDevianceY(devianceY);
		sim.setDuration(duration);
		sim.setInitialTargetHeight(initialHeight);
		sim.setInitialTargetWidth(initialWidth);


		Screen scrn = new Screen(sim);
		scrn.setVisible(true);
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
