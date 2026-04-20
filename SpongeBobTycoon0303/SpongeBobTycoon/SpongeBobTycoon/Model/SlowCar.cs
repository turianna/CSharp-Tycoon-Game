using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class SlowCar : Vehicle
    {
        public SlowCar(Product product) : base(product)
        {
            this.product = product;
            cost = 100;
            time = 5;
            stepsUntilBroken = 45;
            repairCost = 50;
            brokness = stepsUntilBroken;
        }

    }
}
