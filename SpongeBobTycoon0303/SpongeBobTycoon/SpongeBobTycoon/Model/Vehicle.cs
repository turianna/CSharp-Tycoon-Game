using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class Vehicle 
    {
        public List<(int, int)>? route {  get; set; }
        public Product ingredient;
        public Product product;
        public string productName;
        public string typeName;
        public int ID;
        public int route_index;
        
        public Direction direction { get; set; }
        public int time;
        public int timeUntilMoves;
        public int stepsUntilBroken;
        public int repairCost;
        public int capacity;
        public int cost;
        public int x;
        public int y;
        public int brokness;


        //public (int,int) startPoint = path[0]
        public Vehicle(Product product)
        {
            this.product = product;
            route = new List<(int, int)>();

            switch (product)
            {
                case Algae:
                    productName = "Algee";
                    break;
                case Burger:
                    productName = "Burger";
                    break;
                case Citizen:
                    productName = "Citizen";
                    break;
                case Meat:
                    productName = "Meat";
                    break;
                case Net:
                    productName = "Net";
                    break;
                case Petfood:
                    productName = "Petfood";
                    break;
                case Spatula:
                    productName = "Spatula";
                    break;
                case Wood:
                    productName = "Wood";
                    break;
            }
            

        }

        public void Step(GameModel model, int x, int y)
        {
            
            if (model.board[x,y].fieldType is Stop)
            {
                foreach(Stop s in model.stops)
                {
                    if(s.X == x && s.Y == y)
                    {
                        Transaction(s);
                    }
                }
            }


            /*foreach (Vehicle car in vehicles) {
                car.stepsUntilBroken--;
                if (car.stepsUntilBroken == 0)
                {
                    money -= 100;
                    car.stepsUntilBroken = 50000;
                }
            } ez majd a vehicle step-jébe kerül*/
        }

        public void Transaction(Stop currentStop)
        {

        }
    }
}
