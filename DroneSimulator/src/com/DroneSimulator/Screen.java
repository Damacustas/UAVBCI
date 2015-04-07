package com.DroneSimulator;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.FlowLayout;
import java.awt.Graphics;
import java.awt.GridBagConstraints;
import java.awt.GridBagLayout;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.event.WindowEvent;
import java.util.ArrayList;

import javax.swing.*;

/**
 * Represents the visual aspect of the simulation.
 * 
 * @author Lars Bokkers, Joost Coenen, Jouke Geerts, Robert Jansen
 *
 */
public class Screen extends JPanel {

	private static final int TIMER_DELAY = 10;
	// The frame that contains the display.
	private JFrame frame;
	private JProgressBar progressBar = null;

	// The dimensions of the whole desktop.
	private Dimension dim = null;

	// The parameters for the current trial.
	private TrialParameters currentTrial = null;

	// The Simulation which the screen is running.
	//private Simulation simulation = null;

	// The location of the cursor.
	private int cursorX, cursorY;
	
	private int state = Screen.TRIAL_IDLE;

	// The timer for the progressbar
	private Timer timer;
	
	public static final int TRIAL_BUSY = 0;
	public static final int TRIAL_BREAK = 1;
	public static final int TRIAL_IDLE = 2;
	
	private ArrayList<TrialResults> results = new ArrayList<TrialResults>();

	/**
	 * Initializes the screen with the given simulation.
	 * 
	 * @param sim
	 *            The simulation to run in the screen.
	 */
	public Screen() {
		// Initialize fields
		//this.simulation = sim;
		// TODO put in loop: this.currentTrial = this.simulation.generateNext();
//		cursorX = (int) currentTrial.getStartX();
//		cursorY = (int) currentTrial.getStartY();

		// Initialize display frame.
		frame = new JFrame();
		frame.setTitle("UAV BCI Software");
		frame.setExtendedState(JFrame.MAXIMIZED_BOTH);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		this.setLayout(new BoxLayout(this, BoxLayout.Y_AXIS));
		this.setBackground(Color.WHITE);
		frame.add(this);
		frame.setVisible(true);
		dim = Toolkit.getDefaultToolkit().getScreenSize();

		// Initialize progress bar.
		progressBar = new JProgressBar(0, 5000);
		progressBar.setValue(0);
		//progressBar.setSize(100, 100);
		progressBar.setPreferredSize(new Dimension(100,100));
		// progressBar.setValue(1);
		//progressBar.setStringPainted(true);
		progressBar.setVisible(false);
		progressBar.setBackground(Color.BLACK);
		this.add(progressBar);

		// Initialize timer for progress bar
		
		timer = new Timer(TIMER_DELAY, new ActionListener() {

			
			@Override
			public void actionPerformed(ActionEvent e) {
				int value = progressBar.getValue() + TIMER_DELAY;
				if (value > progressBar.getMaximum()) {
					// time over! logic here
					value = 0;
					timer.stop();

					//submitResult();
					//reset();			

				}
				progressBar.setValue(value);
				repaint();
			}

		});

		timer.start();

		// Initialise Maps for Keybindings.
		InputMap im = this.getInputMap(JPanel.WHEN_IN_FOCUSED_WINDOW);
		ActionMap am = this.getActionMap();

		// Keybindings
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_ENTER, 0), "Enter");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_ESCAPE, 0), "Escape");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_UP, 0), "Up");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_DOWN, 0), "Down");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_RIGHT, 0), "Right");
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_LEFT, 0), "Left");

		am.put("Escape", new KeyAction("Escape", this));
		am.put("Enter", new KeyAction("Enter", this));
		am.put("Up", new KeyAction("Up", this));
		am.put("Down", new KeyAction("Down", this));
		am.put("Right", new KeyAction("Right", this));
		am.put("Left", new KeyAction("Left", this));
	}

	public void setCurrentTrial(TrialParameters currentTrial) {
		this.currentTrial = currentTrial;
	}

	@Override
	public void paint(Graphics g) {
		super.paint(g);
		// Robert: moet ie resetten zodra ie geraakt wordt? Beter de 5 seeconden
		// afwachten?
		// update: code verplaatst naar actionPerformed van de Timer (listener
		// daarvan :)).
		// if(isHit() ){
		// submitResult();
		//
		// currentTrial = this.simulation.generateNext(); //Reset simulation
		// cursorX = (int) currentTrial.getStartX(); // Adjust Coordinates
		// cursorY = (int) currentTrial.getStartY(); //
		// }
		
		if (state == Screen.TRIAL_BUSY)
		{
			int height = (int) currentTrial.getTargetHeight();
			int width = (int) currentTrial.getTargetWidth();

			g.setColor(Color.RED);
			g.fillRect(dim.width / 2 - width / 2, dim.height / 2 - height / 2,
					width, height);
			g.setColor(Color.BLACK);
			g.fillOval(cursorX, cursorY, 10, 10);
		}
		else if (state == Screen.TRIAL_BREAK)
		{
			// iets met countdown.
		}
		
	}

	/**
	 * Submits the simulation results to the Simulation.
	 */
	private void submitResult() {
		TrialResults result = new TrialResults();
		result.setHit(isHit());
		//result.setTimeRequired(1337); // TODO: change
		result.setSimulationDetails(currentTrial);
		// TODO put in loop: simulation.reportSimulationResults(result);
	}

	/**
	 * Handles enter key presses
	 */
	public void enterKey() {
		reset();
		this.repaint();
	}

	/**
	 * Handles escape key presses
	 */
	public void escapeKey() {
		this.frame.dispatchEvent(new WindowEvent(frame,
				WindowEvent.WINDOW_CLOSING));
	}

	/**
	 * Handles up key presses
	 */
	public void upKey() {
		cursorY -= 35;
		cursorY = (cursorY < 0) ? 0 : cursorY;
		this.repaint();
	}

	/**
	 * Handles down key presses
	 */
	public void downKey() {
		cursorY += 35;
		cursorY = (int) ((cursorY > (dim.getHeight() - 70)) ? (dim.getHeight() - 70)
				: cursorY);
		this.repaint();
	}

	/**
	 * Handles right key presses
	 */
	public void rightKey() {
		cursorX += 35;
		cursorX = (int) ((cursorX > (dim.getWidth() - 10)) ? (dim.getWidth() - 10)
				: cursorX);
		this.repaint();
	}

	/**
	 * Handles left key presses
	 */
	public void leftKey() {
		cursorX -= 35;
		cursorX = (cursorX < 0) ? 0 : cursorX;
		this.repaint();
	}

	public void setState(int state) {
		this.state = state;
	}

	/**
	 * Checks if the cursor hit the target
	 * 
	 * @return true if hit, otherwise false.
	 */
	public boolean isHit() {
		return (cursorY) <= (dim.getHeight() / 2 + currentTrial
				.getTargetHeight() / 2)
				&& (cursorY) >= (dim.getHeight() / 2 - currentTrial
						.getTargetHeight() / 2)
				&& (cursorX) <= (dim.getWidth() / 2 + currentTrial
						.getTargetWidth() / 2)
				&& (cursorX) >= (dim.getWidth() / 2 - currentTrial
						.getTargetWidth() / 2);
	}
// TODO put in loop:
	public void reset() {
		// TODO put in loop: currentTrial = simulation.generateNext(); // Reset simulation
		cursorX = (int) currentTrial.getStartX(); // Adjust Coordinates
		cursorY = (int) currentTrial.getStartY(); //
		timer.stop();
		progressBar.setValue(0);
		timer.start();
	}
	public void startTimer()
	{
		//timer.st
	}

	public void showProgressBar() {
		progressBar.setVisible(true);
		
	}


}
