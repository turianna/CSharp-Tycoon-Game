using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class FastBridge : Bridge
    {
        public int cost = 400;
        public int speed = 3;
        public FastBridge(bool isRiver) : base(isRiver)
        {
        }
    }
}
