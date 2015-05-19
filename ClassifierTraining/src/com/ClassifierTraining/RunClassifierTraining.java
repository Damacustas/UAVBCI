package com.ClassifierTraining;

import java.io.IOException;

public class RunClassifierTraining {

	public static void main(String[] args) {

		Screen s = new Screen();
		String[] classes = {"music", "house"};
		Trainer tr = new Trainer(s, classes);
		tr.setTotalTrials(80);
		tr.setLongBreakTrials(40);
		tr.setShortBreakTrials(5);
		try {
			tr.startClassifierTraining();
		} catch (IOException e) {
			e.printStackTrace();
		}

	}
}
