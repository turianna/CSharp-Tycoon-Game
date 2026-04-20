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

            switch (product)
            {
                case Algae:
                    capacity = 40;
                    break;

                case Burger:
                    capacity = 6;
                    break;

                case Citizen:
                    capacity = 20;
                    break;

                case Meat:
                    capacity = 40;
                    break;

                case Spatula:
                    capacity = 10;
                    break;

                case Wood:
                    capacity = 40;
                    break;
            }
        }

    }
}
