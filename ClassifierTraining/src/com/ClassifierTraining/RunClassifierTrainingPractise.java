package com.ClassifierTraining;

public class RunClassifierTrainingPractise {

	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"music", "house"};
		Trainer tr = new Trainer(s, classes);
		
		tr.setTotalTrials(20);
		tr.setLongBreakTrials(2);
		tr.setShortBreakTrials(1);
		tr.startClassifierTraining();

	}
}