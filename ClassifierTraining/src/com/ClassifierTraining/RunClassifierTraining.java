package com.ClassifierTraining;

public class RunClassifierTraining {

	public static void main(String[] args) {

		Screen s = new Screen();
		Trainer tr = new Trainer(s);
		tr.startClassifierTraining();

	}
}
