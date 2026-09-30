using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    //uristen
    public class VehicleData
    {
        public bool isFastCar { get; set; }
        public List<Point> route { get; set; } 
        public List<ProductEnum> ingredients { get; set; }
        public List<int> ingredientAmounts { get; set; }
        public List<int> ingredientCapacities { get; set; }

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
            route = new List<Point>();
            ingredients = new List<ProductEnum>();
            ingredientAmounts = new List<int>();
            ingredientCapacities = new List<int>();
            productName = "";
            typeName = "";
        }

        public VehicleData(Vehicle vehicle)
        {
            if (vehicle is FastCar) isFastCar = true; else isFastCar = false;   
                route = TupleListToPointList(vehicle.Route.ToList());
            ingredients = ProductListToProductEnumList(vehicle.Ingredients.ToList());
            ingredientAmounts = vehicle.IngredientAmounts.ToList();
            ingredientCapacities = vehicle.IngredientCapacities.ToList();
            switch (vehicle.product)
            {
                case Algae:
                    product = ProductEnum.Algae;
                    break;
                case Burger:
                    product = ProductEnum.Burger;
                    break;
                case Citizen:
                    product = ProductEnum.Citizen;
                    break;
                case Meat:
                    product = ProductEnum.Meat;
                    break;
                case Net:
                    product = ProductEnum.Net;
                    break;
                case Petfood:
                    product = ProductEnum.Petfood;
                    break;
                case Spatula:
                    product = ProductEnum.Spatula;
                    break;
                case Wood:
                    product = ProductEnum.Wood;
                    break;
            }
            productName = vehicle.productName;
            typeName = vehicle.typeName;
            productAmount = vehicle.productAmount;
            capacity = vehicle.capacity;
            ID = vehicle.ID;
            route_index = vehicle.route_index;
            direction = vehicle.direction;
            dir = vehicle.dir;
            time = vehicle.time;
            baseTime = vehicle.getBaseTime;
            timeUntilMoves = vehicle.timeUntilMoves;
            stepsUntilBroken = vehicle.stepsUntilBroken;
            repairCost = vehicle.repairCost;
            cost = vehicle.cost;
            x = vehicle.x;
            y = vehicle.y;
            brokness = vehicle.brokness;
        }

        private List<Point> TupleListToPointList(List<(int, int)> tuples)
        {
            List<Point> points = new List<Point>();
            foreach ((int, int) tuple in tuples)
            {
                points.Add(new Point(tuple.Item1, tuple.Item2));
            }
            return points;
        }

        private List<ProductEnum> ProductListToProductEnumList(List<Product> products)
        {
            List<ProductEnum> productEnums = new List<ProductEnum>();
            foreach (Product product in products)
            {
                switch (product)
                {
                    case Algae:
                        productEnums.Add(ProductEnum.Algae);
                        break;
                    case Burger:
                        productEnums.Add(ProductEnum.Burger);
                        break;
                    case Citizen:
                        productEnums.Add(ProductEnum.Citizen);
                        break;
                    case Meat:
                        productEnums.Add(ProductEnum.Meat);
                        break;
                    case Net:
                        productEnums.Add(ProductEnum.Net);
                        break;
                    case Petfood:
                        productEnums.Add(ProductEnum.Petfood);
                        break;
                    case Spatula:
                        productEnums.Add(ProductEnum.Spatula);
                        break;
                    case Wood:
                        productEnums.Add(ProductEnum.Wood);
                        break;
                }
            }
            return productEnums;
        }
    }
}
