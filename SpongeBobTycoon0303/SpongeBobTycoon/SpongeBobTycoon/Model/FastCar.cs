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
        }
    }
}
