using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSAHeuristicSolver
{
    //We can use different classes to sort by a different metric. It's a matter of simply instantiating the class and passing it to the Sort method from IComparer interface
    //Distance – the length (in km) of the demand’s shortest path;
    class DemandDistanceSorter : IComparer<Demand>
    {
        public int Compare(Demand d1, Demand d2)
        {
            int comparePathLen = d1.GetShortestPath().CompareTo(d2.GetShortestPath());
            return comparePathLen;
        }
    }
}
