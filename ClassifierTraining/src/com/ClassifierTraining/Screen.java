package com.ClassifierTraining;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Toolkit;
import java.awt.image.BufferedImage;
import java.io.File;
import java.io.IOException;

import javax.imageio.ImageIO;
import javax.swing.*;

@SuppressWarnings("serial")
public class Screen extends JPanel {

	private static final int CROSS_SIZE = 50;
	private static final int CUE_ICON_HEIGHT = 100;
	private static final int CUE_ICON_WIDTH = 100;
	public static final int TRIAL_EMPTY = 0;
	public static final int TRIAL_START = 1;
	public static final int TRIAL_CUE = 2;

	private JFrame frame;
	private Dimension dim = null;
	private int state = 0;
	private String cue;

	public void setCue(String cue) {
		this.cue = cue;
	}

	public Screen() {
		frame = new JFrame();
		frame.setTitle("UAV BCI Software");
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setExtendedState(JFrame.MAXIMIZED_BOTH);
		this.setBackground(Color.WHITE);
		frame.add(this);
		frame.setVisible(true);
		dim = Toolkit.getDefaultToolkit().getScreenSize();

	}

	public void paintComponent(Graphics g) {
		super.paintComponent(g);

		if (state == TRIAL_START) {
			drawFixationCross(g);
			// playBeep();
		} else if (state == TRIAL_CUE) {
			drawFixationCross(g);
			drawCueImage(g, cue);
		}
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

		g.drawImage(img, dim.width / 2 - CUE_ICON_WIDTH / 2, 0, CUE_ICON_WIDTH,
				CUE_ICON_HEIGHT, Color.white, null);

	}

	public void setState(int state) {
		this.state = state;
		//should probably post a bufferevent or sometihng
		repaint();
	}

	private void playBeep() {
		// TODO Find better beep
		Toolkit.getDefaultToolkit().beep();
	}

}
