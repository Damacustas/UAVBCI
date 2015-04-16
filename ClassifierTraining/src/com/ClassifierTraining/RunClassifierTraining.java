package com.ClassifierTraining;

import java.io.IOException;

public class RunClassifierTraining {

	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"music", "house"};
		Trainer tr = new Trainer(s, classes);
		try {
			tr.startClassifierTraining();
		} catch (IOException e) {
			e.printStackTrace();
		}

	}
}
