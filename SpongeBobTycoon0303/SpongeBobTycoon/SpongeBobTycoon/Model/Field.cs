//using Microsoft.VisualBasic.FileIO;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.FieldTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Persistence;

namespace SpongeBobTycoon.Model
{
    public class Field
    {
        public int X; public int Y; public int Z;

        public FieldType fieldType;
        public Industry industry; 
        public bool isVehicle;
        public bool areTwoVehicles;
        public Direction direction;
        public bool isStop;
        public int id; // A viewhoz kell hogy jó képet rakjak az industrieshoz 
        //public bool isCrossRoad --> ez még lehet enum lesz

        public Field(int x, int y,int z, Industry industry, FieldType ft, bool isVehicle, bool areTwoVehicles, bool isStop, int id, Direction direction)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
            this.industry = industry;
            this.fieldType = ft;
            this.isVehicle = isVehicle; //ennek is csak simán falsenak kéne lennie, szerintem nem kell a paraméter
            this.areTwoVehicles = areTwoVehicles; //ennek is csak simán falsenak kéne lennie, szerintem nem kell a paraméter
            this.isStop = isStop;
            this.id = id;
            this.direction = direction;
        }

        public Field(FieldData fieldData, List<Industry> industries)
        {
            this.X = fieldData.x;
            this.Y = fieldData.y;
            this.Z = fieldData.z;
            this.isVehicle = fieldData.isVehicle;
            this.areTwoVehicles = fieldData.areTwoVehicles;
            this.isStop = fieldData.isStop;
            this.direction = fieldData.direction;
            this.id = fieldData.id;


            if (fieldData.industryIndex == -1) industry = new EmptyIndustry();
            else industry = industries[fieldData.industryIndex];

            fieldType = new FieldType();
            FieldTypeData ftd = fieldData.FieldTypeData;
            switch (ftd.fieldType)
            {
                case FieldTypeEnum.Bridge:
                    bool isRiver = ftd.isRiver;

                    if (ftd.isFastBridge)
                    {
                        this.fieldType = new FastBridge(isRiver);
                    }
                    else
                    {
                        this.fieldType = new SlowBridge(isRiver);
                    }
                    break;
                
                case FieldTypeEnum.Road:
                    Road road = new Road();
                    road.direction = ftd.direction;
                    road.vehicle_dir = ftd.vehicle_dir;
                    road.roadType = ftd.roadType;
                    this.fieldType = road;
                    break;
                
                case FieldTypeEnum.Forest:
                    Forest forest = new Forest();
                    forest.treeCount = ftd.treeCount;
                    forest.timeUntilNewTree = ftd.timeUntilNewTree;
                    forest.timeUntilSpread = ftd.timeUntilSpread;
                    this.fieldType = forest;
                    break;
                
                case FieldTypeEnum.Lava:
                    Lava lava = new Lava(ftd.isRiver);
                    this.fieldType = lava;
                    break;
                
                case FieldTypeEnum.Stop:
                    Stop stop = new Stop(fieldData.x, fieldData.y, industries[fieldData.industryIndex]);
                    this.fieldType = stop;
                    break;
                
                case FieldTypeEnum.Factory:
                    this.fieldType = new FactoryField();
                    break;

                case FieldTypeEnum.Grass:
                    this.fieldType = new Grass();
                    break;
            }

        }
    }
}
