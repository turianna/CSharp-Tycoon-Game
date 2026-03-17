//using Microsoft.VisualBasic.FileIO;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.FieldTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class Field
    {
        public int X; public int Y; public int Z;

        public FieldType? fieldType;
        public Industry? industry; 
        public bool isVehicle;
        public bool isStop;
        //public bool isCrossRoad --> ez még lehet enum lesz

        public Field(int x, int y,int z, Industry? industry, FieldType ft, bool isVehicle, bool isStop)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.industry = industry;
            this.fieldType = ft;
            this.isVehicle = isVehicle; //ennek is csak simán falsenak kéne lennie, szerintem nem kell a paraméter
            this.isStop = isStop;   
        }
    }
}
