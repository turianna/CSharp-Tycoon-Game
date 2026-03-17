using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.Industries
{
    public class City : Industry 
    {
        //public Product[] ingredients = Array.Empty<Product>();
        public City()
        {
            product = Product.CITIZEN;
        }
    }
}
