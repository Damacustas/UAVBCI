package com.ClassifierTraining;

import java.io.IOException;

public class RunClassifierTrainingPractise {

	public static void main(String[] args) throws IOException {

		Screen s = new Screen();
		String[] classes = {"music", "house"};
		Trainer tr = new Trainer(s, classes);
		
		tr.setTotalTrials(20);
		tr.setLongBreakTrials(2);
		tr.setShortBreakTrials(1);
		try {
			tr.startClassifierTraining();
		} catch (Exception e) {
			e.printStackTrace();
		}

	}
}