using System;
using System.Collections.Generic;
using System.Linq;
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
        public int Gbps;
        public string Label;

        public ResultHelper(string name, double energy, double time, double sum, double avg)
        {
            ScenarioName = name;
            AvgEnergy = energy;
            AvgTime = time;
            AvgSum = sum;
            AvgAvg = avg;
            Gbps = (Int32.Parse(ScenarioName.Substring(2, 2)))*50;
            Label = Gbps + "-" + (Gbps + 50);
        }
    }

    class Tester
    {
        public void TestFrom50to1000()
        {
            string dir = "D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\Euro28"; //"C:\\EURO16_30Tbps_Avg";
            double initialTemperature = 1000.0;
            double alpha = 0.99;
            double endTemperature = 0.001;
            string[] scenarioFiles =
            {
                "euro28.txt", "euro16_k3.txt", "euro16_k5.txt", "euro16_k10.txt",
                "euro16_k30.txt"
            };
            string[] outputFiles =
            {
                "c:\\euro28.txt", "c:\\k3alpha09.txt", "c:\\k5alpha09.txt", "c:\\k10alpha09.txt",
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
                int n = 1;
                string g = scenario.Name + " Fmin: " + scenario.ObjectiveFunctionResult + " Sum: " +
                           scenario.SumOfAllSlices + " Avg: " +
                           scenario.AverageSliceCountForEachPathAndSpRc.ToString("F2") + " Time:" +
                           scenario.ElapsedAlgorithmTime;
                string result = "Greedy: " + g;
                from50To1000GbpsGreedy.Add(new ResultHelper(scenario.Name, scenario.ObjectiveFunctionResult,
                    scenario.ElapsedAlgorithmTime, scenario.SumOfAllSlices, scenario.AverageSliceCountForEachPathAndSpRc));
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
                from50To1000GbpsSA.Add(new ResultHelper(scenario.Name, avgEnergy, avgTime, avgSum, avgAvg));
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
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "energy", "energy", "Przepływność żądań [Gbps]");
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "avg", "avg", "Przepływność żądań [Gbps]");
            c.GenerateChart(from50To1000GbpsSA, from50To1000GbpsGreedy, "sum", "sum", "Przepływność żądań [Gbps]");
        }

        public void TestForK()
        {
            string dir = "D:\\Dropbox\\politechnika\\mgr\\praca_mgr\\topologie\\Euro28"; //"C:\\EURO16_30Tbps_Avg";
            double initialTemperature = 1000.0;
            double alpha = 0.99;
            double endTemperature = 0.001;
            string[] scenarioFiles =
            {
                "euro28_diff_k.txt", "euro16_k3.txt", "euro16_k5.txt", "euro16_k10.txt",
                "euro16_k30.txt"
            };
            string[] outputFiles =
            {
                "c:\\euro28_diff_k.txt", "c:\\k3alpha09.txt", "c:\\k5alpha09.txt", "c:\\k10alpha09.txt",
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
                ResultHelper resultGreedy = new ResultHelper(scenario.Name, scenario.ObjectiveFunctionResult,
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
                ResultHelper resultSA = new ResultHelper(scenario.Name, avgEnergy, avgTime, avgSum, avgAvg);
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

        public void TestForSpRc()
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
                ResultHelper resultGreedy = new ResultHelper(scenario.Name, scenario.ObjectiveFunctionResult,
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
                ResultHelper resultSA = new ResultHelper(scenario.Name, avgEnergy, avgTime, avgSum, avgAvg);
                resultSA.Label = scenario.Name.Substring(11, 2);
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
        }
    }


class Program
    {
        static void Main(string[] args)
        {
            Tester t = new Tester();
            //t.TestForK();
            //t.TestFrom50to1000();
            t.TestForSpRc();
        }
    }
}