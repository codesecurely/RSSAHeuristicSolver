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
        private List<SpatialResource> _spatialResources;


        public int SpectrumSize(int id)
        {
            return _spatialResources[id].SpectrumSize;
        }

        public int MaxSpectrumSize()
        {
            return _spatialResources.Max(r => r.SpectrumSize);
        }

        public int SpectrumSum()
        {
            int sum = 0;
            foreach (var s in _spatialResources)
                sum += s.SpectrumSize;
            return sum;
        }

        public SpatialResource getSpatialResource(int id)
        {
            return _spatialResources[id];
        }

        public SpectrumEdgeAllocator(int spatialResourcesCount)
        {
            _spatialResources = new List<SpatialResource>();
            for (int i = 0; i< spatialResourcesCount; i++)
                _spatialResources.Add(new SpatialResource());
        }

        public bool IsFree(int firstSlice, int lastSlice, SpatialResource spatialResource)
        {
            while (spatialResource.Slices.Count < lastSlice)
                spatialResource.Slices.Add(0);
            for (int slice = firstSlice; slice < lastSlice; slice++)
                if (spatialResource.Slices[slice] == 1)
                    return false;
            return true;
        }

        public void Allocate(int firstSlice, int lastSlice, SpatialResource spatialResource)
        {
            if (IsFree(firstSlice, lastSlice, spatialResource))
            {
                for (int slice = firstSlice; slice < lastSlice; slice++)
                {
                    spatialResource.Slices[slice] = 1;
                }
                spatialResource.SpectrumSize = lastSlice;
            }
        }

        public int GetFirstFreeSlice(int size, SpatialResource spatialResource)
        {
            int slice = 0;
            for (slice = 0; slice <= spatialResource.Slices.Count; slice++)
                if (IsFree(slice, slice + spatialResource.SpectrumSize, spatialResource))
                    return slice;
            return slice;
        }

        public SpatialResource GetBestSpatialResource()
        {
            int minSpec = _spatialResources[0].SpectrumSize;
            SpatialResource bestResource = _spatialResources[0];
            //we iterate over all SpRc and pick the one with the least allocated spectrum
            foreach (var s in _spatialResources)
            {
                if (s.SpectrumSize < minSpec)
                {
                    bestResource = s;
                    minSpec = s.SpectrumSize;
                }
            }
            return bestResource;
        }

        public void Erase()
        {
            foreach (var s in _spatialResources)
                s.Clear();
        }
    }

    class SpectrumPathAllocator
    {
        private Dictionary<int, Edge> _edges;

        public SpectrumPathAllocator(Dictionary<int, Edge> edges)
        {
            _edges = edges;
        }

        public SpatialResource GetFirstFreeSpectrumOnPath(Path path) //First-Fit allocation
        {
            int minFreeSliceNumber = 0;
            bool keepAllocating = true;
            SpatialResource best = null;
            while (keepAllocating)
            {
                keepAllocating = false;
                foreach (int e in path.EdgesBelongingToPath)
                {
                    Edge edge = _edges[e];
                    //int firstFreeSlice = edge.SpectrumEdgeAllocator.GetFirstFreeSlice(path.NumberOfSlices, edge.SpectrumEdgeAllocator.GetFirstFreeSpatialResource());
                    best = edge.SpectrumEdgeAllocator.GetBestSpatialResource();
                    int firstFreeSlice = edge.SpectrumEdgeAllocator.GetFirstFreeSlice(path.NumberOfSlices, best);
                    if (firstFreeSlice > minFreeSliceNumber)
                    {
                        minFreeSliceNumber = firstFreeSlice;
                        keepAllocating = true;
                    }
                }
            }
            best.MinFreeSliceNumber = minFreeSliceNumber;
            return best;
        }

        //allocated channel must be the same for each edge on a path
        public void AllocateFirstFreeSpectrumOnPath(Path path)
        {
            int spectrumSize = path.NumberOfSlices;
            SpatialResource spatialResource = GetFirstFreeSpectrumOnPath(path);
            int firstSlice = spatialResource.MinFreeSliceNumber;
            foreach (int e in path.EdgesBelongingToPath)
            {
                Edge edge = _edges[e];
                //correct allocation on a path is guaraanted as the parameters don't change
                edge.SpectrumEdgeAllocator.Allocate(firstSlice, firstSlice + spectrumSize, spatialResource);
            }
        }

        public void Erase()
        {
            foreach (var e in _edges)
                e.Value.SpectrumEdgeAllocator.Erase();
        }
    }

    class SpatialResource
    {
        public List<int> Slices;
        public int SpectrumSize;
        public int MinFreeSliceNumber;

        public SpatialResource()
        {
            Slices = new List<int>();
            SpectrumSize = 0;
            MinFreeSliceNumber = 0;
        }
        public void Clear()
        {
            Slices.Clear();
            SpectrumSize = 0;
        }
    }

}

