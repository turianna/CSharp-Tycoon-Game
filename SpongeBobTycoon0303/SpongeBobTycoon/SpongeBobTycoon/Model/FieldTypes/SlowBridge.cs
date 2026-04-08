using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class SlowBridge : Bridge
    {
        public SlowBridge(bool isRiver) : base(isRiver)
        {
            speed = 5;
            cost = 30;
        }
    }
}
