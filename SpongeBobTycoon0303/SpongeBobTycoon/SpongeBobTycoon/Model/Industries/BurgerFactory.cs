using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.Industries
{
    public class BurgerFactory : Industry
    {
        public BurgerFactory()
        {
            ingredients = new Product[2]; //legyen inkább lista?
            ingredientAmounts = new int[2]; 
            ingredients[0] = Product.MEAT;
            ingredients[1] = Product.SPATULA;
            ingredientAmounts[0] = 0;
            ingredientAmounts[1] = 0;
            product = Product.BURGER;
        }
    }
}
