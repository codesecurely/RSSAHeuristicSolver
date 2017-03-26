using System;
using System.Collections.Generic;
using System.Diagnostics;
using RSAHeuristicSolver;

namespace RSAHeuristicSolver
{
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
        int srcIndex = sourceNode * _k * (_numberOfnodes - 1);
        int dstIndex = destinationNode * _k;
        if (destinationNode > sourceNode)
            dstIndex -= _k;
        int index = srcIndex + dstIndex;
        return index;
    }
}