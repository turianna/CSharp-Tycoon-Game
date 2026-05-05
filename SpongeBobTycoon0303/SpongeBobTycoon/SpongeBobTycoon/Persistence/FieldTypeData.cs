using SpongeBobTycoon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    //fat DTO - az osszes lehetseges fieldtype adatot tartalmazza
    public class FieldTypeData
    {
        //bridge
        public int cost { get; set; }
        public int speed { get; set; }
        public bool isRiver { get; set; }
        public RoadType roadType { get; set; }

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
        public bool isVehicle { get; set; }


    }
}
