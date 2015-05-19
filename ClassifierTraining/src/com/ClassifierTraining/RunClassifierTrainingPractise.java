package com.ClassifierTraining;

import java.io.IOException;

public class RunClassifierTrainingPractise {

	public static void main(String[] args) throws IOException {

		Screen s = new Screen();
		String[] classes = {"music", "house"};
		Trainer tr = new Trainer(s, classes);
		
		tr.setTotalTrials(10);
		tr.setLongBreakTrials(5);
		tr.setShortBreakTrials(10);
		try {
			tr.startClassifierTraining();
		} catch (Exception e) {
			e.printStackTrace();
		}

	}
}