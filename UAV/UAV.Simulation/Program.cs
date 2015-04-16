using System;
using UAV.Common;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

namespace UAV.Simulation
{
	class MainClass
	{
        static int passed, total, numLen;
        static DateTime start, prev;
        static readonly int numSims = 500;
        static List<Simulation> results;

        // Different parameters
        static double[] IntelligenceFactors = new double[]{ 0.25, 0.50, 0.75 };
        static double[] InputAccuracies = new double[]{0.5, 0.6, 0.7};
        static int[] historyLenghts = new int[] { 5, 10, 15 };
        static int[] fitDegrees = new int[] { 2, 3, 4 };

		public static void Main (string[] args)
        {
            //RunSimulationsForNoIntelligence();
            RunSimulationsForFitIntelligence();
        }

        /*
        private static void RunAllSimulations()
        {
            // Delete previous results.
            if (Directory.Exists(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output")))
            {
                Directory.Delete(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Output"), true);
            }

            passed = 0;
            total = InputAccuracies.Length * IntelligenceFactors.Length * historyLenghts.Length * fitDegrees.Length * numSims
                + InputAccuracies.Length * IntelligenceFactors.Length * numSims;

            numLen = total.ToString().Length;

            results = new List<Simulation>(total);

            start = DateTime.UtcNow;
            prev = start;

            // Loop through all the parameters.
            foreach (double intelligenceFactor in IntelligenceFactors)
            {
                foreach (double noiseFactor in InputAccuracies)
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
        */

        private static void RunSimulationsForFitIntelligence()
        {
            int x = IntelligenceFactors.Length * InputAccuracies.Length * historyLenghts.Length * fitDegrees.Length;
            int y = 1;

            // Loop through all the parameters.
            foreach (double intelligenceFactor in IntelligenceFactors)
            {
                foreach (double inputAccuracy in InputAccuracies)
                {
                    // Run for FitIntelligence
                    foreach (int historyLength in historyLenghts)
                    {
                        foreach (int fitDegree in fitDegrees)
                        {
                            List<double> timeToCompletions = new List<double>();

                            for (int n = 0; n < numSims; n++)
                            {
                                Simulation sim = GenerateSimulationFitIntelligence(intelligenceFactor, inputAccuracy, historyLength, fitDegree);
                                sim.Run(verbose: false);
                                timeToCompletions.Add(sim.CurrentEpoch);
                            }

                            // Analyse
                            double avg = timeToCompletions.Aggregate(0.0d, (left, right) => left + right) / timeToCompletions.Count;
                            double sd = (from ttc in timeToCompletions
                                select Math.Pow(ttc - avg, 2)).Aggregate(0.0d, (left, right) => left + right);
                            sd *= (1.0 / timeToCompletions.Count);
                            sd = Math.Sqrt(sd);

                            // Report results:
                            Console.WriteLine("({8}/{9}) IF={0}, IA={1}, HL={2}, FD={3}: avg={4}, sd={5}, hits={6}, max={7}",
                                intelligenceFactor.ToString("0.00"),
                                inputAccuracy.ToString("0.00"),
                                historyLength.ToString("##"),
                                fitDegree,
                                avg.ToString("###.00").PadLeft(6),
                                sd.ToString("##0.00").PadLeft(6),
                                timeToCompletions.Count(ttc => ttc < 500),
                                timeToCompletions.Aggregate(0.0d, (left, right) => left > right ? left : right),
                                y++,
                                x
                            );
                        }
                    }
                }
            }
        }

        private static void RunSimulationsForNoIntelligence()
        {
            // Loop through all the parameters.
            foreach (double intelligenceFactor in IntelligenceFactors)
            {
                foreach (double inputAccuraccy in InputAccuracies)
                {
                    List<double> timeToCompletions = new List<double>();

                    // Run for NoIntelligence
                    for (int n = 0; n < numSims; n++)
                    {
                        Simulation sim = GenerateSimulationNoIntelligence(intelligenceFactor, inputAccuraccy);
                        sim.Run(verbose: false);
                        timeToCompletions.Add(sim.CurrentEpoch);
                    }

                    // Analyse
                    double avg = timeToCompletions.Aggregate(0.0d, (left, right) => left + right) / timeToCompletions.Count;
                    double sd = (from ttc in timeToCompletions
                                                select Math.Pow(ttc - avg, 2)).Aggregate(0.0d, (left, right) => left + right);
                    sd *= (1.0 / timeToCompletions.Count);
                    sd = Math.Sqrt(sd);

                    // Report results:
                    Console.WriteLine("IF={0}, IA={1}: avg={2}, sd={3}, hits={4}, max={5}",
                        intelligenceFactor.ToString("0.00"),
                        inputAccuraccy.ToString("0.00"),
                        avg.ToString("###.00").PadLeft(6),
                        sd.ToString("##0.00").PadLeft(6),
                        timeToCompletions.Count(ttc => ttc < 500),
                        timeToCompletions.Aggregate(0.0d, (left, right) => left > right ? left : right)
                    );
                }
            }
        }

        private static Simulation GenerateSimulationNoIntelligence(double intelligenceFactor, double inputAccuracy)
        {
            Simulation sim = GenerateBaseSimulation(intelligenceFactor, inputAccuracy);

            sim.Intelligence = new NoIngelligence();

            return sim;
        }

        private static Simulation GenerateSimulationFitIntelligence(double intelligenceFactor, double inputAccuracy, int historyLength, int fitDegree)
        {
            Simulation sim = GenerateBaseSimulation(intelligenceFactor, inputAccuracy);

            sim.Intelligence = new FitIntelligence(historyLength, fitDegree);

            return sim;
        }

        private static Simulation GenerateBaseSimulation(double IF, double inputAccuracy)
        {
            Simulation sim = new Simulation();
            sim.InputGenerator = new InputGenerator(inputAccuracy);
            sim.MaxEpochs = 500;
            sim.IntelligenceFactor = IF;
            sim.StartLocation = new Vector2D();
            sim.MaxTargetDeviation = 0.5f;
            sim.Targets = new List<Vector2D>() { new Vector2D(100, 100) };

            return sim;
        }

//
//        private static void PrintProgress()
//        {
//            // Progress report.
//            passed++;
//            if (passed % 25 == 0)
//            {
//                var now = DateTime.UtcNow;
//
//                double msLeft = (now - start).TotalMilliseconds;
//                msLeft /= (double)passed;
//                msLeft *= (total - passed);
//
//                Console.WriteLine("{0}/{1} ({2}%), delta: {3} ms, total: {4}, esimated time left: {5}",
//                    passed.ToString().PadLeft(numLen),
//                    total,
//                    (((double)passed/(double)total)*100).ToString("##.0"),
//                    (now - prev).TotalMilliseconds.ToString("####.0"),
//                    (now - start).ToString(@"hh\:mm\:ss"),
//                    new TimeSpan(0, 0, 0, 0, (int)msLeft).ToString(@"hh\:mm\:ss")
//                );
//
//                prev = now;
//            }
//        }
//
//        /// <summary>
//        /// Writes the simulation results to a file.
//        /// </summary>
//        /// <param name="sim">The simulation to write.</param>
//        /// <param name="n">The n-th simulation under the same simulation parameters.</param>
//        private static void WriteSimulationResults(Simulation sim, int n)
//        {
//            /*
//            string[] strings =
//                {
//                    string.Format("noise_{0}", sim.InputGenerator.NoiseRatio),
//                    string.Format("intelligenceFactor_{0}", sim.IntelligenceFactor),
//                    sim.Intelligence.IntelligenceName
//                };
//
//            var json = JsonConvert.SerializeObject(sim);
//            var filename = string.Join("-", strings.Concat(new string[] {(n+1).ToString()})) + ".json";
//            //var dir = EnsureDirectory(new string[] { "Output" }.Concat(strings).ToArray());
//            EnsureDirectory("Output");
//
//            var p = Path.Combine("Output", filename);
//            p = Path.GetFullPath(p);
//            using (StreamWriter writer = new StreamWriter(File.OpenWrite(p)))
//            {
//                writer.Write(json);
//                writer.Close();
//            }
//            */
//            Console.WriteLine(JsonConvert.SerializeObject(sim));
//        }
//
//        /// <summary>
//        /// Ensures the directory exists.
//        /// </summary>
//        /// <returns>The directory.</returns>
//        /// <param name="path">Path.</param>
//        private static string EnsureDirectory(params string[] path)
//        {
//            string fullpath = AppDomain.CurrentDomain.BaseDirectory;
//
//            foreach (string p in path)
//            {
//                fullpath = Path.Combine(fullpath, p);
//
//                if (!Directory.Exists(fullpath))
//                {
//                    Directory.CreateDirectory(fullpath);
//                }
//            }
//
//            return fullpath;
//        }
	}
}
