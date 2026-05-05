using SpongeBobTycoon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    //uristen
    public class VehicleData
    {
        public List<(int, int)>? route { get; set; } 
        public List<ProductEnum>? ingredients { get; set; }
        public List<int>? ingredientAmounts { get; set; }
        public List<int>? ingredientCapacities { get; set; }

        public ProductEnum product { get; set; }
        public string productName { get; set; }
        public string typeName { get; set; }
        public int productAmount { get; set; }
        public int capacity { get; set; }
        public int ID { get; set; }
        public int route_index { get; set; }

        public Direction direction { get; set; }
        public int dir { get; set; }
        public int time { get; set; }
        public int baseTime { get; set; }
        public int timeUntilMoves { get; set; }
        public int stepsUntilBroken { get; set; }
        public int repairCost { get; set; }
        public int cost { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int brokness { get; set; }

        public VehicleData()
        {
            
        }

    }
}
