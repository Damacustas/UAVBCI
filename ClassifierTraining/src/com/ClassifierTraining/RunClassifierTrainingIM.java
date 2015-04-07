package com.ClassifierTraining;

public class RunClassifierTrainingIM {
	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"IMLeft", "IMRight"};
		Trainer tr = new Trainer(s, classes);
		tr.startClassifierTraining();

	}
}
