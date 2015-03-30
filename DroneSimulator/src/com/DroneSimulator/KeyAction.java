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
		switch(action){
		case "Enter": screen.enterKey(); break;
		case "Escape": screen.escapeKey(); break;
		case "Up": screen.upKey(); break;
		case "Down": screen.downKey(); break;
		case "Right": screen.rightKey(); break;
		case "Left": screen.leftKey(); break;
		}
		
	
	}

}
