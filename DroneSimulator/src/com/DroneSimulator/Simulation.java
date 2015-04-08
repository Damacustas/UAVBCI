package com.DroneSimulator;

import java.awt.Dimension;
import java.awt.Toolkit;
import java.util.Random;

public class Simulation {
	public static final int TRIAL_LENGTH = 5000;

	// These represent the constant values of the simulation.
	private double velocity, duration;

	// These represent the maximum starting distance
	// of the simulated drone from the X/Y axis respectively.
	private double devianceX, devianceY;

	// These represent the initial dimensions of the target.
	private double initialTargetHeight, initialTargetWidth;

	// // These represent the percentages of targets hit by the user, depending
	// on the dimension.
	// private double xHits, yHits, totalTrials;

	// Used to generate the random start positions.
	private Random random;

	// Used to find the screensize.
	private Dimension dim = null;

	private Screen screen;

	private TrialParameters last = null;

	private int shortBreakTrials = 1;
	private int longBreakTrials = 2;
	private int totalTrials = 80;

	public void setShortBreakTrials(int shortBreakTrials) {
		this.shortBreakTrials = shortBreakTrials;
	}

	public void setLongBreakTrials(int longBreakTrials) {
		this.longBreakTrials = longBreakTrials;
	}

	public Simulation(Screen s) {
		this.screen = s;
		random = new Random();
		velocity = duration = devianceX = devianceY = 0;
		// xHits = yHits = totalTrials = 0;
		initialTargetHeight = initialTargetWidth = 100;
		dim = Toolkit.getDefaultToolkit().getScreenSize();
		startExperiment();
	}

	public void startExperiment() {
		int teller = 0;
		while (teller < totalTrials) {
			if (teller % longBreakTrials == 0 && teller != 0) {
				screen.setState(Screen.TRIAL_BREAK);
				screen.startCountdown(30);

			} else if (teller % shortBreakTrials == 0 && teller != 0) {
				screen.setState(Screen.TRIAL_BREAK);
				screen.startCountdown(5);
			}
			screen.setCurrentTrial(generateNext());
//			try {
//				Thread.sleep(randomBreakTime());
//			} catch (InterruptedException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
			screen.setState(Screen.TRIAL_BUSY);
			screen.reset();			
			screen.showProgressBar();
			try {
				Thread.sleep(TRIAL_LENGTH);
			} catch (InterruptedException e) {
				e.printStackTrace();
			}

			teller++;
		}
	}

	/**
	 * private void generateInitial() { // Create the first simulation run
	 * object. last = new TrialParameters(); double angle =
	 * random.nextDouble()*360; last.setStartX(dim.width/2 +
	 * 200*Math.cos(angle)); last.setStartY(dim.height/2 + 200*Math.sin(angle));
	 * last.setTargetWidth(initialTargetWidth);
	 * last.setTargetHeight(initialTargetHeight); }
	 */

	public TrialParameters generateNext() {
		double lastWidth, lastHeight;

		if (last != null) {
			lastWidth = last.getTargetWidth();
			lastHeight = last.getTargetHeight();
		} else {
			lastWidth = initialTargetWidth;
			lastHeight = initialTargetHeight;
		}

		// TODO: Calculate new width/height here.

		// Pseudocode because there are no necessary variables/classes for
		// controlling the cursor implemented yet
		//
		// totalTrials++
		// if lastXHit
		// xHits++;
		//
		// double hitPercentageX = xHits / totalTrials;
		// if hitPercentageX > grensVoorPerformance + waardeOmChanceTeVerkleinen
		// {
		// newWidth = last.getTargetWidth() - someValue; (or
		// last.getTargetWidth() / someValue)
		// }
		// else
		// if hitPercentageX < grensVoorPerformance - waardeOmChanceTeVerkleinen
		// newWidth = last.getTargetWidth() + someValue; (or....)
		//
		// Same for calculating the new height
		//
		//
		//
		//
		//
		//
		//
		//
		// SOMEWHERE IN A METHOD CALL totalTrials++

		double newWidth, newHeight;
		newWidth = lastWidth;
		newHeight = lastHeight;

		// Create the new simulation run object.
		TrialParameters run = new TrialParameters();
		double angle = random.nextDouble() * 360;
		run.setStartX(dim.width / 2 + 200 * Math.cos(angle));
		run.setStartY(dim.height / 2 + 200 * Math.sin(angle));
		run.setTargetWidth(newWidth);
		run.setTargetHeight(newHeight);
		return run;
	}

	/**
	 * Adds the provided SimulationRun to the saved records.
	 * 
	 * @param run
	 *            The run to save.
	 * @return
	 */
	public void reportSimulationResults(TrialResults run) {
		// TODO: implement.
		// My idea was to save all the data to a json file.
		// Alternatively, we can save it to a csv (comma seperated value) file.
	}

	public double getDevianceY() {
		return devianceY;
	}

	public void setDevianceY(double devianceY) {
		this.devianceY = devianceY;
	}

	public double getDevianceX() {
		return devianceX;
	}

	public void setDevianceX(double devianceX) {
		this.devianceX = devianceX;
	}

	public double getVelocity() {
		return velocity;
	}

	public void setVelocity(double velocity) {
		this.velocity = velocity;
	}

	public double getDuration() {
		return duration;
	}

	public void setDuration(double duration) {
		this.duration = duration;
	}

	public double getInitialTargetHeight() {
		return initialTargetHeight;
	}

	public void setInitialTargetHeight(double initialTargetHeight) {
		this.initialTargetHeight = initialTargetHeight;
	}

	public double getInitialTargetWidth() {
		return initialTargetWidth;
	}

	public void setInitialTargetWidth(double initialTargetWidth) {
		this.initialTargetWidth = initialTargetWidth;
	}

	/**
	 * 
	 * @return random number between 500 and 2500
	 */
	private long randomBreakTime() {
		Random rand = new Random();
		return rand.nextInt(2000) + 500;
	}

}
