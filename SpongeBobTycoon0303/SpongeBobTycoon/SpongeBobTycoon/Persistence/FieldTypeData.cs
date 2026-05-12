using SpongeBobTycoon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SpongeBobTycoon.Model.FieldTypes;
using System.Runtime.CompilerServices;

namespace SpongeBobTycoon.Persistence
{
    public enum FieldTypeEnum
    {
        Bridge,
        Lava,
        Forest,
        Road,
        Stop,
        Grass,
        Factory
    }

    //fat DTO - az osszes lehetseges fieldtype adatot tartalmazza
    public class FieldTypeData
    {
        public FieldTypeEnum fieldType { get; set; }

        //bridge
        public bool isRiver { get; set; }
        public RoadType roadType { get; set; }
        public bool isFastBridge { get; set; }

        //lava
        //isRiver

        //forest
        public int treeCount { get; set; }
        public int timeUntilNewTree { get; set; }
        public int timeUntilSpread { get; set; }

        //road
        public Direction direction { get; set; }
        //roadType
        public int vehicle_dir { get; set; }

        //stop
        public int industryIndex { get; set; }

        public FieldTypeData()
        {

        }

        public FieldTypeData(FieldType fieldType, int industryIndex)
        {
            if (fieldType is null) return;
            switch (fieldType)
            {
                case FastBridge:
                    FastBridge bridge = (FastBridge)fieldType;
                    this.fieldType = FieldTypeEnum.Bridge;
                    this.isFastBridge = true;
                    this.isRiver = bridge.isRiver;
                    this.roadType = bridge.roadType;
                    break;
                case SlowBridge:
                    SlowBridge sbridge = (SlowBridge)fieldType;
                    this.fieldType = FieldTypeEnum.Bridge;
                    this.isFastBridge = false;
                    this.isRiver = sbridge.isRiver;
                    this.roadType = sbridge.roadType;
                    break;
                case Road:
                    this.fieldType = FieldTypeEnum.Road;
                    Road road = (Road)fieldType;
                    this.direction = road.direction;
                    this.vehicle_dir = road.vehicle_dir;
                    this.roadType = road.roadType;
                    break;
                case Forest:
                    this.fieldType = FieldTypeEnum.Forest;
                    Forest forest = (Forest)fieldType;
                    this.treeCount = forest.treeCount;
                    this.timeUntilNewTree = forest.timeUntilNewTree;
                    this.timeUntilSpread = forest.timeUntilSpread;
                    break;
                case Lava:
                    this.fieldType = FieldTypeEnum.Lava;
                    this.isRiver = ((Lava)fieldType).isRiver;
                    break;
                case Stop:
                    this.fieldType = FieldTypeEnum.Stop;
                    this.industryIndex = industryIndex;
                    break;
                case Grass:
                    this.fieldType = FieldTypeEnum.Grass;
                    break;
                case FactoryField:
                    this.fieldType = FieldTypeEnum.Factory;
                    break;
            }
        }

    }
}
