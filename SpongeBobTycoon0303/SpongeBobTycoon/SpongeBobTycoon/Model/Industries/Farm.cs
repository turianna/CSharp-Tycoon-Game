using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model.Products;

namespace SpongeBobTycoon.Model.Industries
{
    public class Farm : Industry
    {
        public Farm(Product product) 
        { 
            this.product = product;
        }

        public void Produce() { }
    }
}
