package com.ClassifierTraining;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Font;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.GridBagLayout;
import java.awt.Toolkit;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.awt.event.KeyEvent;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;
import javax.swing.*;

@SuppressWarnings("serial")
public class Screen extends JPanel {
	// Size settings
	private static final int CROSS_SIZE = 50;
	private static final int CUE_ICON_HEIGHT = 100;
	private static final int CUE_ICON_WIDTH = 100;

	// States
	public static final int TRIAL_EMPTY = 0;
	public static final int TRIAL_START = 1;
	public static final int TRIAL_CUE = 2;
	public static final int TRIAL_BREAK = 3;

	private JFrame frame;
	private Dimension dim = null;
	private int state = 0;
	private String cue;
	private int breakTimeLeft;
	private JLabel countdownLabel = new JLabel();

	public Screen() {
		frame = new JFrame();
		frame.setTitle("UAV BCI Software");
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setExtendedState(JFrame.MAXIMIZED_BOTH);
		frame.add(this);
		frame.setVisible(true);
		this.setBackground(Color.WHITE);
		this.setLayout(new GridBagLayout());

		dim = Toolkit.getDefaultToolkit().getScreenSize();

		countdownLabel.setVisible(false);
		countdownLabel.setFont(new Font(null, Font.PLAIN, 100));
		countdownLabel.setHorizontalAlignment(SwingConstants.CENTER);
		countdownLabel.setText("");
		countdownLabel.setLocation(dim.width / 2, dim.height / 2);
		this.add(countdownLabel);

		//Initialise Maps for Keybindings.
		InputMap im = this.getInputMap(JPanel.WHEN_IN_FOCUSED_WINDOW);
		ActionMap am = this.getActionMap();
		
		im.put(KeyStroke.getKeyStroke(KeyEvent.VK_SPACE, 0), "Space");
		am.put("Space", new KeyAction("Space", this));
	}

	public void setCue(String cue) {
		this.cue = cue;
	}

	public void paintComponent(Graphics g) {
		super.paintComponent(g);

		if (state == TRIAL_START) {
			drawFixationCross(g);
			// playBeep();
		} else if (state == TRIAL_CUE) {
			drawFixationCross(g);
			drawCueImage(g, cue);
		} else if (state == TRIAL_BREAK) {
			drawCountdown(g);
		}
	}

	private void drawCountdown(Graphics g) {
		// TODO Auto-generated method stub
		countdownLabel.setText(Integer.toString(breakTimeLeft));
	}

	private void drawFixationCross(Graphics g) {
		Graphics2D g2 = (Graphics2D) g;
		g2.setStroke(new BasicStroke(3));
		g2.drawLine(dim.width / 2 - CROSS_SIZE / 2, dim.height / 2, dim.width
				/ 2 + CROSS_SIZE / 2, dim.height / 2);
		g2.drawLine(dim.width / 2, dim.height / 2 - CROSS_SIZE / 2,
				dim.width / 2, dim.height / 2 + CROSS_SIZE / 2);

	}

	private void drawCueImage(Graphics g, String cue) {
		BufferedImage img = null;
		String filename = "Images/" + cue + ".png";
		try {

			img = ImageIO.read(new File(filename));
		} catch (IOException ex) {
			System.err.println("Something went wrong loading the image ("
					+ filename + ")");
		}

		g.drawImage(img, dim.width / 2 - CUE_ICON_WIDTH / 2, dim.height / 2
				- CUE_ICON_HEIGHT / 2, CUE_ICON_WIDTH, CUE_ICON_HEIGHT,
				Color.white, null);

	}

	public void setState(int state) {
		this.state = state;
		// should probably post a bufferevent or sometihng
		repaint();
	}

	public void showCountdown() {
		countdownLabel.setVisible(true);
	}

	public void hideCountdown() {
		countdownLabel.setVisible(false);
	}

	private void playBeep() {
		// TODO Find better beep
		Toolkit.getDefaultToolkit().beep();
	}

	public void setBreakTimeLeft(int breakTimeLeft) {
		this.breakTimeLeft = breakTimeLeft;
	}

	public void startCountdown(int length) {
		breakTimeLeft = length;
		showCountdown();
		while (breakTimeLeft > 0) {

			System.out.println("Timeleft: " + breakTimeLeft);
			setBreakTimeLeft(breakTimeLeft);

			try {
				Thread.sleep(100);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			repaint();
			try {
				Thread.sleep(900);
			} catch (InterruptedException e) {
				// TODO Auto-generated catch block
				e.printStackTrace();
			}
			breakTimeLeft--;
		}
		hideCountdown();
	}

	/**
	 * Handles spacebar presses
	 */
	public void spaceKey() {
		if (state == TRIAL_BREAK) {
			breakTimeLeft = 2;
			System.out.println("Space pressed");
		}
	}
}
