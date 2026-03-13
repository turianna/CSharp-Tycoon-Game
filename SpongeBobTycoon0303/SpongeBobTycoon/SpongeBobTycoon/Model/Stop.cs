using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class Stop
    {
        public Product ingredient { get; set; }
        public Product ingredient2 { get; set; }
        public Product product { get; set; }
        public int X;
        public int Y;
        public int? ingredientAmount { get; set; } //ez csak azokhoz kell akiknek van szüksége alapanyagra
        public int? ingredientAmount2 { get; set; } //ez csak azokhoz kell akiknek van szüksége alapanyagra
        public int productAmount { get; set; }
        public int timeToProduce { get; set; }

        public Stop(Product product, int x, int y, FieldType fieldType) 
        {
            this.product = product;
            this.X = x;
            this.Y = y;
            if(product == Product.NET || product == Product.WOOD) //ha nyersanyagot termel akkor nincs szüksége más anyagra a termeléshez
            {
                ingredient = Product.EMPTY;
                ingredient2 = Product.EMPTY;
                ingredientAmount = null;
                ingredientAmount2 = null;
            }

        }

        
 


    }
}
