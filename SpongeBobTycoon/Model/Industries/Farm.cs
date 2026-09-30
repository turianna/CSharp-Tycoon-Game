using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;

namespace SpongeBobTycoon.Model.Industries
{
    public class Farm : Industry
    {
        public Farm(Product product) : base()
        {
            this.product = product;
            productionTime = random.Next(minProductionTime, maxProductionTime + 1);
            productionTimeLeft = productionTime;
            productAmount = 0;

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

        public Farm(IndustryData industryData)
        {
            this.productionTime = industryData.productionTime;
            this.productionTimeLeft = industryData.productionTimeLeft;
            this.productAmount = industryData.ProductAmount;
            product = ProductEnumToProduct(industryData.product);

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
        /// <summary>
        /// Kezeli az ipari egység folyamatos termelési ciklusát: minden híváskor csökkenti a hátralévő gyártási időt, majd az idő lejártakor növeli a késztermék mennyiségét a tárolókapacitás (productCapacity) felső határáig. A sikeres gyártási fázis után a következő ciklus alapidejét véletlenszerűen módosítja (-1, 0 vagy +1 értékkel), ügyelve arra, hogy az a megadott minimum és maximum határok között maradjon, végül pedig újraindítja a visszaszámlálót az újonnan kalkulált időértékre.
        /// </summary>
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

                //System.Diagnostics.Debug.WriteLine($"Produced {product.GetType()}. New amount: {productAmount}");

            }

        }
    }
}
