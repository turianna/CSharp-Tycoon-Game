using SpongeBobTycoon.Model.Industries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class Stop : FieldType
    { /*
        //lehet ezeket egy dictionary-ben kéne tárolni, hogy ne essenek szét a párok (mármint hogy melyik ingredientből mennyi van)
        //public Product[] ingredient { get; set; }
        //public Product ingredient2 { get; set; } //ez nem kell, bővíthető legyen az ingredientek száma (tömb)
        //public Product product { get; set; }
        public int? ingredientAmount { get; set; } //ez csak azokhoz kell akiknek van szüksége alapanyagra
        //public int? ingredientAmount2 { get; set; } //ez csak azokhoz kell akiknek van szüksége alapanyagra
        public int productAmount { get; set; }
        public int timeToProduce { get; set; }
        public FieldType fieldType { get; set; }

        public Stop(Product product, int x, int y, FieldType fieldType) 
        {
            //this.product = product;
            X = x;
            Y = y;
            if(product == Product.NET || product == Product.WOOD) //ha nyersanyagot termel akkor nincs szüksége más anyagra a termeléshez
            {
                //ingredient = [];
                //ingredient2 = Product.EMPTY;
                ingredientAmount = null;
                //ingredientAmount2 = null;
            }

        } */

        public Industry industry;

        public bool isVehicle;
        public Stop(int x, int y, Industry industry) : base(x, y) 
        {
            this.industry = industry;
        }
 


    }
}
