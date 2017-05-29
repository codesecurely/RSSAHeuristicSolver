using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class ResultHelper
    {
        public string ScenarioName;
        public double AvgEnergy;
        public double AvgTime;
        public double AvgSum;
        public double AvgAvg;
        public string Label;

        public ResultHelper(string name, string label, double energy, double time, double sum, double avg)
        {
            ScenarioName = name;
            AvgEnergy = energy;
            AvgTime = time;
            AvgSum = sum;
            AvgAvg = avg;
            Label = label;
        }
    }

    class Tester
    {
        public void PerformTests(string topologyPathFile, string topology, int repeat, double t, double a, string[] s,
            string[] l, string[] o, string xaxis)
        {
            string dir = topologyPathFile; //"C:\\EURO16_30Tbps_Avg";
            double initialTemperature = t;
            double alpha = a;
            double endTemperature = 0.001;
            string[] scenarioFiles = s;
            string[] labels = l;
            string[] outputFiles = o;
            List<ResultHelper> greedyResultHelper = new List<ResultHelper>();
            List<ResultHelper> SAResultHelper = new List<ResultHelper>();
            List<string> avgsGreedyToFile = new List<string>();
            List<string> avgsSAToFile = new List<string>();
            for (int index = 0; index < scenarioFiles.Length; index++)
            {
                System.IO.StreamWriter file = new System.IO.StreamWriter(topologyPathFile + outputFiles[index]);
                string fileHeader = "Name, avgEnergy, avgSum, avgAvg, avgTime";
                file.WriteLine(fileHeader);

                var list = scenarioFiles[index];
                SimulatedAnnealing SA = new SimulatedAnnealing();
                GreedyHeuristic greedy = new GreedyHeuristic();
                Parser parser = new Parser();
                parser.addScenarios(dir, list);
                double avgEnergy = 0.0, avgTime = 0.0, avgSum = 0.0, avgAvg = 0.0;
                List<string> greedyResults = new List<string>();
                List<string> saResults = new List<string>();
                double[] avgsGreedy = {0.0, 0.0, 0.0, 0.0}; //avgEnergy, avgSum, avgAvg, avgTime
                double[] avgsSA = {0.0, 0.0, 0.0, 0.0};
                foreach (var scenario in parser.ScenarioList)
                {
                    avgEnergy = 0.0;
                    avgTime = 0.0;
                    avgSum = 0.0;
                    avgAvg = 0.0;
                    var comp = new DemandDistanceSorter();
                    greedy.Start(scenario, comp);
                    int n = repeat;
                    string g = scenario.Name + " Fmin: " + scenario.ObjectiveFunctionResult + " Sum: " +
                               scenario.SumOfAllSlices + " Avg: " +
                               scenario.AverageSliceCountForEachPathAndSpRc.ToString("F2") + " Time:" +
                               scenario.ElapsedAlgorithmTime;

                    avgsGreedy[0] += scenario.ObjectiveFunctionResult;
                    avgsGreedy[1] += scenario.ElapsedAlgorithmTime;
                    avgsGreedy[2] += scenario.SumOfAllSlices;
                    avgsGreedy[3] += scenario.AverageSliceCountForEachPathAndSpRc;
                    greedyResults.Add("G\t" + scenario.Name + "\t" + scenario.ObjectiveFunctionResult.ToString("F2") +
                                      "\t" + scenario.SumOfAllSlices.ToString("F2") + "\t" +
                                      scenario.AverageSliceCountForEachPathAndSpRc.ToString("F2") + "\t" +
                                      scenario.ElapsedAlgorithmTime.ToString("F2"));
                    Console.WriteLine("{0,0}{1,10}", "Greedy: ", g);
                    for (int i = 0; i < n; i++)
                    {
                        SA.Start(initialTemperature, alpha, endTemperature, scenario, true);
                        avgEnergy += scenario.ObjectiveFunctionResult;
                        avgTime += scenario.ElapsedAlgorithmTime;
                        avgSum += scenario.SumOfAllSlices;
                        avgAvg += scenario.AverageSliceCountForEachPathAndSpRc;
                    }
                    avgEnergy /= n;
                    avgTime /= n;
                    avgSum /= n;
                    avgAvg /= n;

                    avgsSA[0] += avgEnergy;
                    avgsSA[1] += avgTime;
                    avgsSA[2] += avgSum;
                    avgsSA[3] += avgAvg;

                    string fileResult = scenario.Name + "\t" + avgEnergy.ToString("F2") + "\t" + avgSum.ToString("F2") +
                                        "\t" + avgAvg.ToString("F2") + "\t" +
                                        avgTime.ToString("F2");

                    string sa = scenario.Name + " Fmin: " + avgEnergy + " Sum: " +
                                avgSum + " Avg: " +
                                avgAvg + " Time:" +
                                avgTime;
                    saResults.Add("S\t" + fileResult);
                    Console.WriteLine("{0,0}{1,15}", "SA: ", sa);
                    Console.WriteLine("----------");
                }
                int div = parser.ScenarioList.Count;
                greedyResultHelper.Add(new ResultHelper(list, labels[index], avgsGreedy[0]/div, avgsGreedy[1]/div,
                    avgsGreedy[2]/div, avgsGreedy[3]/div));
                SAResultHelper.Add(new ResultHelper(list, labels[index], avgsSA[0]/div, avgsSA[1]/div, avgsSA[2]/div,
                    avgsSA[3]/div));
                file.Close();
                string greedyavgs = avgsGreedy[0]/div + "\t" + avgsGreedy[1]/div + "\t" + avgsGreedy[2]/div + "\t" +
                                    avgsGreedy[3]/div;
                string SAavgs = avgsSA[0]/div + "\t" + avgsSA[1]/div + "\t" + avgsSA[2]/div + "\t" +
                                avgsSA[3]/div;
                File.AppendAllLines(topologyPathFile + outputFiles[index], greedyResults);
                File.AppendAllLines(topologyPathFile + outputFiles[index], saResults);
                File.AppendAllText(topologyPathFile + outputFiles[index], "Gavg: " + greedyavgs + "\n");
                File.AppendAllText(topologyPathFile + outputFiles[index], "SAavg: " + SAavgs + "\n");
                avgsGreedyToFile.Add(greedyavgs);
                avgsSAToFile.Add(SAavgs);
            }
            File.WriteAllLines(topologyPathFile + "avggreedy" + xaxis, avgsGreedyToFile);
            File.WriteAllLines(topologyPathFile + "avgSA" + xaxis, avgsSAToFile);
            ChartGenerator c = new ChartGenerator();
            c.GenerateChart(SAResultHelper, greedyResultHelper, topology + "energy", "energy", xaxis);
            c.GenerateChart(SAResultHelper, greedyResultHelper, topology + "avg", "avg", xaxis);
            c.GenerateChart(SAResultHelper, greedyResultHelper, topology + "sum", "sum", xaxis);
        }

        /*public void TestForK(string topologyPathFile, string topology, int repeat, double t, double a, string[] s, string[] l, string[] o)
        {
            string dir = topologyPathFile; //"C:\\EURO16_30Tbps_Avg";
            double initialTemperature = t;
            double alpha = a;
            double endTemperature = 0.001;
            string[] scenarioFiles = s;
            string[] outputFiles = o;
            string[] labels = l;
            List<ResultHelper> from50To1000GbpsGreedy = new List<ResultHelper>();
            List<ResultHelper> from50To1000GbpsSA = new List<ResultHelper>();
            SimulatedAnnealing SA = new SimulatedAnnealing();
            GreedyHeuristic greedy = new GreedyHeuristic();
            Parser parser = new Parser();
            parser.addScenarios(dir, scenarioFiles[0]);
            double avgEnergy = 0.0, avgTime = 0.0, avgSum = 0.0, avgAvg = 0.0;
            System.IO.StreamWriter file = new System.IO.StreamWriter(outputFiles[0]);
            string fileHeader = "Name, avgEnergy, avgSum, avgAvg, avgTime";
            file.WriteLine(fileHeader);
            foreach (var scenario in parser.ScenarioList)
            {
                avgEnergy = 0.0;
                avgTime = 0.0;
                avgSum = 0.0;
                avgAvg = 0.0;
                var comp = new DemandDistanceSorter();
                greedy.Start(scenario, comp);
                int n = 10;
                string g = scenario.Name + " Fmin: " + scenario.ObjectiveFunctionResult + " Sum: " +
                           scenario.SumOfAllSlices + " Avg: " +
                           scenario.AverageSliceCountForEachPathAndSpRc.ToString("F2") + " Time:" +
                           scenario.ElapsedAlgorithmTime;
                string result = "Greedy: " + g;
                ResultHelper resultGreedy = new ResultHelper(scenario.Name, "", scenario.ObjectiveFunctionResult,
                    scenario.ElapsedAlgorithmTime, scenario.SumOfAllSlices, scenario.AverageSliceCountForEachPathAndSpRc);
                resultGreedy.Label = scenario.K.ToString();
                from50To1000GbpsGreedy.Add(resultGreedy);
                Console.WriteLine("{0,0}{1,10}", "Greedy: ", g);
                for (int i = 0; i < n; i++)
                {
                    SA.Start(initialTemperature, alpha, endTemperature, scenario, true);
                    avgEnergy += scenario.ObjectiveFunctionResult;
                    avgTime += scenario.ElapsedAlgorithmTime;
                    avgSum += scenario.SumOfAllSlices;
                    avgAvg += scenario.AverageSliceCountForEachPathAndSpRc;
                }
                avgEnergy /= n;
                avgTime /= n;
                avgSum /= n;
                avgAvg /= n;

                string fileResult = scenario.Name + " " + avgEnergy + " " + avgSum + " " + avgAvg + " " +
                                    avgTime;
                file.WriteLine(fileResult);
                ResultHelper resultSA = new ResultHelper(scenario.Name, "", avgEnergy, avgTime, avgSum, avgAvg);
                resultSA.Label = scenario.K.ToString();
                from50To1000GbpsSA.Add(resultSA);
                string sa = scenario.Name + " Fmin: " + avgEnergy + " Sum: " +
                            avgSum + " Avg: " +
                            avgAvg + " Time:" +
                            avgTime;
                result = "SA: " + sa;
                Console.WriteLine("{0,0}{1,15}", "SA: ", sa);
                Console.WriteLine("----------");
            }
            file.Close();
            ChartGenerator c = new ChartGenerator();
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "energy_k", "energy",
                "Liczba ścieżek kandydujących");
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "avg_k", "avg", "Liczba ścieżek kandydujących");
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "sum_k", "sum", "Liczba ścieżek kandydujących");
        }
        */
        /*public void TestForSpRc()
        {
            string dir = "D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\Euro28"; //"C:\\EURO16_30Tbps_Avg";
            double initialTemperature = 1000.0;
            double alpha = 0.99;
            double endTemperature = 0.001;
            string[] scenarioFiles =
            {
                "euro28_diff_sprc.txt", "euro16_k3.txt", "euro16_k5.txt", "euro16_k10.txt",
                "euro16_k30.txt"
            };
            string[] outputFiles =
            {
                "c:\\euro28_diff_sprc.txt", "c:\\k3alpha09.txt", "c:\\k5alpha09.txt", "c:\\k10alpha09.txt",
                "c:\\k30alpha09.txt"
            };
            List<ResultHelper> from50To1000GbpsGreedy = new List<ResultHelper>();
            List<ResultHelper> from50To1000GbpsSA = new List<ResultHelper>();
            SimulatedAnnealing SA = new SimulatedAnnealing();
            GreedyHeuristic greedy = new GreedyHeuristic();
            Parser parser = new Parser();
            parser.addScenarios(dir, scenarioFiles[0]);
            double avgEnergy = 0.0, avgTime = 0.0, avgSum = 0.0, avgAvg = 0.0;
            System.IO.StreamWriter file = new System.IO.StreamWriter(outputFiles[0]);
            string fileHeader = "Name, avgEnergy, avgSum, avgAvg, avgTime";
            file.WriteLine(fileHeader);
            foreach (var scenario in parser.ScenarioList)
            {
                avgEnergy = 0.0;
                avgTime = 0.0;
                avgSum = 0.0;
                avgAvg = 0.0;
                var comp = new DemandDistanceSorter();
                greedy.Start(scenario, comp);
                int n = 10;
                string g = scenario.Name + " Fmin: " + scenario.ObjectiveFunctionResult + " Sum: " +
                           scenario.SumOfAllSlices + " Avg: " +
                           scenario.AverageSliceCountForEachPathAndSpRc.ToString("F2") + " Time:" +
                           scenario.ElapsedAlgorithmTime;
                string result = "Greedy: " + g;
                ResultHelper resultGreedy = new ResultHelper(scenario.Name, "", scenario.ObjectiveFunctionResult,
                    scenario.ElapsedAlgorithmTime, scenario.SumOfAllSlices, scenario.AverageSliceCountForEachPathAndSpRc);
                resultGreedy.Label = scenario.Name.Substring(11, 2);
                from50To1000GbpsGreedy.Add(resultGreedy);
                Console.WriteLine("{0,0}{1,10}", "Greedy: ", g);
                for (int i = 0; i < n; i++)
                {
                    SA.Start(initialTemperature, alpha, endTemperature, scenario, true);
                    avgEnergy += scenario.ObjectiveFunctionResult;
                    avgTime += scenario.ElapsedAlgorithmTime;
                    avgSum += scenario.SumOfAllSlices;
                    avgAvg += scenario.AverageSliceCountForEachPathAndSpRc;
                }
                avgEnergy /= n;
                avgTime /= n;
                avgSum /= n;
                avgAvg /= n;

                string fileResult = scenario.Name + " " + avgEnergy + " " + avgSum + " " + avgAvg + " " +
                                    avgTime;
                file.WriteLine(fileResult);
                ResultHelper resultSA = new ResultHelper(scenario.Name, "", avgEnergy, avgTime, avgSum, avgAvg);
                resultSA.Label = Int32.Parse(scenario.Name.Substring(11, 2)).ToString();
                from50To1000GbpsSA.Add(resultSA);
                string sa = scenario.Name + " Fmin: " + avgEnergy + " Sum: " +
                            avgSum + " Avg: " +
                            avgAvg + " Time:" +
                            avgTime;
                result = "SA: " + sa;
                Console.WriteLine("{0,0}{1,15}", "SA: ", sa);
                Console.WriteLine("----------");
            }
            file.Close();
            ChartGenerator c = new ChartGenerator();
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "energy_sprc", "energy",
                "Liczba zasobów przestrzennych");
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "avg_sprc", "avg", "Liczba zasobów przestrzennych");
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "sum_sprc", "sum", "Liczba zasobów przestrzennych");
        }*/

        public void Tuning(int repeat)
        {
            string dir = "D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\Euro28"; //"C:\\EURO16_30Tbps_Avg";
            double[] initialTemperature = {500.0, 1000.0, 2000.0, 5000.0, 10000.0, 50000.0, 100000.0};
            double[] alpha = {0.975, 0.981, 0.985, 0.987, 0.99};
            double[] endTemperature = {0.0001};
            string scenarioFile = "euro28.txt";
            string outputFile = "tuning2.txt";
            List<ResultHelper> from50To1000GbpsGreedy = new List<ResultHelper>();
            List<ResultHelper> from50To1000GbpsSA = new List<ResultHelper>();
            SimulatedAnnealing SA = new SimulatedAnnealing();
            Parser parser = new Parser();
            parser.addScenarios(dir, scenarioFile);
            double avgEnergy = 0.0, avgTime = 0.0, avgSum = 0.0, avgAvg = 0.0;
            System.IO.StreamWriter file = new System.IO.StreamWriter(dir + outputFile);
            string fileHeader = "Tinit\t alpha\t Tend\t avgEnergy\t avgSum\t avgAvg\t avgTime";
            file.WriteLine(fileHeader);
            foreach (var scenario in parser.ScenarioList)
            {
                for (int t = 0; t < initialTemperature.Length; t++)
                {
                    for (int a = 0; a < alpha.Length; a++)
                    {
                        for (int k = 0; k < endTemperature.Length; k++)
                        {
                            avgEnergy = 0.0;
                            avgTime = 0.0;
                            avgSum = 0.0;
                            avgAvg = 0.0;
                            int n = repeat;
                            for (int i = 0; i < n; i++)
                            {
                                SA.Start(initialTemperature[t], alpha[a], endTemperature[k], scenario, true);
                                avgEnergy += scenario.ObjectiveFunctionResult;
                                avgTime += scenario.ElapsedAlgorithmTime;
                                avgSum += scenario.SumOfAllSlices;
                                avgAvg += scenario.AverageSliceCountForEachPathAndSpRc;
                            }
                            avgEnergy /= n;
                            avgTime /= n;
                            avgSum /= n;
                            avgAvg /= n;

                            string parameters = initialTemperature[t] + "\t" + alpha[a] + "\t" +
                                                endTemperature[k];
                            string fileResult = "\t" + (int) avgEnergy + "\t" + (int) avgSum + "\t" + (int) avgAvg +
                                                "\t" +
                                                (int) avgTime;
                            file.WriteLine(parameters + fileResult);
                            ResultHelper resultSA = new ResultHelper(scenario.Name, "", avgEnergy, avgTime, avgSum,
                                avgAvg);
                            resultSA.Label = scenario.K.ToString();
                            from50To1000GbpsSA.Add(resultSA);
                            string sa = scenario.Name + " Fmin: " + (int) avgEnergy + " Sum: " +
                                        (int) avgSum + " Avg: " +
                                        (int) avgAvg + " Time:" +
                                        (int) avgTime;
                            Console.WriteLine("SA: " + sa);
                            Console.WriteLine("----------");
                        }
                    }
                }
            }
            file.Close();
        }
    }


    class Program
    {
        static void Main(string[] args)
        {
            Tester t = new Tester();
            //t.TestForK();
            string[] scenarioFilesDT14SpRc =
            {
                "dt14-s1.txt", "dt14-s2.txt", "dt14-s3.txt", "dt14-s4.txt", "dt14-s5.txt",
                "dt14-s6.txt", "dt14-s7.txt", "dt14-s8.txt", "dt14-s9.txt", "dt14-s10.txt"
            };
            string[] scenarioFilesEuro28SpRc =
{
                "euro28-s1.txt", "euro28-s2.txt", "euro28-s3.txt", "euro28-s4.txt", "euro28-s5.txt",
                "euro28-s6.txt", "euro28-s7.txt", "euro28-s8.txt", "euro28-s9.txt", "euro28-s10.txt"
            };
            string[] scenarioFilesDT14K =
            {
                "dt14-k2.txt", "dt14-k3.txt", "dt14-k5.txt", "dt14-k10.txt"
            };
            string[] scenarioFilesEuro28K =
{
                "euro28-k2.txt", "euro28-k3.txt", "euro28-k5.txt", "euro28-k10.txt"
            };
            string[] labelsSpRc = {"1", "2", "3", "4", "5", "6", "7", "8", "9", "10"};
            string[] labelsK = {"2", "3", "5", "10"};
            string[] outputFilesDT14SpRc =
            {
                "dt14-s1-res.txt", "dt14-s2-res.txt", "dt14-s3-res.txt", "dt14-s4-res.txt", "dt14-s5-res.txt",
                "dt14-s6-res.txt", "dt14-s7-res.txt", "dt14-s8-res.txt", "dt14-s9-res.txt", "dt14-s10-res.txt"
            };
            string[] outputFilesDT14K =
            {
                "dt14-k2-res.txt", "dt14-k3-res.txt", "dt14-k5-res.txt", "dt14-k10-res.txt"
            };
            string[] outputFilesEuro28SpRc =
{
                "euro28-s1-res.txt", "euro28-s2-res.txt", "euro28-s3-res.txt", "euro28-s4-res.txt", "euro28-s5-res.txt",
                "euro28-s6-res.txt", "euro28-s7-res.txt", "euro28-s8-res.txt", "euro28-s9-res.txt", "euro28-s10-res.txt"
            };
            string[] outputFilesEuro28K =
            {
                "euro28-k2-res.txt", "euro28-k3-res.txt", "euro28-k5-res.txt", "euro28-k10-res.txt"
            };

            //t.TestFrom50to1000("D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\DT14", "DT14", 10, 1000.0, 0.99, scenarioFiles, labels, outputFiles);
            t.PerformTests("D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\DT14", "DT14-sprc", 10, 1000.0, 0.99,
                scenarioFilesDT14SpRc, labelsSpRc, outputFilesDT14SpRc, "Liczba zasobów przestrzennych");
            t.PerformTests("D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\DT14", "DT14-k", 10, 1000.0, 0.99,
                scenarioFilesDT14K, labelsK, outputFilesDT14K, "Liczba ścieżek kandydujących");
            //t.TestFrom50to1000("D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\Euro28", "Euro28", 10, 1000.0, 0.99, scenarioFilesEuro, labels, outputFilesEuro);
            //t.TestForSpRc();
            //t.Tuning(10);
        }
    }
}