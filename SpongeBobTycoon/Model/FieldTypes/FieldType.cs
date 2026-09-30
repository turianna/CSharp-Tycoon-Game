using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class FieldType
    {
        public int X;
        public int Y;


        public FieldType() { }
        public FieldType(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }
    }
}
