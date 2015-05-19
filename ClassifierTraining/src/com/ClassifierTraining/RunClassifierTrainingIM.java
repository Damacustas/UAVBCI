package com.ClassifierTraining;

import java.io.IOException;

public class RunClassifierTrainingIM {
	
	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"IMLeft", "IMRight"};
		Trainer tr = new Trainer(s, classes);
		tr.setTotalTrials(20);
		tr.setLongBreakTrials(20);
		tr.setShortBreakTrials(5);
		try {
			tr.startClassifierTraining();
		} catch (IOException e) {
			e.printStackTrace();
		}

	}
}
