using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model.Products;

namespace SpongeBobTycoon.Model.Industries
{
    public class Factory : Industry
    {
        public List<Product> ingredients { get; set; }
        public Factory(List<Product> ingredients, Product product)
        {
            this.product = product;
            this.ingredients = ingredients;
        }

        public void Produce() { }


    }
}
