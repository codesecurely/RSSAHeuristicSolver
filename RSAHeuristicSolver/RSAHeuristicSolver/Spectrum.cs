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
        private List<int> _spectrum;
        private int _spectrumSize;

        public int SpectrumSize
        {
            get { return _spectrumSize; }
        }

        public SpectrumEdgeAllocator()
        {
            _spectrum = new List<int>();
            _spectrumSize = 0;
        }

        public bool IsFree(int start, int end)
        {
            while (_spectrum.Count < end)
                _spectrum.Add(0);
            for (int channel = start; channel < end; channel++)
                if (_spectrum[channel] == 1)
                    return false;
            return true;
        }

        public void Allocate(int start, int end)
        {
            if (IsFree(start, end))
            {
                for (int channel = start; channel < end; channel++)
                {
                    _spectrum[channel] = 1;
                }
                _spectrumSize = end;
            }
        }

        public int GetFirstFreeSlice(int size)
        {
            int channel = 0;
            for (channel = 0; channel <= _spectrum.Count; channel++)
                if (IsFree(channel, channel + _spectrumSize))
                    return channel;
            return 0;
        }

        public void Erase()
        {
            _spectrum.Clear();
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

        public int GetFirstFreeSpectrumOnPath(Path path)
        {
            int minSpectrum = 0;
            bool keepAllocating = true;
            while (keepAllocating)
            {
                keepAllocating = false;
                foreach (int e in path.EdgesBelongingToPath)
                {
                    Edge edge = _edges[e];
                    int firstFreeSpectrum = edge.SpectrumEdgeAllocator.GetFirstFreeSlice(path.NumberOfSlices);
                    if (firstFreeSpectrum > minSpectrum)
                    {
                        minSpectrum = firstFreeSpectrum;
                        keepAllocating = true;
                    }
                }
            }
            return minSpectrum;
        }

        //allocated channel must be the same for each edge on a path
        public void AllocateFirstFreeSpectrumOnPath(Path path)
        {
            int spectrumSize = path.NumberOfSlices;
            int start = GetFirstFreeSpectrumOnPath(path);
            foreach (int e in path.EdgesBelongingToPath)
            {
                Edge edge = _edges[e];
                edge.SpectrumEdgeAllocator.Allocate(start, start + spectrumSize);
            }
        }

        public void Erase()
        {
            foreach (var e in _edges)
                e.Value.SpectrumEdgeAllocator.Erase();
        }
    }
}