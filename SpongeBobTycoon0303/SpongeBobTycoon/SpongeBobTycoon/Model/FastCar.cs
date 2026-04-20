using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class FastCar : Vehicle
    {
        public FastCar(Product product) : base(product)
        {
            this.product = product;
            time = 3;
            cost = 200;
            stepsUntilBroken = 60;
            repairCost = 90;
            brokness = stepsUntilBroken;
        }
    }
}
