using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class Forest
    {
        public int X; public int Y;
        public int treeCount;
        public int timeUntilNewTree;
        
        public void Grow() { }
        public void Spread(GameModel model) { }

    }
}
