package com.ClassifierTraining;

public class RunClassifierTraining {

	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"music", "house"};
		Trainer tr = new Trainer(s, classes);
		tr.startClassifierTraining();

	}
}
