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
        public Farm(Product product) : base(product)
        {
            switch (product)
            {
                case Algae:
                    productCapacity = 50;
                    productPerProduce = 5;
                    break;

                case Citizen:
                    productCapacity = 100;
                    productPerProduce = 5;
                    break;

                case Meat:
                    productCapacity = 20;
                    productPerProduce = 4;
                    break;
                
                case Net:
                    productCapacity = 10;
                    productPerProduce = 1;
                    break;

                case Spatula:
                    productCapacity = 10;
                    productPerProduce = 1;
                    break;

                case Wood:
                    productCapacity = 50;
                    productPerProduce = 5;
                    break;
            }
        }
        public void Produce()
        {

            productionTimeLeft--;
            
            if (productionTimeLeft <= 0)
            {
                if(productAmount == productCapacity) return;

                productAmount = Math.Min(productAmount + productPerProduce, productCapacity);

                //az uj termeles ideje vagy 1-el nagyobb, 1-el kisebb, vagy uganannyi lesz, mint eddig
                int productionTimeDifference = 1 - random.Next(3); //-1, 0, vagy 1
                
                productionTime += productionTimeDifference;
                if(productionTime > maxProductionTime) productionTime = maxProductionTime;
                if(productionTime < minProductionTime) productionTime = minProductionTime;

                productionTimeLeft = productionTime;

                System.Diagnostics.Debug.WriteLine($"Produced {product.GetType()}. New amount: {productAmount}");

            }

        }
    }
}
