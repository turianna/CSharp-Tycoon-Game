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


        //public (int,int) startPoint = path[0]
        public void Step(GameModel model, int x, int y)
        {
            
            if (model.board[x,y].fieldType == FieldType.STOP)
            {
                foreach(Stop s in model.stops)
                {
                    if(s.X == x && s.Y == y)
                    {
                        Transaction(s);
                    }
                }
            }
        }

        public void Transaction(Stop currentStop)
        {

        }
    }
}
