package com.ClassifierTraining;

import java.util.ArrayList;
import java.util.Collections;
import java.util.Iterator;

public class Run {

	public static void main(String[] args) throws InterruptedException {
		// TODO Auto-generated method stub
		Screen scre = new Screen();
		
		ArrayList<String> cues = new ArrayList<String>();
		
		cues.add("music");
		cues.add("music");
		cues.add("house");
		cues.add("house");
		cues.add("music");
		cues.add("music");
		cues.add("house");
		cues.add("house");
		
		
		Collections.shuffle(cues);
		Iterator<String> it = cues.iterator();
		int trialcounter = 0;
		//Loop through all trials
		while(it.hasNext()){
			
			Thread.sleep(1000);
			scre.setState(Screen.TRIAL_START);
			Thread.sleep(1000);
			scre.setCue(it.next());
			scre.setState(Screen.TRIAL_CUE);
			Thread.sleep(4000);
			scre.setState(Screen.TRIAL_EMPTY);
			Thread.sleep(1500);
			
			//break every 5th
			if (++trialcounter % 2 == 0){
				System.out.println(" Breaktime!");
			}
			
		}
		
		
		
		//scre.setCue("house");
		//scre.setState(Screen.TRIAL_CUE);
	}

}
