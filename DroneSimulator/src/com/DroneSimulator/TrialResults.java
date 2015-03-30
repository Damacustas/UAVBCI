package com.DroneSimulator;

/**
 * Represents the results of a trial.
 * @author Lars Bokkers
 *
 */
public class TrialResults
{
	// The trial for which this class holds the results.
	private TrialParameters trialParams;
	
	// Whether the cursor hit the target.
	private boolean isHit;
	
	// The time that expired during the trial.
	private double timeExpired; // In seconds.
	
	public TrialResults(){
	}

	
	
	public double getTimeRequired() {
		return timeExpired;
	}

	public void setTimeRequired(double timeRequired) {
		this.timeExpired = timeRequired;
	}

	public boolean isHit() {
		return isHit;
	}

	public void setHit(boolean isHit) {
		this.isHit = isHit;
	}

	public TrialParameters getSimulationDetails() {
		return trialParams;
	}

	public void setSimulationDetails(TrialParameters simulationDetails) {
		this.trialParams = simulationDetails;
	}
}
