package com.DroneSimulator;

import java.io.IOException;

import nl.fcdonders.fieldtrip.bufferclient.*;

public class BufferReader implements Runnable {

	private Screen screen;
	Header hdr;
	BufferClientClock c;
	private final int THRESHOLD = 2;

	public BufferReader(Screen s) {
		this.screen = s;
		connectBuffer();

	}

	private void connectBuffer() {
		String hostname = Simulation.BUFFER_HOSTNAME;
		int port = Simulation.BUFFER_PORT;

		c = new BufferClientClock();

		hdr = null;
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
			// bufferConnected = true;
			// Print stuff
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
				System.out.println("Waiting for events...");
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
						// only process if trial is busy and type is
						// classifier.prediction
						// else if (evttype.equals("classifier.prediction") &&
						// screen.getState() == Screen.TRIAL_CLASSIFYING) {
						else if (evttype.equals("keyboard")
								&& screen.getState() == Screen.States.TRIAL_CLASSIFYING) {
							switch (evt.getValue().toString()) {
							case "w":
								screen.upKey();
								System.out.println("pressed W");
								break;
							case "s":
								screen.downKey();
								System.out.println("pressed S");
								break;
							case "a":
								screen.leftKey();
								System.out.println("pressed A");
								break;
							case "d":
								screen.rightKey();
								System.out.println("pressed D");
								break;
							}
						} else if (evttype
								.equalsIgnoreCase("classifier.prediction")
								&& screen.getState() == Screen.States.TRIAL_CLASSIFYING) {
							if (Math.abs(Float.parseFloat(evt.getValue()
									.toString())) > THRESHOLD) {
								if ((Float
										.parseFloat(evt.getValue().toString()) > 0)) {
									screen.upKey();
								} else if ((Float.parseFloat(evt.getValue()
										.toString()) < 0)) {
									screen.downKey();
								}
							}

						} else if (evttype
								.equalsIgnoreCase("classifier.prediction")
								&& screen.getState() == Screen.States.TRIAL_CLASSIFYING) {
							if (Math.abs(Float.parseFloat(evt.getValue()
									.toString())) > 0.2) {
								if ((Float
										.parseFloat(evt.getValue().toString()) > 0)) {
									screen.rightKey();
								} else if ((Float.parseFloat(evt.getValue()
										.toString()) < 0)) {
									screen.leftKey();
								}
							}

						} else {
							System.out.println("Event: " + evt.toString());
						}

					}
				} else { // timed out without new events
					System.out.println("Timeout waiting for events");
				}
			} catch (IOException e) {
				System.err.println("Error in " + this.getClass().getName());
				e.printStackTrace();
			}
			// try {
			// //Thread.sleep(1000);
			// } catch (InterruptedException e) {
			// // TODO Auto-generated catch block
			// e.printStackTrace();
			// }
		}
	}
}
