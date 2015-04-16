package com.ClassifierTraining;

import java.awt.event.ActionEvent;

import javax.swing.AbstractAction;


/**
 * 
 * @author jgeerts
 * This class creates Action depending on the keypresses and returns them to screen.
 */
@SuppressWarnings("serial")
public class KeyAction extends AbstractAction  {

	private String action;
	private Screen screen;
	
	public KeyAction(String action, Screen screen) {
		this.action = action;
		this.screen = screen;
	}

	@Override
	public void actionPerformed(ActionEvent e) {
	
		switch(action){
		case "Space": screen.spaceKey(); break;
		}
	}

	

}
