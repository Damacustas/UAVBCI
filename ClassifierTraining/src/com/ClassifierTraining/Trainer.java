package com.ClassifierTraining;

import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;
import java.util.Random;
import java.io.*;
import java.nio.*;

import nl.fcdonders.fieldtrip.bufferclient.*;

import javax.swing.Timer;

public class Trainer {

	private int shortBreakTrials = 5;
	private int longBreakTrials = 40;
	private String[] classes = new String[2];
	private int totalTrials = 80;
	private ArrayList<String> cues = new ArrayList<String>();
	private Screen screen;

	public Trainer(Screen screen, String[] classes){
	
		this.screen = screen;
		
		for (int i = 0; i<classes.length;i++)
			this.classes[i] = classes[i];
		cues = addCues();
		//this.classes = classes;

	}

	public void startClassifierTraining() throws IOException {
		// TODO Add buffer stuff
		String hostname = "localhost";
		int port = 1972;
		int timeout = 5000;
		
		BufferClientClock c = new BufferClientClock();

		Header hdr = null;
		while (hdr == null) {
			try {
				System.out.println("Connecting to " + hostname + ":" + port);
				c.connect(hostname, port);
				// C.setAutoReconnect(true);
				if (c.isConnected()) {
					System.out.println("GETHEADER");
					hdr = c.getHeader();
				}
			} catch (IOException e) {
				hdr = null;
			}
			if (hdr == null) {
				System.out.println("Invalid Header... waiting");
				try {
					Thread.sleep(1000);
				} catch (InterruptedException e) {
					// TODO Auto-generated catch block
					e.printStackTrace();
				}
			}
			

		}
		//Print stuff
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
		
		
		Iterator<String> it = cues.iterator();
		int trialcounter = 0;
		String next;
		// Loop through all trials
		while (it.hasNext()) {
			next = it.next();
			System.out.println("Now doing: " + next);
			try {
				screen.setState(Screen.TRIAL_START);
				//TODO cahnge?
				c.putEvent(new BufferEvent("Start", "", -1));
				Thread.sleep(1000);
				screen.setCue(next);
				screen.setState(Screen.TRIAL_CUE);
				c.putEvent(new BufferEvent("Cue", next, -1));
				Thread.sleep(4000);
				screen.setState(Screen.TRIAL_EMPTY);
				c.putEvent(new BufferEvent("Finish", "", -1));
				Thread.sleep(randomBreakTime());
			} catch (InterruptedException e) {
				System.err.println("ERROR");
			}

			// break every 40 trials (30s)
			if (++trialcounter % longBreakTrials == 0) {
				System.out.println(" Breaktime! (30 seconds)");
				screen.setState(Screen.TRIAL_BREAK);
				c.putEvent(new BufferEvent("Break", 30, -1));
				screen.startCountdown(30);
				// screen.setBreakTimeLeft(30);

				// small break every 5
			} else if (trialcounter % shortBreakTrials == 0) {
				System.out.println(" Breaktime! (5 seconds)");

				screen.setState(Screen.TRIAL_BREAK);
				c.putEvent(new BufferEvent("Break", 5, -1));
				screen.startCountdown(5);

			}
		}
		c.disconnect();
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
