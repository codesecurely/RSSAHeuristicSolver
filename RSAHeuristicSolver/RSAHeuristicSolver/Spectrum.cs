using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Security.Authentication.ExtendedProtection;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    class SpectrumEdgeAllocator
    {
        private List<int> _spatialResource;
        private int _spectrumSize;

        public int SpectrumSize
        {
            get { return _spectrumSize; }
        }

        public SpectrumEdgeAllocator()
        {
            _spatialResource = new List<int>();
            _spectrumSize = 0;
        }

        public bool IsFree(int firstSlice, int lastSlice)
        {
            while (_spatialResource.Count < lastSlice)
                _spatialResource.Add(0);
            for (int slice = firstSlice; slice < lastSlice; slice++)
                if (_spatialResource[slice] == 1)
                    return false;
            return true;
        }

        public void Allocate(int firstSlice, int lastSlice)
        {
            if (IsFree(firstSlice, lastSlice))
            {
                for (int slice = firstSlice; slice < lastSlice; slice++)
                {
                    _spatialResource[slice] = 1;
                }
                _spectrumSize = lastSlice;
            }
        }

        public int GetFirstFreeSlice(int size)
        {
            int slice = 0;
            for (slice = 0; slice <= _spatialResource.Count; slice++)
                if (IsFree(slice, slice + _spectrumSize))
                    return slice;
            return 0;
        }

        public void Erase()
        {
            _spatialResource.Clear();
            _spectrumSize = 0;
        }
    }

    class SpectrumPathAllocator
    {
        private Dictionary<int, Edge> _edges;

        public SpectrumPathAllocator(Dictionary<int, Edge> edges)
        {
            _edges = edges;
        }

        public int GetFirstFreeSpectrumOnPath(Path path) //First-Fit allocation
        {
            int minFreeSliceNumber = 0;
            bool keepAllocating = true;
            while (keepAllocating)
            {
                keepAllocating = false;
                foreach (int e in path.EdgesBelongingToPath)
                {
                    Edge edge = _edges[e];
                    int firstFreeSlice = edge.SpectrumEdgeAllocator.GetFirstFreeSlice(path.NumberOfSlices);
                    if (firstFreeSlice > minFreeSliceNumber)
                    {
                        minFreeSliceNumber = firstFreeSlice;
                        keepAllocating = true;
                    }
                }
            }
            return minFreeSliceNumber;
        }

        //allocated channel must be the same for each edge on a path
        public void AllocateFirstFreeSpectrumOnPath(Path path)
        {
            int spectrumSize = path.NumberOfSlices;
            int firstSlice = GetFirstFreeSpectrumOnPath(path);
            foreach (int e in path.EdgesBelongingToPath)
            {
                Edge edge = _edges[e];
                edge.SpectrumEdgeAllocator.Allocate(firstSlice, firstSlice + spectrumSize);
            }
        }

        public void Erase()
        {
            foreach (var e in _edges)
                e.Value.SpectrumEdgeAllocator.Erase();
        }
    }
}