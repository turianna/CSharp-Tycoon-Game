using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class Lava :FieldType //water
    {
        public bool isRiver;
        
        public Lava(bool isRiver)
        {
            this.isRiver = isRiver;
        }
    }
}
