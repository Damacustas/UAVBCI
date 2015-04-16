package com.ClassifierTraining;

import java.io.IOException;

public class RunClassifierTrainingIM {
	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"IMLeft", "IMRight"};
		Trainer tr = new Trainer(s, classes);
		try {
			tr.startClassifierTraining();
		} catch (IOException e) {
			e.printStackTrace();
		}

	}
}
