package com.DroneSimulator;

import java.io.IOException;

import net.java.games.input.Component;
import net.java.games.input.Component.Identifier.Axis;
import net.java.games.input.Controller;
import net.java.games.input.ControllerEnvironment;
import nl.fcdonders.fieldtrip.bufferclient.BufferClientClock;
import nl.fcdonders.fieldtrip.bufferclient.BufferEvent;
import nl.fcdonders.fieldtrip.bufferclient.Header;

public class JoystickHandler implements Runnable {

	Controller joystick;
	Component x_axis;
	Header hdr;
	BufferClientClock c;
	
	public JoystickHandler()
	{
		connectBuffer();
		
		Controller[] ca = ControllerEnvironment.getDefaultEnvironment()
				.getControllers();
		
		// find the joystick
		for (int i = 0; i < ca.length; i++)
		{
			
			if (ca[i].getType() ==Controller.Type.STICK )
			{
				joystick = ca[i];
				x_axis = joystick.getComponent(Component.Identifier.Axis.X);
			}
		}		
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
			//bufferConnected = true;

		}
	}
	@Override
	public void run() {
		// TODO Auto-generated method stub

		while (true)
		{
			joystick.poll();		
			
			x_axis.getPollData();
			try {
				c.putEvent(new BufferEvent("Joystick", x_axis.getPollData(), -1 ));
			} catch (IOException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			
		}
		

	}

}
