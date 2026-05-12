using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class SlowBridge : Bridge
    {
        public int speed = 5;
        public int cost = 200;
        public SlowBridge(bool isRiver) : base(isRiver)
        {
        }
    }
}
