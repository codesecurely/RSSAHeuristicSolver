using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;


namespace RSAHeuristicSolver
{
    abstract class Demand
    {
        //protected string _name;
        protected List<Path> _candidatePaths;
        protected int _sourceNode;
        protected int _numberOfPaths;
        protected const double SliceSize = 6.25;
        protected abstract void CalculateNumberOfSlices();
        public abstract void SetRandomPath();
        public abstract void AllocateDemand(SpectrumPathAllocator allocator);

        public List<Path> CandidatePaths
        {
            get { return _candidatePaths; }
        }
    }

    class UnicastDemand : Demand
    {
        private int _destinationNode;
        private int _demandVolume;
        private int _demandPath;
        private Demand demand;

        public int DemandPath
        {
            get { return _demandPath; }
        }

        public int NumberOfPaths
        {
            get { return _numberOfPaths; }
        }

        public UnicastDemand(int sourceNode, int destinationNode, int demandVolume, List<Path> candidatePaths)
        {
            _sourceNode = sourceNode;
            _destinationNode = destinationNode;
            _demandVolume = demandVolume;
            _candidatePaths = candidatePaths;
            _numberOfPaths = _candidatePaths.Count;
            _demandPath = 0; //these values are to be computed later by simulated annealing
            CalculateNumberOfSlices();
        }

        public UnicastDemand(UnicastDemand demand)
        {
            _candidatePaths = new List<Path>(demand._candidatePaths);
            _sourceNode = demand._sourceNode;
            _numberOfPaths = demand._numberOfPaths;
            _demandVolume = demand._demandVolume;
            _numberOfPaths = demand._candidatePaths.Count;
            _demandPath = demand._demandPath;
        }


        protected override void CalculateNumberOfSlices()
            //sets a number of slices for each path. We need to call this method from UnicastDemand, because it knows about the volume
        {
            foreach (Path path in _candidatePaths)
                path.SetNumberOfSlices(_demandVolume, SliceSize);
        }

        public void SelectDemandPath(int index)
        {
            Debug.Assert(!(index > NumberOfPaths || index < 0), "Path index out of range");
            _demandPath = index;
        }

        public override void SetRandomPath()
        {
            Random rnd = new Random();
            int path = rnd.Next(0, NumberOfPaths);
            if (CandidatePaths[path].PathLength < CandidatePaths[DemandPath].PathLength)
                SelectDemandPath(path);
        }

        public override void AllocateDemand(SpectrumPathAllocator allocator)
        {
            Path path = GetDemandPath();
            allocator.AllocateFirstFreeSpectrumOnPath(path);
        }

        public Path GetDemandPath()
        {
            return _candidatePaths[_demandPath];
        }
    }

    class AnycastDemand : Demand
    {
        //protected List<Path> _candidatePaths;
        //protected int _sourceNode;
        //protected int _numberOfPaths;
        //protected const double SliceSize = 6.25;
        private int _upstreamVolume;
        private int _downstreamVolume;
        private int _demandPathUp;
        private int _demandPathDown;
        private int _numberOfDc;
        private int _selectedDC;
        private List<int> _dataCenterNodes;
        private Dictionary<int, List<List<Path>>> _pathsToAndFromDataCenters;

        public List<int> DataCenterNodes
        {
            get { return _dataCenterNodes; }
        }

        public Dictionary<int, List<List<Path>>> PathsToAndFromDataCenters
        {
            get { return _pathsToAndFromDataCenters; }
        }

        public AnycastDemand(int sourceNode, int upstreamVolume, int downstreamVolume, List<int> dataCenterNodes,
            Dictionary<int, List<List<Path>>> pathsToAndFromDataCenters)
        {
            _sourceNode = sourceNode;
            _upstreamVolume = upstreamVolume;
            _downstreamVolume = downstreamVolume;
            _demandPathUp = 0;
            _demandPathDown = 0;
            _selectedDC = 0;
            _dataCenterNodes = dataCenterNodes;
            _numberOfDc = dataCenterNodes.Count;
            _pathsToAndFromDataCenters = pathsToAndFromDataCenters;
            _numberOfPaths = _pathsToAndFromDataCenters.Count;
            CalculateNumberOfSlices();
        }

        public AnycastDemand(AnycastDemand demand)
        {
            _sourceNode = demand._sourceNode;
            _upstreamVolume = demand._upstreamVolume;
            _downstreamVolume = demand._downstreamVolume;
            _demandPathUp = demand._demandPathUp;
            _demandPathDown = demand._demandPathDown;
            _selectedDC = demand._selectedDC;
            _dataCenterNodes = demand._dataCenterNodes;
            _numberOfDc = demand._dataCenterNodes.Count;
            //[0] upstream, [1] downstream
            _pathsToAndFromDataCenters = new Dictionary<int, List<List<Path>>>(demand._pathsToAndFromDataCenters);
            _numberOfPaths = demand._pathsToAndFromDataCenters.Count;
        }

        protected override void CalculateNumberOfSlices()
            //sets a number of slices for each path. We need to call this method from AnycastDemand, because it knows about the volume
        {
            foreach (var p in _pathsToAndFromDataCenters)
            {
                foreach (var up in p.Value[0])
                    up.SetNumberOfSlices(_upstreamVolume, SliceSize);
                foreach (var down in p.Value[1])
                    down.SetNumberOfSlices(_downstreamVolume, SliceSize);
            }
        }

        public void SelectDemandPathUp(int index)
        {
            Debug.Assert(!(index > _numberOfPaths || index < 0), "Path index out of range");
            _demandPathUp = index;
        }

        public void SelectDemandPathDown(int index)
        {
            Debug.Assert(!(index > _numberOfPaths || index < 0), "Path index out of range");
            _demandPathDown = index;
        }

        public void SelectDataCenter(int nodeID)
        {
            Debug.Assert((_dataCenterNodes.Contains(nodeID)), "No such DC available");
            _selectedDC = nodeID;
        }

        public int getNumberOfPaths()
        {
            return _pathsToAndFromDataCenters.Values.Count;
        }

        public Path GetDemandPathUp()
        {
            return _pathsToAndFromDataCenters[_selectedDC][0][_demandPathUp];
        }

        public Path GetDemandPathDown()
        {
            return _pathsToAndFromDataCenters[_selectedDC][1][_demandPathDown];
        }

        public override void SetRandomPath()
        {
            Random rnd = new Random();
            int dataCenter = rnd.Next(0, DataCenterNodes.Count);
            SelectDataCenter(DataCenterNodes[dataCenter]);
            int pathUp = rnd.Next(0, getNumberOfPaths() - 1);
            if (PathsToAndFromDataCenters[DataCenterNodes[dataCenter]][0][pathUp].PathLength <
                GetDemandPathUp().PathLength)
                SelectDemandPathUp(pathUp);
            int pathDown = rnd.Next(0, getNumberOfPaths() - 1);
            if (PathsToAndFromDataCenters[DataCenterNodes[dataCenter]][1][pathDown].PathLength <
                GetDemandPathDown().PathLength)
                SelectDemandPathDown(pathDown);
        }

        public override void AllocateDemand(SpectrumPathAllocator allocator)
        {
            Path pathUp = GetDemandPathUp();
            Path pathDown = GetDemandPathDown();
            allocator.AllocateFirstFreeSpectrumOnPath(pathUp);
            allocator.AllocateFirstFreeSpectrumOnPath(pathDown);
        }
    }

    class DemandsVector
    {
        private List<Demand> _unicastDemands;
        private List<Demand> _anycastDemands;
        private List<Demand> _demands;

        public List<Demand> Demands
        {
            get { return _demands; }
        }

        public DemandsVector()
        {
            _demands = new List<Demand>();
        }

        public DemandsVector(DemandsVector v)
        {
            _demands = new List<Demand>(v._demands);
        }

        public DemandsVector(Scenario demands, PathAllocator pathAllocator)
        {
            _unicastDemands = CreateUnicastDemands(demands, pathAllocator);
            _anycastDemands = CreateAnycastDemands(demands, pathAllocator);
            _demands = _unicastDemands;
            _demands.AddRange(_anycastDemands);
        }

        public void AddDemand(Demand d)
        {
            _demands.Add(d);
        }

        public List<Demand> CreateUnicastDemands(Scenario demands, PathAllocator pathAllocator)
        {
            List<Demand> unicastDemands = new List<Demand>();
            Dictionary<int, UnicastDemandData> demandsData = demands.UnicastDemands.GetUnicastDemandDataDictionary();
            foreach (var key in demandsData)
            {
                UnicastDemand u = new UnicastDemand(key.Value.SourceNode, key.Value.DestinationNode,
                    key.Value.DemandVolume,
                    pathAllocator.GetCandidatePaths(key.Value.SourceNode, key.Value.DestinationNode));
                unicastDemands.Add(u);
            }

            return unicastDemands;
        }

        public List<Demand> CreateAnycastDemands(Scenario demands, PathAllocator pathAllocator)
        {
            List<Demand> anycastDemands = new List<Demand>();
            List<int> dataCenters = new List<int>(demands.DataCenters.DataCenterNodes.ToList());
            Dictionary<int, AnycastDemandData> demandsData = demands.AnycastDemands.GetAnycastDemandDataDictionary();

            foreach (var key in demandsData)
            {
                var dict = new Dictionary<int, List<List<Path>>>();
                foreach (var d in dataCenters)
                {
                    var pathsUp = new List<Path>(pathAllocator.GetCandidatePaths(key.Value.ClientNode, d));
                    var pathsDown = new List<Path>(pathAllocator.GetCandidatePaths(d, key.Value.ClientNode));
                    var paths = new List<List<Path>>();
                    paths.Add(pathsUp);
                    paths.Add(pathsDown);
                    dict.Add(d, paths);
                }
                AnycastDemand a = new AnycastDemand(key.Value.ClientNode, key.Value.DownstreamVolume,
                    key.Value.UpstreamVolume, dataCenters, dict);
                anycastDemands.Add(a);
            }
            return anycastDemands;
        }
    }
}