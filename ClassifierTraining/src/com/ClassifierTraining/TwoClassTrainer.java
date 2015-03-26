package com.ClassifierTraining;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.Random;

public class TwoClassTrainer implements Trainer {

	private String[] classes = { "music", "house" };
	private static final int TOTAL_TRIALS = 80;
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
			if (++trialcounter % 40 == 0) {
				System.out.println(" Breaktime! (30 seconds)");
				try {
					Thread.sleep(30000);
				} catch (InterruptedException e) {
				}
				// small break every 5
			} else if (trialcounter % 5 == 0) {
				System.out.println(" Breaktime! (5 seconds)");
				try {
					Thread.sleep(5000);
				} catch (InterruptedException e) {
				}
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

		for (int i = 0; i < TOTAL_TRIALS; i++) {
			cues.add(classes[i % classes.length]);
		}

		Collections.shuffle(cues);
		return cues;
	}
	
	

}
