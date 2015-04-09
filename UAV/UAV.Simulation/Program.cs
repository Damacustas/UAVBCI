using System;
using UAV.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.IO.Compression;

namespace UAV.Simulation
{
	class MainClass
	{
        static int passed, total, numLen;
        static DateTime start, prev;
        static readonly int numSims = 20;
        static List<Simulation> results;

		public static void Main (string[] args)
		{
            // Delete previous results.
            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output")))
            {
                Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output"), true);
            }

            // Different parameters
            double[] intelligenceFactors = new double[]{ 0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9, 1.0 };
            double[] noiseFactors = new double[]{0, 0.1, 0.2, 0.3, 0.4, 0.5, 0.6, 0.7, 0.8, 0.9};
            int[] historyLenghts = new int[] { 2, 5, 10, 15, 20 };
            int[] fitDegrees = new int[] { 1, 2, 3, 4 };

            passed = 0;
            total = noiseFactors.Length * intelligenceFactors.Length * historyLenghts.Length * fitDegrees.Length * numSims
                + noiseFactors.Length * intelligenceFactors.Length * numSims;

            numLen = total.ToString().Length;

            results = new List<Simulation>(total);

            start = DateTime.UtcNow;
            prev = start;

            // Loop through all the parameters.
            foreach (double intelligenceFactor in intelligenceFactors)
            {
                foreach (double noiseFactor in noiseFactors)
                {
                    // Run for NoIntelligence
                    for (int n = 0; n < numSims; n++)
                    {
                        Simulation sim = GenerateSimulationNoIntelligence(intelligenceFactor, noiseFactor);

                        sim.Run(verbose: false);

                        //WriteSimulationResults(sim, n);
                        results.Add(sim);
                        PrintProgress();
                    }

                    // Run for FitIntelligence
                    foreach (int historyLength in historyLenghts)
                    {
                        foreach (int fitDegree in fitDegrees)
                        {
                            for (int n = 0; n < numSims; n++)
                            {
                                Simulation sim = GenerateSimulationFitIntelligence(intelligenceFactor, noiseFactor, historyLength, fitDegree);

                                sim.Run(verbose: false);

                                //WriteSimulationResults(sim, n);
                                results.Add(sim);
                                PrintProgress();
                            }
                        }
                    }
                }
            }

            DateTime end = DateTime.UtcNow;
            var ms = (end - start).TotalMilliseconds;

            Console.WriteLine();
            Console.WriteLine("Ran {0} simulations in {1} ({2} ms).", passed, (end-start).ToString(@"hh\:mm\:ss"), ms);

            EnsureDirectory("Output");
            using (StreamWriter writer = new StreamWriter(Path.Combine("Output", "output.json")))
            {
                using (JsonWriter jsonWriter = new JsonTextWriter(writer))
                {
                    var serializer = new JsonSerializer();
                    serializer.Serialize(jsonWriter, results);
                }
            }
		}

        private static Simulation GenerateSimulationNoIntelligence(double intelligenceFactor, double noiseFactor)
        {
            Simulation sim = new Simulation();
            sim.InputGenerator = new InputGenerator(noiseFactor);
            sim.MaxEpochs = 500;
            sim.IntelligenceFactor = intelligenceFactor;
            sim.StartLocation = new Vector2D();
            sim.MaxTargetDeviation = 3.0f;
            sim.Targets = new List<Vector2D>() { new Vector2D(100, 100) };

            sim.Intelligence = new NoIngelligence();

            return sim;
        }

        private static Simulation GenerateSimulationFitIntelligence(double intelligenceFactor, double noiseFactor, int historyLength, int fitDegree)
        {
            Simulation sim = new Simulation();
            sim.InputGenerator = new InputGenerator(noiseFactor);
            sim.MaxEpochs = 500;
            sim.IntelligenceFactor = intelligenceFactor;
            sim.StartLocation = new Vector2D();
            sim.MaxTargetDeviation = 3.0f;
            sim.Targets = new List<Vector2D>() { new Vector2D(100, 100) };

            sim.Intelligence = new FitIntelligence(historyLength, fitDegree);

            return sim;
        }

        private static void PrintProgress()
        {
            // Progress report.
            passed++;
            if (passed % 25 == 0)
            {
                var now = DateTime.UtcNow;

                double msLeft = (now - start).TotalMilliseconds;
                msLeft /= (double)passed;
                msLeft *= (total - passed);

                Console.WriteLine("{0}/{1} ({2}%), delta: {3} ms, total: {4}, esimated time left: {5}",
                    passed.ToString().PadLeft(numLen),
                    total,
                    (((double)passed/(double)total)*100).ToString("##.0"),
                    (now - prev).TotalMilliseconds.ToString("####.0"),
                    (now - start).ToString(@"hh\:mm\:ss"),
                    new TimeSpan(0, 0, 0, 0, (int)msLeft).ToString(@"hh\:mm\:ss")
                );

                prev = now;
            }
        }

        /// <summary>
        /// Writes the simulation results to a file.
        /// </summary>
        /// <param name="sim">The simulation to write.</param>
        /// <param name="n">The n-th simulation under the same simulation parameters.</param>
        private static void WriteSimulationResults(Simulation sim, int n)
        {
            /*
            string[] strings =
                {
                    string.Format("noise_{0}", sim.InputGenerator.NoiseRatio),
                    string.Format("intelligenceFactor_{0}", sim.IntelligenceFactor),
                    sim.Intelligence.IntelligenceName
                };

            var json = JsonConvert.SerializeObject(sim);
            var filename = string.Join("-", strings.Concat(new string[] {(n+1).ToString()})) + ".json";
            //var dir = EnsureDirectory(new string[] { "Output" }.Concat(strings).ToArray());
            EnsureDirectory("Output");

            var p = Path.Combine("Output", filename);
            p = Path.GetFullPath(p);
            using (StreamWriter writer = new StreamWriter(File.OpenWrite(p)))
            {
                writer.Write(json);
                writer.Close();
            }
            */
            Console.WriteLine(JsonConvert.SerializeObject(sim));
        }

        /// <summary>
        /// Ensures the directory exists.
        /// </summary>
        /// <returns>The directory.</returns>
        /// <param name="path">Path.</param>
        private static string EnsureDirectory(params string[] path)
        {
            string fullpath = AppDomain.CurrentDomain.BaseDirectory;

            foreach (string p in path)
            {
                fullpath = Path.Combine(fullpath, p);

                if (!Directory.Exists(fullpath))
                {
                    Directory.CreateDirectory(fullpath);
                }
            }

            return fullpath;
        }
	}
}
