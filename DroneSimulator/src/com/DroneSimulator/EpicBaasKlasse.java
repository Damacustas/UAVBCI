package com.DroneSimulator;

import java.io.IOException;

import nl.fcdonders.fieldtrip.bufferclient.*;

public class EpicBaasKlasse implements Runnable {

	private Screen screen;
	Header hdr;
	BufferClientClock c;

	public EpicBaasKlasse(Screen s, Header hdr, BufferClientClock c) {
		this.screen = s;
		this.hdr = hdr;
		this.c = c;

	}

	@Override
	public void run() {
		// TODO Auto-generated method stub
		// check for events and stuff
		int timeout = 5000;
		// Now do the echo-server
		int nEvents = hdr.nEvents;
		boolean endExpt = false;
		while (!endExpt) {
			try {
				SamplesEventsCount sec = c.waitForEvents(nEvents, timeout); // Block
																			// until
																			// there
																			// are
																			// new
																			// events
				if (sec.nEvents > nEvents) {
					// get the new events
					BufferEvent[] evs = c.getEvents(nEvents, sec.nEvents - 1);
					nEvents = sec.nEvents;// update record of which events we've
											// seen
					// filter for ones we want
					System.out.println("Got " + evs.length + " events");
					for (int ei = 0; ei < evs.length; ei++) {
						BufferEvent evt = evs[ei];
						String evttype = evt.getType().toString(); // N.B.
																	// to*S*tring,
																	// not upper
																	// case!
						if (evttype.equals("exit")) { // check for a finish
							// event
							endExpt = true;
						}
						// only process if it's an event of a type we care about
						// only process if trial is busy and type is classifier.prediction 
						else if (evttype.equals("classifier.prediction") && screen.getState() == Screen.TRIAL_CLASSIFYING) {
							switch(evt.getValue().toString())
							{
							case "up": screen.upKey(); break;
							case "down": screen.downKey(); break;
							case "left": screen.leftKey(); break;
							case "right": screen.rightKey(); break;
							}
						}

					}
				} else { // timed out without new events
					System.out.println("Timeout waiting for events");
				}
			} catch (IOException e) {
				System.err.println("Error in " + this.getClass().getName());
				e.printStackTrace();
			}
		}
	}
}
