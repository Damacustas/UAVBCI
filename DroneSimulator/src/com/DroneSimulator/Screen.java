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
	
	public Screen(){
		frame = new JFrame();
		frame.setTitle("UAV BCI Software");
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		frame.setExtendedState(JFrame.MAXIMIZED_BOTH);
		this.setBackground(Color.WHITE);
		frame.add(this);
		frame.setVisible(true);	
		dim = Toolkit.getDefaultToolkit().getScreenSize();
	}
	
	@Override
	public void paint(Graphics g) 
    {
        super.paint(g);
		Target target = new Target(); 
        int height = target.getHeight();
        int width = target.getWidth();
        double angle = (int) Math.ceil(Math.random()*360);
        g.setColor(Color.RED);
		g.fillRect(dim.width/2 - width/2, dim.height/2 - height/2, width, height);
		g.setColor(Color.BLACK);
		g.fillOval((int) Math.floor(dim.width/2 + 200*Math.cos(angle)), (int) Math.floor(dim.height/2 + 200*Math.sin(angle)), 10, 10);
		
    }
	
}

