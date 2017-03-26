using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class Program
    {
        static void Main(string[] args)
        {
            string dir = "C:\\EURO16_30Tbps_Avg";
            double initialTemperature = 1000.0;
            double alpha = 0.99;
            double endTemperature = 0.01;
            string[] scenarioFiles =
            {
                "euro16_k2.txt", "euro16_k3.txt", "euro16_k5.txt", "euro16_k10.txt",
                "euro16_k30.txt"
            };
            string[] outputFiles =
            {
                "c:\\k2alpha09.txt", "c:\\k3alpha09.txt", "c:\\k5alpha09.txt", "c:\\k10alpha09.txt",
                "c:\\k30alpha09.txt"
            };

            SimulatedAnnealing SA = new SimulatedAnnealing();
            GreedyHeuristic greedy = new GreedyHeuristic();
            Parser parser = new Parser();
            parser.addScenarios(dir, scenarioFiles[0]);
            double avgEnergy = 0.0, avgTime = 0.0;
            foreach (var scenario in parser.ScenarioList)
            {
                var comp = new DemandDistanceSorter();
                greedy.Start(scenario, comp);
                //SA.Start(initialTemperature, alpha, endTemperature, scenario);
                string result = scenario.ScenarioName + " " + scenario.ObjectiveFunctionResult + " " + scenario.ElapsedAlgorithmTime;
                Console.WriteLine(result);
                avgEnergy += scenario.ObjectiveFunctionResult;
                avgTime += scenario.ElapsedAlgorithmTime;
            }
            /*for (int i = 0; i < 5; i++)
            {
                SimulatedAnnealing SA = new SimulatedAnnealing();
                System.IO.StreamWriter file = new System.IO.StreamWriter(outputFiles[i]);
                file.WriteLine("Alpha: "+alpha);
                Parser parser = new Parser();
                parser.addScenarios(dir, scenarioFiles[i]);
                double avgEnergy = 0.0, avgTime = 0.0;
                foreach (var scenario in parser.ScenarioList)
                {
                    SA.Start(initialTemperature, alpha, endTemperature, scenario);
                    string result = scenario.ScenarioName + " " + scenario.ObjectiveFunctionResult + " " + scenario.Time;
                    Console.WriteLine(result);
                    file.WriteLine(result);
                    avgEnergy += scenario.ObjectiveFunctionResult;
                    avgTime += scenario.Time;
                }
                avgEnergy /= parser.ScenarioList.Count;
                avgTime /= parser.ScenarioList.Count;
                file.WriteLine("average energy: "+ avgEnergy);
                file.WriteLine("average time: " + avgTime);
                file.Close();
            }
            
    */
        }
    }
}