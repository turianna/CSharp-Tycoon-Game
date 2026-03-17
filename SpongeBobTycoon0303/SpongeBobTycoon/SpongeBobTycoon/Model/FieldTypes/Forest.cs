using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class Forest : FieldType
    {
        public int treeCount;
        public int timeUntilNewTree;
        
        public void Grow() { }
        public void Spread(GameModel model) { }

    }
}
