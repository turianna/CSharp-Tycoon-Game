using SpongeBobTycoon.Model.Industries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class Stop : FieldType
    { 
        public Industry industry;
        //public bool isVehicle { get; private set; }
        public Stop(int x, int y, Industry industry) : base(x, y) 
        {
            this.industry = industry;
        }
    }
}
