using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class GreedyHeuristic : HeuristicAlgorithm
    {
        public double Start(Scenario scenario, IComparer<Demand> comparator)
        {
            _scenario = scenario;
            _topologyGraph = new Graph(_scenario);
            _allocator = new SpectrumPathAllocator(_topologyGraph.Edges);
            var timer = new Stopwatch();
            double timeStart = 0.0;
            timer.Start();
            PathAllocator pathAllocator = new PathAllocator(_scenario, _topologyGraph.NumberOfNodes);
            DemandsVector currentSolution = new DemandsVector(_scenario, pathAllocator);
            createInitialSolution(currentSolution);
            currentSolution.Demands.Sort(comparator);
            allocateDemands(currentSolution);
            timer.Stop();
            _scenario.ObjectiveFunctionResult = _topologyGraph.GetHighestAllocatedSlot();
            _scenario.SumOfAllSlices = _topologyGraph.GetSumOfAllAllocatedSlots();
            _scenario.AverageSliceCountForEachPathAndSpRc = _topologyGraph.GetAverageMaxSpectrumSize();
            _scenario.ElapsedAlgorithmTime = timer.ElapsedMilliseconds;
            return timer.ElapsedMilliseconds;
        }
    }
}
