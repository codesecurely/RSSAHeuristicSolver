using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class NetworkTopologyParser
    {
        private int _numberOfNodes;
        private int[][] _networkMatrix;

        public int[][] NetworkMatrix
        {
            get { return _networkMatrix; }
        }

        public int NumberOfNodes
        {
            get { return _numberOfNodes; }
        }

        public NetworkTopologyParser(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            _numberOfNodes = Convert.ToInt32(lines[0]);
            _networkMatrix = new int[_numberOfNodes][];
            for (int i = 2; i < lines.Length; i++)
                _networkMatrix[i - 2] = lines[i].Split('\t').Select(n => Convert.ToInt32(n)).ToArray();
        }
    }

    class UnicastDemandsParser
    {
        private int _numberOfDemands;
        private int[] _sourceNode;
        private int[] _destinationNode;
        private int[] _demandVolume;

        public UnicastDemandsParser(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            _numberOfDemands = Convert.ToInt32(lines[0]);
            _sourceNode = new int[_numberOfDemands];
            _destinationNode = new int[_numberOfDemands];
            _demandVolume = new int[_numberOfDemands];
            for (int i = 1; i < lines.Length; i++)
            {
                string s = Regex.Replace(lines[i], @"\s+", " ");
                s.Trim();
                string[] data = s.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();
                _sourceNode[i - 1] = Convert.ToInt32(data[0]);
                _destinationNode[i - 1] = Convert.ToInt32(data[1]);
                _demandVolume[i - 1] = Convert.ToInt32(data[2]);
            }
        }

        public Dictionary<int, UnicastDemandData> GetUnicastDemandDataDictionary()
        {
            var dict = new Dictionary<int, UnicastDemandData>();
            for (int i = 0; i < _numberOfDemands; i++)
            {
                UnicastDemandData u = new UnicastDemandData(_sourceNode[i], _destinationNode[i], _demandVolume[i]);
                dict.Add(i, u);
            }
            return dict;
        }
    }

    class UnicastDemandData
    {
        private int _sourceNode;
        private int _destinationNode;
        private int _demandVolume;

        public int SourceNode
        {
            get { return _sourceNode; }
        }

        public int DestinationNode
        {
            get { return _destinationNode; }
        }

        public int DemandVolume
        {
            get { return _demandVolume; }
        }

        public UnicastDemandData(int sourceNode, int destinationNode, int demandVolume)
        {
            _sourceNode = sourceNode;
            _destinationNode = destinationNode;
            _demandVolume = demandVolume;
        }
    }

    class AnycastDemandsParser
    {
        private int _numberOfDemands;
        private int[] _clientNode;
        private int[] _downstreamVolume;
        private int[] _upstreamVolume;

        public AnycastDemandsParser(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            _numberOfDemands = Convert.ToInt32(lines[0]);
            _clientNode = new int[_numberOfDemands];
            _downstreamVolume = new int[_numberOfDemands];
            _upstreamVolume = new int[_numberOfDemands];
            for (int i = 1; i < lines.Length; i++)
            {
                string s = Regex.Replace(lines[i], @"\s+", " ");
                s.Trim();
                string[] data = s.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();
                _clientNode[i - 1] = Convert.ToInt32(data[0]);
                _downstreamVolume[i - 1] = Convert.ToInt32(data[1]);
                _upstreamVolume[i - 1] = Convert.ToInt32(data[2]);
            }
        }

        public Dictionary<int, AnycastDemandData> GetAnycastDemandDataDictionary()
        {
            var dict = new Dictionary<int, AnycastDemandData>();
            for (int i = 0; i < _numberOfDemands; i++)
            {
                AnycastDemandData a = new AnycastDemandData(_clientNode[i], _downstreamVolume[i], _upstreamVolume[i]);
                dict.Add(i, a);
            }
            return dict;
        }
    }

    class AnycastDemandData
    {
        private int _clientNode;
        private int _downstreamVolume;
        private int _upstreamVolume;

        public int ClientNode
        {
            get { return _clientNode; }
        }

        public int DownstreamVolume
        {
            get { return _downstreamVolume; }
        }

        public int UpstreamVolume
        {
            get { return _upstreamVolume; }
        }

        public AnycastDemandData(int clientNode, int downstreamVolume, int upstreamVolume)
        {
            _clientNode = clientNode;
            _downstreamVolume = downstreamVolume;
            _upstreamVolume = upstreamVolume;
        }
    }

    class CandidatePathsParser
    {
        private int _numberOfCandidates;
        private int[][] _candidatePaths;
        private int _k;

        public int[][] CandidatePaths
        {
            get { return _candidatePaths; }
        }

        public CandidatePathsParser(string path, int k)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            _numberOfCandidates = Convert.ToInt32(lines[0]);
            _candidatePaths = new int[_numberOfCandidates][];
            _k = k;
            for (int i = 1; i < lines.Length; i++)
                _candidatePaths[i - 1] = lines[i].Split(' ').Select(n => Convert.ToInt32(n)).ToArray();
        }
    }

    class PathLengthParser
    {
        private int _numberOfPaths;
        private int[] _pathLength;
        private int[] _modulationLevel;

        public int[] PathLength
        {
            get { return _pathLength; }
        }

        public int[] ModulationLevel
        {
            get { return _modulationLevel; }
        }

        public PathLengthParser(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            _numberOfPaths = Convert.ToInt32(lines[0]);
            _pathLength = new int[_numberOfPaths];
            _modulationLevel = new int[_numberOfPaths];
            for (int i = 1; i < lines.Length; i++)
            {
                string s = Regex.Replace(lines[i], @"\s+", " ");
                s.Trim();
                string[] data = s.Split(' ').Where(n => !string.IsNullOrWhiteSpace(n)).ToArray();
                _pathLength[i - 1] = Convert.ToInt32(data[0]);
                _modulationLevel[i - 1] = Convert.ToInt32(data[1]);
            }
        }
    }

    class DataCentersParser
    {
        private int _numberOfDataCenters;
        private int[] _dataCenterNodes;

        public int[] DataCenterNodes
        {
            get { return _dataCenterNodes; }
        }

        public DataCentersParser(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            _numberOfDataCenters = Convert.ToInt32(lines[0]);
            _dataCenterNodes = new int[_numberOfDataCenters];
            _dataCenterNodes = lines[1].Split(' ').Select(n => Convert.ToInt32(n)).ToArray();;
        }
    }

    class Scenario
    {
        private string _scenarioName;
        private NetworkTopologyParser _networkTopology;
        private UnicastDemandsParser _unicastDemands;
        private AnycastDemandsParser _anycastDemands;
        private CandidatePathsParser _candidatePaths;
        private PathLengthParser _pathLengths;
        private DataCentersParser _dataCenters;
        private int _k;
        private double _elapsedAlgorithmTime;
        private int _objectiveFunctionResult; //highest slice number
        private int _spatialResourcesCount;

        public int SpatialResourcesCount
        {
            get { return _spatialResourcesCount; }
        }

        public CandidatePathsParser CandidatePaths
        {
            get { return _candidatePaths; }
        }

        public int K
        {
            get { return _k; }
        }

        public PathLengthParser PathLengths
        {
            get { return _pathLengths; }
        }

        public NetworkTopologyParser NetworkTopology
        {
            get { return _networkTopology; }
        }

        public UnicastDemandsParser UnicastDemands
        {
            get { return _unicastDemands; }
        }

        public DataCentersParser DataCenters
        {
            get { return _dataCenters; }
        }

        public AnycastDemandsParser AnycastDemands
        {
            get { return _anycastDemands; }
        }

        public double ElapsedAlgorithmTime
        {
            get { return _elapsedAlgorithmTime; }

            set { _elapsedAlgorithmTime = value; }
        }

        public int ObjectiveFunctionResult
        {
            get { return _objectiveFunctionResult; }

            set { _objectiveFunctionResult = value; }
        }

        public string ScenarioName
        {
            get { return _scenarioName; }
        }

        public Scenario(string scenario, string path, int spatialResourcesCount)
        {
            _scenarioName = scenario;
            _spatialResourcesCount = spatialResourcesCount;
            _k = Convert.ToInt32(scenario.Substring(7, 2));
            var dict = new Dictionary<string, string>(MakeDictOfFiles(scenario, path));
            _networkTopology = new NetworkTopologyParser(dict["net"]);
            _unicastDemands = new UnicastDemandsParser(dict["dem"]);
            _anycastDemands = new AnycastDemandsParser(dict["dea"]);
            _candidatePaths = new CandidatePathsParser(dict["pat"], _k);
            _pathLengths = new PathLengthParser(dict["len"]);
            _dataCenters = new DataCentersParser(dict["rep"]);
            _objectiveFunctionResult = 0;
            _elapsedAlgorithmTime = 0.0;
        }

        private Dictionary<string, string> MakeDictOfFiles(string scenarioName, string path)
        {
            Dictionary<string, string> dict = new Dictionary<string, string>();
            dict.Add("net", path + "\\" + scenarioName.Substring(0, 2) + ".net");
            dict.Add("dem", path + "\\" + scenarioName.Substring(2, 2) + ".dem");
            dict.Add("dea", path + "\\" + scenarioName.Substring(4, 2) + ".dea");
            dict.Add("pat", path + "\\" + scenarioName.Substring(6, 3) + ".pat");
            dict.Add("len", path + "\\" + scenarioName.Substring(6, 3) + ".len");
            dict.Add("rep", path + "\\" + scenarioName.Substring(9, 2) + ".rep");

            return dict;
        }
    }

    class Parser
    {
        private List<Scenario> _scenarioList;
        private NetworkTopologyParser _networkTopology;
        private UnicastDemandsParser _unicastDemands;
        private AnycastDemandsParser _anycastDemands;
        private CandidatePathsParser _candidatePaths;
        private PathLengthParser _pathLengths;
        private DataCentersParser _dataCenters;
        private int _k;
        private int _spatialResourcesCount;

        public Parser(int spatialResourcesCount)
        {
            _scenarioList = new List<Scenario>();
            _spatialResourcesCount = spatialResourcesCount;
        }

        public List<Scenario> ScenarioList
        {
            get { return _scenarioList; }
        }

        public void addScenarios(string path, string filename)
        {
            string[] scenarioNames = getScenarioNames(path + "\\" + filename);
            foreach (var s in scenarioNames)
            {
                Scenario scenario = new Scenario(s, path, _spatialResourcesCount);
                _scenarioList.Add(scenario);
            }
        }

        private string[] getScenarioNames(string path)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException();
            string[] lines = File.ReadAllLines(path, Encoding.UTF8);
            return lines;
        }
    }
}