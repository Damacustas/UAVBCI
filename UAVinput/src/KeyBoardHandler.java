

import java.io.IOException;

import net.java.games.input.Component;
import net.java.games.input.Component.Identifier.Axis;
import net.java.games.input.Component.Identifier.Button;
import net.java.games.input.Controller;
import net.java.games.input.ControllerEnvironment;
import net.java.games.input.Event;
import net.java.games.input.EventQueue;
import net.java.games.input.Keyboard;
import nl.fcdonders.fieldtrip.bufferclient.BufferClientClock;
import nl.fcdonders.fieldtrip.bufferclient.BufferEvent;
import nl.fcdonders.fieldtrip.bufferclient.Header;

public class KeyBoardHandler implements Runnable {

	Keyboard keyboard;
	//Component x_axis;
//	Component w_button;
//	Component a_button;
//	Component s_button;
//	Component d_button;
//	Component esc_button;
	
	Header hdr;
	BufferClientClock c;
	
	public KeyBoardHandler()
	{
		//connectBuffer();
		
		Controller[] ca = ControllerEnvironment.getDefaultEnvironment()
				.getControllers();
		
		// find the joystick
		for (int i = 0; i < ca.length; i++)
		{
			System.out.println(ca[i].getType().toString());
			
			if (ca[i].getType() ==Controller.Type.KEYBOARD )
			{
				keyboard = (Keyboard) ca[i];
				//x_axis = keyboard.getComponent(Component.Identifier.Butt);
				
			}
		}	
		//print components
//		for (Component c : keyboard.getComponents())
//		{
//			System.out.println(c);
//			if (c.toString().equals("W"))
//			{
//				System.out.println("Yay");
//			}
//		
//		}
	}
	
	private void connectBuffer() {
		String hostname = ConnectionInfo.BUFFER_HOSTNAME;
		int port = ConnectionInfo.BUFFER_PORT;

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
		String keypressed = null;
		EventQueue queue = keyboard.getEventQueue();
        Event event = new Event();
		while (true)
		{			
			queue.getNextEvent(event);
			System.out.println(event);
			try {
			Thread.sleep(500);
		} catch (InterruptedException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
	        
			//x_axis.getPollData();
//			try {
//				c.putEvent(new BufferEvent("Joystick", x_axis.getPollData(), -1 ));
//			} catch (IOException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
//			try {
//				Thread.sleep(500);
//			} catch (InterruptedException e) {
//				// TODO Auto-generated catch block
//				e.printStackTrace();
//			}
			
		}
		

	}

}
