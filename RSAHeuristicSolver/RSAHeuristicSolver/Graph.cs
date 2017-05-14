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
                        Edge e = new Edge(matrix.NetworkTopology.NetworkMatrix[i][j], i, j, matrix.SpatialResourcesCount);
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

        public int GetHighestAllocatedSlot() //returns the highest allocated slot, which is the objective function
        {
            int highestAllocatedSlot = 0;
            foreach (var e in _edges)
            {
                if (e.Value.SpectrumEdgeAllocator.MaxSpectrumSize() > highestAllocatedSlot)
                    highestAllocatedSlot = e.Value.SpectrumEdgeAllocator.MaxSpectrumSize();
            }
            return highestAllocatedSlot;
        }

        public int GetSumOfAllAllocatedSlots()
        {
            int sumOfAllAllocatedSlots = 0;
            foreach (var e in _edges)
                sumOfAllAllocatedSlots += e.Value.SpectrumEdgeAllocator.SpectrumSum();
            return sumOfAllAllocatedSlots;
        }
        public double GetAverageMaxSpectrumSize() //returns average of max sizes of spectrum for every path and SpRc
        {
            double avgSpectrumSize = 0.0;
            int div = _edges.Count;
            foreach (var e in _edges)
                avgSpectrumSize += e.Value.SpectrumEdgeAllocator.MaxSpectrumSize();

            return avgSpectrumSize/div;

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

        public Edge(int edgeLength, int sourceNode, int destinationNode, int spatialResourceCount)
        {
            _edgeLength = edgeLength;
            _sourceNode = sourceNode;
            _destinationNode = destinationNode;
            _spectrumEdgeAllocator = new SpectrumEdgeAllocator(spatialResourceCount);
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

    
}