package com.ClassifierTraining;

public class RunClassifierTrainingPractise {

	public static void main(String[] args) {

		Screen s = new Screen();
		Trainer tr = new Trainer(s);
		
		tr.setTotalTrials(20);
		tr.setLongBreakTrials(2);
		tr.setShortBreakTrials(1);
		tr.startClassifierTraining();

	}
}