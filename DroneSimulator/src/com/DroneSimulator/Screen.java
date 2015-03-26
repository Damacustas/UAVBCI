package com.DroneSimulator;

import java.awt.Color;
import java.awt.Dimension;
import java.awt.Graphics;
import java.awt.Toolkit;
import java.awt.event.KeyEvent;
import java.awt.event.KeyListener;
import java.awt.event.WindowEvent;

import javax.swing.*;

public class Screen extends JPanel implements KeyListener{
	
	private JFrame frame;
	private Dimension dim = null;
	private SimulationRun simRun = null;
	private Simulation simulation = null;
	
	public Screen(Simulation sim)
	{
		this.simulation = sim;
		this.simRun = this.simulation.generateNext();
		
		frame = new JFrame();
		frame.setTitle("UAV BCI Software");
		frame.setExtendedState(JFrame.MAXIMIZED_BOTH);
		frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
		this.setBackground(Color.WHITE);
		frame.add(this);
		frame.setVisible(true);	
		dim = Toolkit.getDefaultToolkit().getScreenSize();
		
		this.addKeyListener(this);
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

	@Override
	public void keyReleased(KeyEvent arg0) {}

	@Override
	public void keyPressed(KeyEvent arg0)
	{
		if(arg0.getKeyCode() == KeyEvent.VK_ENTER)
		{
			this.simRun = this.simulation.generateNext();
			this.repaint();
		}
		else if(arg0.getKeyCode() == KeyEvent.VK_ESCAPE)
		{
			this.frame.dispatchEvent(new WindowEvent(frame, WindowEvent.WINDOW_CLOSING));
		}
	}

	@Override
	public void keyTyped(KeyEvent arg0) {}
}

