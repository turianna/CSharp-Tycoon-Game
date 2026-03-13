using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class Field
    {
        public int X ; public int Y; public int Z;

        public FieldType fieldType;
        public bool isVehicle;
        //public bool isCrossRoad --> ez még lehet enum lesz

        public Field(int x, int y,int z, FieldType ft, bool isVehicle)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.fieldType = ft;
            this.isVehicle = isVehicle;
        }
    }
}
