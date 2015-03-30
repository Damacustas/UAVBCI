package com.DroneSimulator;

import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;
/**
 * 
 * @author jgeerts
 * This class creates Action depending on the keypresses and returns them to screen.
 */
public class KeyAction extends AbstractAction {

	private String action;
	private Screen screen;
	
	public KeyAction(String action, Screen screen){
		this.action = action;
		this.screen = screen;
	}
	
	@Override
	public void actionPerformed(ActionEvent e) {
		if(action.equalsIgnoreCase("Enter")){
			screen.enterKey();
		} else if(action.equalsIgnoreCase("Escape")){
			screen.escapeKey();
		}
		
	}

}
