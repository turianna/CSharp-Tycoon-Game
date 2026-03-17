using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.Industries
{
    public class PetfoodFactory : Industry
    {
        public PetfoodFactory()
        {
            ingredients = new Product[1];
            ingredients[0] = Product.ALGAE;
            product = Product.PETFOOD;
        }
        
    }
}
