using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class SimulatedAnnealing : MetaheuristicAlgorithm
    {
        private int _bestEnergy;
        private int _currentEnergy;
        private double _initialTemperature;
        private double _alpha;
        private double _finalTemperature;
        private int _nextEnergy;
        private double _currentTemperature;

        public SimulatedAnnealing()
        {
            _initialTemperature = 0.0;
            _alpha = 0.0;
            _finalTemperature = 0.0;
            _currentTemperature = _initialTemperature;
            _bestEnergy = 0;
            _currentEnergy = 0;
            _nextEnergy = 0;
            _scenario = null;
            _topologyGraph = null;
            _allocator = null;
        }


        public double Start(double initialTemperature, double alpha, double finalTemperature, Scenario scenario)
        {
            _initialTemperature = initialTemperature;
            _currentTemperature = initialTemperature;
            _alpha = alpha;
            _finalTemperature = finalTemperature;
            _scenario = scenario;
            _bestEnergy = 0;
            _currentEnergy = 0;
            _nextEnergy = 0;
            _topologyGraph = new Graph(_scenario);
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
            _currentEnergy = _topologyGraph.ComputeCost();
            int initialEnergy = _currentEnergy;
            DemandsVector bestSolution = new DemandsVector(currentSolution);
            _bestEnergy = _currentEnergy;
            while (_currentTemperature > _finalTemperature)
            {
                DemandsVector nextSolution = new DemandsVector(createNextSolution(currentSolution));
                allocateDemands(nextSolution);
                _nextEnergy = _topologyGraph.ComputeCost();
                if (_nextEnergy < _currentEnergy)
                {
                    _currentEnergy = _nextEnergy;
                    currentSolution = new DemandsVector(nextSolution);
                    if (_nextEnergy < _bestEnergy)
                    {
                        _bestEnergy = _nextEnergy;
                        bestSolution = new DemandsVector(nextSolution);
                    }
                }
                else if (computeProbablity() > rnd.NextDouble())
                    //this returns a value in range (0.0, 0.1) by default with no arguments
                {
                    _currentEnergy = _nextEnergy;
                    currentSolution = new DemandsVector(nextSolution);
                }
                _currentTemperature = _currentTemperature*_alpha;
                iterations++;
                if (iterations > 10000) _currentTemperature = 0.0;
            }
            timer.Stop();
            _scenario.ObjectiveFunctionResult = _bestEnergy;
            _scenario.ElapsedAlgorithmTime = timer.ElapsedMilliseconds;
            return timer.ElapsedMilliseconds;
        }
        private double computeProbablity()
        {
            return Math.Exp(-(_nextEnergy - _currentEnergy)/_currentTemperature);
        }
    }
}