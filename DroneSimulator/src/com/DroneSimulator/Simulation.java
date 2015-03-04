package com.DroneSimulator;

import java.util.Random;

public class Simulation
{
	// These represent the constant values of the simulation.
	private double velocity, duration;
	
	// These represent the maximum starting distance
	// of the simulated drone from the X/Y axis respectively.
	private double devianceX, devianceY;
	
	// These represent the initial dimensions of the target.
	private double initialTargetHeight, initialTargetWidth;
	
	// Used to generate the random start positions.
	private Random random;
	
	public Simulation()
	{
		random = new Random();
		velocity = duration = devianceX = devianceY = 0;
	}
	
	public SimulationRun generateNext(SimulationRun last)
	{
		double lastWidth, lastHeight;
		
		if(last != null)
		{
			lastWidth = last.getTargetWidth();
			lastHeight = last.getTargetHeight();
		}
		else
		{
			lastWidth = initialTargetWidth;
			lastHeight = initialTargetHeight;
		}
		
		// TODO: Calculate new width/height here.
		double newWidth, newHeight;
		newWidth = lastWidth;
		newHeight = lastHeight;
		
		// Create the new simulation run object.
		SimulationRun run = new SimulationRun();
		run.setStartX(computeNewStart(devianceX));
		run.setStartY(computeNewStart(devianceY));
		run.setTargetWidth(newWidth);
		run.setTargetHeight(newHeight);
		
		return run;
	}

	/**
	 * Generates a new position within the range [-deviance, deviance]
	 * @param deviance The maximum deviance in either direction from the 0-axis.
	 * @return A position in the range [-deviance, deviance]
	 */
	private double computeNewStart(double deviance)
	{
		double min = -deviance;
		double max = deviance;
		
		double start = min + (max - min) * random.nextDouble();
		
		return start;
	}

	
	
	public double getDevianceY()
	{
		return devianceY;
	}

	public void setDevianceY(double devianceY)
	{
		this.devianceY = devianceY;
	}

	public double getDevianceX()
	{
		return devianceX;
	}

	public void setDevianceX(double devianceX)
	{
		this.devianceX = devianceX;
	}

	public double getVelocity()
	{
		return velocity;
	}

	public void setVelocity(double velocity)
	{
		this.velocity = velocity;
	}

	public double getDuration()
	{
		return duration;
	}

	public void setDuration(double duration)
	{
		this.duration = duration;
	}

	public double getInitialTargetHeight()
	{
		return initialTargetHeight;
	}

	public void setInitialTargetHeight(double initialTargetHeight)
	{
		this.initialTargetHeight = initialTargetHeight;
	}

	public double getInitialTargetWidth()
	{
		return initialTargetWidth;
	}

	public void setInitialTargetWidth(double initialTargetWidth)
	{
		this.initialTargetWidth = initialTargetWidth;
	}
}
