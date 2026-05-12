using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Persistence;

namespace SpongeBobTycoon.Model
{
    public class FastCar : Vehicle
    {
        public FastCar(VehicleData vd) : base(vd) { }
        public FastCar(Product product) : base(product)
        {
            this.product = product;
            baseTime = 1;
            timeUntilMoves = baseTime;
            cost = 200;

            switch (product)
            {
                case Algae:
                    capacity = 10;
                    break;

                case Burger:
                    capacity = 3;
                    break;

                case Citizen:
                    capacity = 10;
                    break;

                case Meat:
                    capacity = 20;
                    break;

                case Spatula:
                    capacity = 5;
                    break;

                case Petfood:
                    capacity = 10;
                    break;

                case Net:
                    capacity = 10;
                    break;

                case Wood:
                    capacity = 20;
                    break;
            }
            stepsUntilBroken = 60;
            repairCost = 90;
            brokness = stepsUntilBroken;
        }
    }
}
