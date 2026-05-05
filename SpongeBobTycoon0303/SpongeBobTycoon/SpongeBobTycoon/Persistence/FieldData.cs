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
        public IndustryData IndustryData { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int z { get; set; }
        public bool isVehicle { get; set; }
        public bool areTwoVehicles { get; set; }
        public bool isStop { get; set; }
        public int id { get; set; }

        public FieldData(Field f)
        {
            FieldTypeData = new FieldTypeData();
            IndustryData = new IndustryData(f.industry);

            this.x = f.X;
            this.y = f.Y;
            this.z = f.Z;
            this.isVehicle = f.isVehicle;
            this.areTwoVehicles = f.areTwoVehicles;
            this.isStop = f.isStop;
            this.id = f.id;

            FieldTypeData ftd = new FieldTypeData();
            switch (f.fieldType)
            {
                case Bridge:
                    ftd.cost = ((Bridge)f.fieldType!).cost;
                    ftd.isRiver = ((Bridge)f.fieldType!).isRiver;
                    ftd.speed = ((Bridge)f.fieldType!).speed;
                    ftd.roadType = ((Bridge)f.fieldType!).roadType;
                    break;
                case Road:
                    ftd.direction = ((Road)f.fieldType!).direction;
                    ftd.vehicle_dir = ((Road)f.fieldType!).vehicle_dir;
                    break;
                case Forest:
                    ftd.treeCount = ((Forest)f.fieldType!).treeCount;
                    ftd.timeUntilNewTree = ((Forest)f.fieldType!).getTimeUntilNewTree();
                    ftd.timeUntilSpread = ((Forest)f.fieldType!).getTimeUntilSpread();
                    break;
            }
            this.FieldTypeData = ftd;
        }


    }
}
