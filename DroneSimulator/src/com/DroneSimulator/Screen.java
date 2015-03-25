package com.DroneSimulator;

import java.awt.Color;
import java.awt.Component;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Toolkit;

import javax.swing.*;

public class Screen extends JPanel{
	
	private JFrame frame;
	private Dimension dim = null;
	private SimulationRun simRun;
	
	public Screen(SimulationRun simRun){
		frame = new JFrame();
		frame.setTitle("UAV BCI Software");
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setExtendedState(JFrame.MAXIMIZED_BOTH);
		this.setBackground(Color.WHITE);
		frame.add(this);
		frame.setVisible(true);	
		dim = Toolkit.getDefaultToolkit().getScreenSize();
		this.simRun = simRun;
		
	}
	
	@Override
	public void paint(Graphics g) 
    {
        super.paint(g);
		int height = (int) simRun.getTargetHeight();
        int width = (int) simRun.getTargetWidth();
        int startX = (int) simRun.getStartX();
        int startY = (int) simRun.getStartY();
        g.setColor(Color.RED);
		g.fillRect(dim.width/2 - width/2, dim.height/2 - height/2, width, height);
		g.setColor(Color.BLACK);
		g.fillOval(startX, startY, 10, 10);
		
    }
	
}

