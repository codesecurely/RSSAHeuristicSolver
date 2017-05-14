using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class TabuSearch : HeuristicAlgorithm
    {
        private int _bestFitness;
        private int _currentFitness;
        private double _initialTemperature;
        private double _finalTemperature;
        private int _nextFitness;
        private int _tabuListLength;
        private double _currentTemperature;
        private Queue<Move> _tabuList;


        public TabuSearch()
        {
            
        }
        public double Start()
        {
            _allocator = new SpectrumPathAllocator(_topologyGraph.Edges);
            int iterations = 0;
            var timer = new Stopwatch();
            double timeStart = 0.0;
            timer.Start();
            Random rnd = new Random();
            PathAllocator pathAllocator = new PathAllocator(_scenario, _topologyGraph.NumberOfNodes);
            DemandsVector currentSolution = new DemandsVector(_scenario, pathAllocator);
            currentSolution = createInitialSolution(currentSolution);
            allocateDemands(currentSolution);
            _currentFitness = _topologyGraph.GetHighestAllocatedSlot();

            timer.Stop();
            _scenario.ObjectiveFunctionResult = _bestFitness;
            _scenario.ElapsedAlgorithmTime = timer.ElapsedMilliseconds;
            return timer.ElapsedMilliseconds;
        }
    }

    struct Move
    {
        int A;
        int B;
    }
}
