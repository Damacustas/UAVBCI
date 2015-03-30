package com.DroneSimulator;

/**
 * The parameters of a trial.
 * @author Lars Bokkers
 *
 */
public class TrialParameters
{
	private double startX, startY;
	private double targetWidth, targetHeight;
	
	public TrialParameters()
	{
		this.startX = startY = targetWidth = targetHeight = 0;
		
	}
	
	public TrialParameters(double startX, double startY)
	{
		this.startX = startX;
		this.startY = startY;
	}
	
	public double getStartX()
	{
		return startX;
	}
	
	public void setStartX(double startX)
	{
		this.startX = startX;
	}
	
	public double getTargetWidth()
	{
		return targetWidth;
	}
	
	public void setTargetWidth(double targetWidth)
	{
		this.targetWidth = targetWidth;
	}
	
	public double getTargetHeight()
	{
		return targetHeight;
	}
	
	public void setTargetHeight(double targetHeight)
	{
		this.targetHeight = targetHeight;
	}
	
	public double getStartY()
	{
		return startY;
	}
	
	public void setStartY(double startY)
	{
		this.startY = startY;
	}
	
}
