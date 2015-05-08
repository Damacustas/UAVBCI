package com.ClassifierTraining;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.Random;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.io.*;

import javax.swing.Timer;

import nl.fcdonders.fieldtrip.bufferclient.*;

public class Trainer {
	private static final int BUFFER_PORT = 1972;
	private static final String BUFFER_HOSTNAME = "localhost";
	private static final int LONG_BREAK_TIME = 30;
	private static final int SHORT_BREAK_TIME = 10;
	//
	private int shortBreakTrials = 5;
	private int longBreakTrials = 40;
	private int totalTrials = 80;
	private String[] classes = new String[2];

	private ArrayList<String> cues = new ArrayList<String>();
	private Screen screen;
	
	// Used for timing 
	boolean sleepTimeOver = false;

	public Trainer(Screen screen, String[] classes) {

		this.screen = screen;

		for (int i = 0; i < classes.length; i++)
			this.classes[i] = classes[i];
		cues = addCues();
	}

	public void startClassifierTraining() throws IOException {
		String hostname = BUFFER_HOSTNAME;
		int port = BUFFER_PORT;
		BufferClientClock c = new BufferClientClock();

		// try to connect to bufferclientclock and retrieve header
		Header hdr = connect(hostname, port, c);
		printSettings(hdr);

		Iterator<String> it = cues.iterator();
		int trialcounter = 0;
		String next;
		screen.setState(Screen.States.TRIAL_BREAK);
		screen.startCountdown(LONG_BREAK_TIME);
		// Loop through all trials (and do stuff)
		while (it.hasNext()) {
			c.putEvent(new BufferEvent("Trial", trialcounter, -1));
			next = it.next();
			//System.out.println("Now doing: " + next);
			screen.setState(Screen.States.TRIAL_START);
			// TODO change events?
			c.putEvent(new BufferEvent("Start", "", -1));

			// cue shown after 1 second
			sleep(1000);
			screen.setCue(next);
			screen.setState(Screen.States.TRIAL_CUE);
			c.putEvent(new BufferEvent("Cue", next, -1));

			// start "classifying phase" (data being collected) after 2 seconds
			// (total)
			sleep(1000);
			screen.setState(Screen.States.TRIAL_CLASSIFYING);

			// clear the screen after 5 seconds (total)
			sleep(3000);
			screen.setState(Screen.States.TRIAL_EMPTY);
			c.putEvent(new BufferEvent("Finish", "", -1));

			// break every 40 trials (30s)
			if (++trialcounter % longBreakTrials == 0) {
				System.out.printf(" Breaktime! (%d seconds)", LONG_BREAK_TIME);
				screen.setState(Screen.States.TRIAL_BREAK);
				c.putEvent(new BufferEvent("Break", LONG_BREAK_TIME, -1));
				screen.startCountdown(LONG_BREAK_TIME);

				// small break every 5
			} else if (trialcounter % shortBreakTrials == 0) {
				System.out.printf(" Breaktime! (%d seconds)", SHORT_BREAK_TIME);
				screen.setState(Screen.States.TRIAL_BREAK);
				// c.putEvent(new BufferEvent("Break", 5, -1));
				screen.startCountdown(SHORT_BREAK_TIME);
			}
			sleep(randomBreakTime());
		}
		// disconnect for bufferclientclock when done
		// c.putEvent(new BufferEvent("exit", "", -1));
		c.disconnect();
	}

	private void sleep(final long ms) {
		try {
			Thread.sleep(ms);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		
//		Timer t = new Timer(500, new ActionListener() {
//			long starttime = System.currentTimeMillis();
//			@Override
//			public void actionPerformed(ActionEvent arg0) {
//				// TODO Auto-generated method stub
//				sleepTimeOver = (System.currentTimeMillis() - starttime) >= ms;				
//			}
//		});
//		while(!sleepTimeOver)
//		{
//			
//		}
	}

	private void printSettings(Header hdr) {
		System.out.println("#channels....: " + hdr.nChans);
		System.out.println("#samples.....: " + hdr.nSamples);
		System.out.println("#events......: " + hdr.nEvents);
		System.out.println("Sampling Freq: " + hdr.fSample);
		System.out.println("data type....: " + hdr.dataType);
		for (int n = 0; n < hdr.nChans; n++) {
			if (hdr.labels[n] != null) {
				System.out.println("Ch. " + n + ": " + hdr.labels[n]);
			}
		}
	}

	private Header connect(String hostname, int port, BufferClientClock C) {
		Header hdr = null;
		while (hdr == null) {
			try {
				System.out.println("Connecting to " + hostname + ":" + port);
				C.connect(hostname, port);
				// C.setAutoReconnect(true);
				if (C.isConnected()) {
					System.out.println("GETHEADER");
					hdr = C.getHeader();
				}
			} catch (IOException e) {
				hdr = null;
			}
			if (hdr == null) {
				System.out.println("Invalid Header... waiting");
				try {
					Thread.sleep(1000);
				} catch (InterruptedException e) {
					e.printStackTrace();
				}
			}

		}
		return hdr;
	}

	/**
	 * 
	 * @return random number between 500 and 2500
	 */
	private long randomBreakTime() {
		Random rand = new Random();
		return rand.nextInt(2000) + 1500;
	}

	private ArrayList<String> addCues() {
		// TODO Make semi-random instead of fully random (remove triple+)
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
