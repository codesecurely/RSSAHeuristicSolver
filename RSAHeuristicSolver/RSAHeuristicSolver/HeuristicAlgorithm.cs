using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    abstract class HeuristicAlgorithm
    {
        protected SpectrumPathAllocator _allocator;
        protected DemandsVector _demands;
        protected Graph _topologyGraph;
        protected Scenario _scenario;

        protected DemandsVector createNextSolution(DemandsVector currentSolution)
        {
            Random rnd = new Random();
            DemandsVector nextSolution = new DemandsVector(currentSolution);
            int firstIndex = rnd.Next(0, nextSolution.Demands.Count);
            int lastIndex = rnd.Next(0, nextSolution.Demands.Count);
            nextSolution.Demands.Swap(firstIndex, lastIndex);
            nextSolution.Demands[lastIndex].SetRandomPath();
            nextSolution.Demands[firstIndex].SetRandomPath();

            return nextSolution;
        }

        protected DemandsVector createInitialSolution(DemandsVector demands)
        {
            DemandsVector initialSolution = new DemandsVector(demands);
            foreach (var d in initialSolution.Demands)
                d.SetRandomPath();
            initialSolution.Demands.Shuffle();
            return initialSolution;
        }

        protected void allocateDemands(DemandsVector demands)
        {
            _allocator.Erase();
            foreach (var d in demands.Demands)
                d.AllocateDemand(_allocator);
        }
    }
}
