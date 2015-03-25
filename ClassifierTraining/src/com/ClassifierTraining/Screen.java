package com.ClassifierTraining;

import java.awt.BasicStroke;
import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Graphics2D;
import java.awt.Image;
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
	private JFrame frame;
	private Dimension dim = null;
	//comments zijn een beetje overrated
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

	public void drawFixationCross(Graphics g) {
		Graphics2D g2 = (Graphics2D) g;
		g2.setStroke(new BasicStroke(3));
		g2.drawLine(dim.width / 2 - CROSS_SIZE / 2, dim.height / 2, dim.width / 2 + CROSS_SIZE / 2,
				dim.height / 2);
		g2.drawLine(dim.width / 2, dim.height / 2 - CROSS_SIZE / 2, dim.width / 2,
				dim.height / 2 + CROSS_SIZE / 2);
	}

	public void drawCueImage(Graphics g, String cue) {
		BufferedImage img = null;
		if (cue.equalsIgnoreCase("house")) {
			try {
				img = ImageIO.read(new File("Images/house.png"));
			} catch (IOException ex) {
				
			}
		} else if (cue.equalsIgnoreCase("music"))
			try {
				img = ImageIO.read(new File("Images/music.jpg"));
			} catch (IOException ex) {

			}

		g.drawImage(img, dim.width / 2 - CUE_ICON_WIDTH / 2, 0, CUE_ICON_WIDTH, CUE_ICON_HEIGHT,
				Color.white, null);
		System.out.println(img.getHeight());
		System.out.println(dim.width);

	}

	public void paintComponent(Graphics g) {
		super.paintComponent(g);
		drawFixationCross(g);
		drawCueImage(g, "music");

	}


}
