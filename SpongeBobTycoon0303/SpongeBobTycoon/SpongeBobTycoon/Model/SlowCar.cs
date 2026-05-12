using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class SlowCar : Vehicle
    {
        public SlowCar(VehicleData vd) : base(vd) { }
        public SlowCar(Product product) : base(product)
        {
            this.product = product;
            cost = 200;
            baseTime = 2;
            timeUntilMoves = baseTime;

            switch (product)
            {
                case Algae:
                    capacity = 20;
                    break;

                case Burger:
                    capacity = 6;
                    break;

                case Citizen:
                    capacity = 20;
                    break;

                case Meat:
                    capacity = 30;
                    break;

                case Spatula:
                    capacity = 10;
                    break;

                case Petfood:
                    capacity = 15;
                    break;

                case Net:
                    capacity = 20;
                    break;

                case Wood:
                    capacity = 40;
                    break;
            }

            stepsUntilBroken = 45;
            repairCost = 50;
            brokness = stepsUntilBroken;
        }

    }
}
