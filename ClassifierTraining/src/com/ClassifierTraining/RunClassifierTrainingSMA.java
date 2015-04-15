package com.ClassifierTraining;

import java.io.IOException;

public class RunClassifierTrainingSMA {
	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"SMALeft", "SMARight"};
		Trainer tr = new Trainer(s, classes);
		try {
			tr.startClassifierTraining();
		} catch (IOException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}

	}
}
