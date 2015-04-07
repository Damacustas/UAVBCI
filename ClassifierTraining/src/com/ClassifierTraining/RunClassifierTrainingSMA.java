package com.ClassifierTraining;

public class RunClassifierTrainingSMA {
	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"SMALeft", "SMARight"};
		Trainer tr = new Trainer(s, classes);
		tr.startClassifierTraining();

	}
}
