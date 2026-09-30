using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class RouteData
    {
        public (int x, int y) Previous { get; }
        public (int x, int y) Current { get; }
        public int DistanceFromStart { get;  }

        public RouteData((int x, int y) previous,(int x, int y) current, int d) 
        {
            Previous = previous;
            Current = current;
            DistanceFromStart = d;
        
        }
    }
}
