using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class Graph
    {
        private Dictionary<int, Node> _nodes;
        private Dictionary<int, Edge> _edges;
        private int _numberOfNodes;

        public Dictionary<int, Edge> Edges
        {
            get { return _edges; }
        }

        public int NumberOfNodes
        {
            get { return _numberOfNodes; }
        }

        public Graph(Scenario scenario)
        {
            _edges = CreateEdges(scenario);
            _nodes = CreateNodes(scenario, _edges);
            _numberOfNodes = _nodes.Count;
        }

        private Dictionary<int, Edge> CreateEdges(Scenario matrix)
        {
            var dict = new Dictionary<int, Edge>();
            int key = 0;
            for (int i = 0; i < matrix.NetworkTopology.NetworkMatrix.Length; i++)
                for (int j = 0; j < matrix.NetworkTopology.NetworkMatrix[i].Length; j++)
                    if (matrix.NetworkTopology.NetworkMatrix[i][j] > 0)
                    {
                        Edge e = new Edge(matrix.NetworkTopology.NetworkMatrix[i][j], i, j);
                        dict.Add(key, e);
                        key++;
                    }

            return dict;
        }

        private Dictionary<int, Node> CreateNodes(Scenario matrix, Dictionary<int, Edge> edges)
        {
            var dict = new Dictionary<int, Node>();
            for (int key = 0; key < matrix.NetworkTopology.NumberOfNodes; key++)
            {
                Node n = new Node();
                dict.Add(key, n);
            }
            for (int i = 0; i < edges.Count; i++)
            {
                dict[edges[i].SourceNode].AddEdge(edges[i]);
                dict[edges[i].DestinationNode].AddEdge(edges[i]);
            }


            return dict;
        }

        public int ComputeCost() //returns the highest allocated slot, which equals a sum of spectrum slices
        {
            int cost = 0;
            foreach (var e in _edges)
            {
                if (e.Value.SpectrumEdgeAllocator.SpectrumSize > cost)
                    cost = e.Value.SpectrumEdgeAllocator.SpectrumSize;
            }
            return cost;
        }
    }

    class Edge
    {
        private int _edgeLength;
        private int _sourceNode;
        private int _destinationNode;
        private SpectrumEdgeAllocator _spectrumEdgeAllocator;

        public int SourceNode
        {
            get { return _sourceNode; }
        }

        public int DestinationNode
        {
            get { return _destinationNode; }
        }

        public SpectrumEdgeAllocator SpectrumEdgeAllocator
        {
            get { return _spectrumEdgeAllocator; }
        }

        public Edge(int edgeLength, int sourceNode, int destinationNode)
        {
            _edgeLength = edgeLength;
            _sourceNode = sourceNode;
            _destinationNode = destinationNode;
            _spectrumEdgeAllocator = new SpectrumEdgeAllocator();
        }
    }

    class Node
    {
        //private string _nodeName;
        private List<Edge> _edgesBelongingToNode;

        public Node()
        {
            _edgesBelongingToNode = new List<Edge>();
        }

        public void AddEdge(Edge e)
        {
            _edgesBelongingToNode.Add(e);
        }
    }

    class Path
    {
        private int _pathID;
        private List<int> _edgesBelongingToPath;
        private int _pathLength;
        //modulation level and number of slices are the same for each edge in a path, so we can have these fields in Path class
        private int _modulationLevel;
        private int _numberOfSlices;

        public List<int> EdgesBelongingToPath
        {
            get { return _edgesBelongingToPath; }
        }

        public int NumberOfSlices
        {
            get { return _numberOfSlices; }
        }

        public int PathID
        {
            get { return _pathID; }
        }

        public int PathLength
        {
            get { return _pathLength; }
        }

        public Path(int id, List<int> edges, int length, int modulationLevel)
        {
            _pathID = id;
            _edgesBelongingToPath = edges;
            _pathLength = length;
            _modulationLevel = modulationLevel;
            _numberOfSlices = 0;
        }

        public void SetNumberOfSlices(int demandVolume, double slizeSize)
        {
            _numberOfSlices = (int) Math.Ceiling(demandVolume/(2*_modulationLevel*slizeSize));
        }
    }

    class PathAllocator
    {
        private int[][] _allCandidatePaths;
        private int _k;
        private int _numberOfnodes;
        private int[] _modulationLevels;
        private int[] _lengths;

        public PathAllocator(Scenario paths, int numberOfNodes)
        {
            _k = paths.K;
            _numberOfnodes = numberOfNodes;
            _allCandidatePaths = paths.CandidatePaths.CandidatePaths;
            _modulationLevels = paths.PathLengths.ModulationLevel;
            _lengths = paths.PathLengths.PathLength;
        }

        public List<Path> GetCandidatePaths(int sourceNode, int destinationNode) //for a given demand
        {
            Debug.Assert(!(sourceNode > _numberOfnodes - 1 || destinationNode > _numberOfnodes - 1),
                "Node number out of bounds");
            List<Path> candidatePaths = new List<Path>();
            int first = GetFirstIndex(sourceNode, destinationNode);
            int last = first + _k;
            for (int i = first; i < last; i++)
            {
                List<int> e = new List<int>();
                for (int j = 0; j < _allCandidatePaths[i].Length; j++)
                    if (_allCandidatePaths[i][j] == 1)
                        e.Add(j);

                Path p = new Path(i, e, _lengths[i], _modulationLevels[i]);
                candidatePaths.Add(p);
            }
            return candidatePaths;
        }

        private int GetFirstIndex(int sourceNode, int destinationNode)
        {
            int srcIndex = sourceNode*_k*(_numberOfnodes - 1);
            int dstIndex = destinationNode*_k;
            if (destinationNode > sourceNode)
                dstIndex -= _k;
            int index = srcIndex + dstIndex;
            return index;
        }
    }
}