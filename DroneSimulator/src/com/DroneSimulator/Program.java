package com.DroneSimulator;

import java.io.File;
import java.io.FileReader;
import java.io.IOException;
import java.util.Dictionary;
import java.util.Hashtable;

public class Program
{
	/**
	 * The entry point of the application.
	 * @param args
	 */
	public static void main(String[] args)
	{
		Dictionary<String, String> arguments = parseArguments(args);
		if(arguments == null)
		{
			return;
		}
		
		// Create some objects, set some values.
		
		// Create and show window.
	}

	/**
	 * Parses arguments from the commandline, or from a configuration file.
	 * @param raw The raw commandline arguments.
	 * @return A dictionary mapping names to values.
	 */
	private static Dictionary<String, String> parseArguments(String[] raw)
	{
		if (raw.length == 0)
		{
			return new Hashtable<String, String>();
		}
		
		// Only support paired arguments.
		if(raw.length % 2 == 1)
		{
			if(raw.length == 1)
			{
				if(raw[0] == "--help")
				{
					printHelp();
					return null;
				}
				else
				{
					printInvalidArguments();
					return null;
				}
			}
			else
			{
				printInvalidArguments();
				return null;
			}
		}
		
		// Check if config-file was given.
		if(raw[0] == "--config")
		{
			return readArgumentsFromConfig(raw[1]);
		}
		
		// Add arguments to dict.
		Dictionary<String, String> args = new Hashtable<String, String>();
		for(int i = 0; i < raw.length; i += 2)
		{
			String name = raw[i];
			String value = raw[i+1];
			
			args.put(name, value);
		}
		
		return args;
	}
	
	/**
	 * Reads commandline arguments from a file.
	 * @param filename The file containing the arguments.
	 * @return A dictionary mapping names to values.
	 */
	private static Dictionary<String, String> readArgumentsFromConfig(String filename)
	{
		File path = new File(filename);
		if(!path.exists())
		{
			System.out.println("Could not open the file given after the --config option.");
			return null;
		}
		else
		{
			try
			{
				String contents = readFileToEnd(path.getPath());
				contents.replace("\n", ""); // Remove newlines
				contents.replace("\r", "");
				
				String[] params = contents.split(" ");
				return parseArguments(params);
			}
			catch (IOException e)
			{
				System.out.println("Could not read file given with the --config options.");
				return null;
			}
		}
	}
	
	/**
	 * Reads an entire text file into memory.
	 * @param filename The name of the file.
	 * @return The contents of the file.
	 * @throws IOException Thrown if an exception was encountered during reading.
	 */
	private static String readFileToEnd(String filename) throws IOException {
        FileReader in = new FileReader(filename);
        StringBuilder contents = new StringBuilder();
        char[] buffer = new char[4096];
        int read = 0;
        do {
            contents.append(buffer, 0, read);
            read = in.read(buffer);
        } while (read >= 0);
        in.close();
        return contents.toString();
    }

	/**
	 * Informs the user invalid arguments were provided.
	 */
	private static void printInvalidArguments()
	{
		System.out.println("Received invalid arguments, please check your commandline parameters.\nRun with --help for help.");
	}

	/**
	 * Prints help.
	 */
	private static void printHelp() 
	{
		System.out.println("Help:\n- Todo: finish.");
	}
}
