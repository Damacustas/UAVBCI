using System;
using UAV.Common;
using System.Collections.Generic;

namespace UAV.Simulation
{
	class MainClass
	{
		public static void Main (string[] args)
		{
            Simulation sim = new Simulation();
            sim.IntelligenceFactor = 0.4;
            sim.InputGenerator = new InputGenerator(0.5);
            sim.MaxEpochs = 500;
            sim.StartLocation = new Vector2D(0, 0);
            sim.State = new WorldState();
            sim.MaxTargetDeviation = 3.0f;
            sim.Targets = new List<Vector2D>() { new Vector2D(100, 100) };


            sim.Intelligence = new FitIntelligence(10, 2);
            //sim.Intelligence = new NoIngelligence();

            sim.Run();

            if (sim.IsFinished())
            {
                Console.WriteLine("Finished path. :D");
            }
            else
            {
                Console.WriteLine("Didn't finish path. :(");
            }
		}
	}
}
