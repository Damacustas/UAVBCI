package com.ClassifierTraining;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.Random;

import javax.swing.Timer;

public class TwoClassTrainer implements Trainer {

	private int shortBreakTrials = 5;
	private int longBreakTrials = 40;
	private String[] classes = { "music", "house" };
	private int totalTrials = 80;
	private ArrayList<String> cues = new ArrayList<String>();
	private Screen screen;

	public TwoClassTrainer(Screen screen) {
		cues = addCues();
		this.screen = screen;

	}

	@Override
	public void startClassifierTraining() {
		// TODO Auto-generated method stub
		Iterator<String> it = cues.iterator();

		int trialcounter = 0;
		String next;
		// Loop through all trials
		while (it.hasNext()) {
			// System.out.println(it.next());
			next = it.next();
			System.out.println("Now doing: " + next);
			try {
				screen.setState(Screen.TRIAL_START);
				Thread.sleep(1000);
				screen.setCue(next);
				screen.setState(Screen.TRIAL_CUE);
				Thread.sleep(4000);
				screen.setState(Screen.TRIAL_EMPTY);
				Thread.sleep(randomBreakTime());
			} catch (InterruptedException e) {
				System.err.println("ERROR");
			}

			// break every 40 trials (30s)
			if (++trialcounter % longBreakTrials == 0) {
				System.out.println(" Breaktime! (30 seconds)");
				// screen.setBreakTimeLeft(30);
				screen.setState(Screen.TRIAL_BREAK);
				screen.startCountdown(30);
				// screen.setBreakTimeLeft(30);

				// small break every 5
			} else if (trialcounter % shortBreakTrials == 0) {
				System.out.println(" Breaktime! (5 seconds)");

				screen.setState(Screen.TRIAL_BREAK);
				screen.startCountdown(5);

			}
		}
	}

	/**
	 * 
	 * @return random number between 500 and 2500
	 */
	private long randomBreakTime() {
		Random rand = new Random();
		return rand.nextInt(2000) + 500;
	}

	private ArrayList<String> addCues() {
		ArrayList<String> cues = new ArrayList<String>();

		for (int i = 0; i < totalTrials; i++) {
			cues.add(classes[i % classes.length]);
		}

		Collections.shuffle(cues);
		return cues;
	}

	public void setShortBreakTrials(int shortBreakTrials) {
		this.shortBreakTrials = shortBreakTrials;
	}

	public void setLongBreakTrials(int longBreakTrials) {
		this.longBreakTrials = longBreakTrials;
	}

	public void setTotalTrials(int totalTrials) {
		this.totalTrials = totalTrials;
	}

}
