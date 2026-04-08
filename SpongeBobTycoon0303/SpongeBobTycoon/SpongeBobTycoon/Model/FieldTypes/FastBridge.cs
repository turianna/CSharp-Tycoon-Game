using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class FastBridge : Bridge
    {
        public FastBridge(bool isRiver) : base(isRiver)
        {
            speed = 3;
            cost = 50;
        }
    }
}
