
package com.DroneSimulator;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Toolkit;
import java.awt.event.KeyEvent;

import java.awt.event.WindowEvent;

import javax.swing.*;

public class Screen extends JPanel{
	
	private JFrame frame;
	private Dimension dim = null;
	private TrialParameters trial = null;
	private Simulation simulation = null;
	private JProgressBar progressBar = null;
	
	private int cursorX, cursorY;
	
	public Screen(Simulation sim)
	{
		this.simulation = sim;
		this.trial = this.simulation.generateNext();
		
		frame = new JFrame();
		frame.setTitle("UAV BCI Software");
		frame.setExtendedState(JFrame.MAXIMIZED_BOTH);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		this.setBackground(Color.WHITE);
		frame.add(this);
		frame.setVisible(true);	
		dim = Toolkit.getDefaultToolkit().getScreenSize();
		
		this.progressBar = new JProgressBar();
		
		
		cursorX = (int) trial.getStartX();
        cursorY = (int) trial.getStartY();
		
		//Initialise Maps for Keybindings.
		InputMap im = this.getInputMap(JPanel.WHEN_IN_FOCUSED_WINDOW);
		ActionMap am = this.getActionMap();
		
		//Keybindings
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_ENTER, 0), "Enter");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE,0), "Escape");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_UP,0), "Up");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_DOWN,0), "Down");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_RIGHT,0), "Right");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_LEFT,0), "Left");
		
		am.put("Escape", new KeyAction("Escape", this));
		am.put("Enter", new KeyAction("Enter", this));
		am.put("Up", new KeyAction("Up", this));
		am.put("Down", new KeyAction("Down", this));
		am.put("Right", new KeyAction("Right", this));
		am.put("Left", new KeyAction("Left", this));
	}
	
	@Override
	public void paint(Graphics g) 
    {
        super.paint(g);
        if(isHit()){
        	submitResult();
        	
        	trial = this.simulation.generateNext(); //Reset simulation
        	cursorX = (int) trial.getStartX();	// Adjust Coordinates
        	cursorY = (int) trial.getStartY(); //
        }
		int height = (int) trial.getTargetHeight();
        int width = (int) trial.getTargetWidth();
        
        g.setColor(Color.RED);
		g.fillRect(dim.width/2 - width/2, dim.height/2 - height/2, width, height);
		g.setColor(Color.BLACK);
		g.fillOval(cursorX, cursorY, 10, 10);
    }
	
	private void submitResult()
	{
		TrialResults results = new TrialResults();
		results.setHit(isHit());
		results.setTimeRequired(1337); // TODO: change
		results.setSimulationDetails(trial);
		simulation.reportSimulationResults(results);
	}

	/**
	 * Handles enter key presses
	 */
	public void enterKey(){
		this.trial = this.simulation.generateNext(); //Reset simulation
		cursorX = (int) trial.getStartX();	// Adjust Coordinates
    	cursorY = (int) trial.getStartY(); //
		this.repaint();
	}
	
	/**
	 * Handles escape key presses
	 */
	public void escapeKey(){
		this.frame.dispatchEvent(new WindowEvent(frame, WindowEvent.WINDOW_CLOSING));
	}
	
	/**
	 * Handles up key presses
	 */
	public void upKey(){
		cursorY -= 10;
		cursorY = (cursorY < 0) ? 0 : cursorY; 
		this.repaint();
	}
	
	/**
	 * Handles down key presses
	 */
	public void downKey(){
		cursorY += 10;
		cursorY = (int) ((cursorY > (dim.getHeight()-70)) ? (dim.getHeight()-70) : cursorY); 
		this.repaint();
	}
	
	/**
	 * Handles right key presses
	 */
	public void rightKey(){
		cursorX += 10;
		cursorX = (int) ((cursorX > (dim.getWidth()-10)) ? (dim.getWidth()-10) : cursorX); 
		this.repaint();
	}
	
	/**
	 * Handles left key presses
	 */
	public void leftKey(){
		cursorX -= 10;
		cursorX = (cursorX < 0) ? 0 : cursorX;
		this.repaint();
	}
	
	/**
	 * Checks if the cursor hit the target
	 * @return true if hit, otherwise false.
	 */
	public boolean isHit(){
		return 	   (cursorY) <= (dim.getHeight()/2 + trial.getTargetHeight()/2)  
				&& (cursorY) >= (dim.getHeight()/2 - trial.getTargetHeight()/2)
				&& (cursorX) <= (dim.getWidth()/2 + trial.getTargetWidth()/2)
				&& (cursorX) >= (dim.getWidth()/2 - trial.getTargetWidth()/2);
	}
}

	


