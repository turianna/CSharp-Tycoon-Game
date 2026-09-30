using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    public class StopData
    {
        public int x { get; set; }
        public int y { get; set; }

        public int industryIndex { get; set; }

        public StopData(int x, int y, int indIndex)
        {
            this.x = x;
            this.y = y;
            this.industryIndex = indIndex;
        }

        public StopData()
        {
            
        }
    }
}
