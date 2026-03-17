using SpongeBobTycoon.Model.FieldTypes;
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
        public Direction direction { get; set; }
        public int time;
        public int timeUntilMoves;
        public int stepsUntilBroken;
        public int repairCost;
        public int capacity;
        public int cost;


        //public (int,int) startPoint = path[0]
        public Vehicle(Product product)
        {
            this.product = product;
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
