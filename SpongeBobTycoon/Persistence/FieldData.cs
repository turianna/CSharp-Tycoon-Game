using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    public class FieldData
    {
        public FieldTypeData FieldTypeData { get; set; }
        public int industryIndex { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public bool isVehicle { get; set; }
        public bool areTwoVehicles { get; set; }
        public bool isStop { get; set; }
        public int id { get; set; }
        public Direction direction { get; set; }

        public FieldData()
        {
            FieldTypeData = new FieldTypeData();
        }

        public FieldData(Field f, int industryIndex)
        {
            this.industryIndex = industryIndex;
            this.x = f.X;
            this.y = f.Y;
            this.z = f.Z;
            this.isVehicle = f.isVehicle;
            this.areTwoVehicles = f.areTwoVehicles;
            this.isStop = f.isStop;
            this.id = f.id;
            this.direction = f.direction;
            this.FieldTypeData = new FieldTypeData(f.fieldType, industryIndex);
        }


    }
}
