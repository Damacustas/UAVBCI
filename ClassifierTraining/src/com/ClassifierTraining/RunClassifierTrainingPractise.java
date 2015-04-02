package com.ClassifierTraining;

public class RunClassifierTrainingPractise {

	public static void main(String[] args) {

		Screen s = new Screen();
		Trainer tr = new TwoClassTrainer(s);
		((TwoClassTrainer) tr).setTotalTrials(20);
		((TwoClassTrainer) tr).setLongBreakTrials(2);
		((TwoClassTrainer) tr).setShortBreakTrials(1);
		tr.startClassifierTraining();

	}
}