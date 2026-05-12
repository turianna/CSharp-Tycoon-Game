using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;

namespace SpongeBobTycoon.Model.Industries
{
    public class Factory : Industry
    {
        private List<Product> ingredients;
        private List<int> ingredientAmounts;
        private List<int> ingredientsForProduction;

        public List<int> getIngredientAmounts => ingredientAmounts;
        public List<int> getIngredientsForProduction => ingredientsForProduction;
        public List<Product> getIngredients => ingredients;

        public bool producing;

        public Factory(Product product) : base()
        {
            this.product = product;
            productionTime = random.Next(minProductionTime, maxProductionTime + 1);
            productionTimeLeft = productionTime;
            productAmount = 0;

            this.ingredients = new List<Product>();
            this.ingredientAmounts = new List<int>();
            this.ingredientsForProduction = new List<int>();

            producing = false;

            switch (product)
            {
                case Burger:
                    productCapacity = 10;
                    productPerProduce = 1;

                    ingredients.Add(new Spatula());
                    ingredientAmounts.Add(0);
                    ingredientsForProduction.Add(1);

                    ingredients.Add(new Meat());
                    ingredientAmounts.Add(0);
                    ingredientsForProduction.Add(8);

                    break;

                case Petfood:
                    productCapacity = 10;
                    productPerProduce = 1;

                    ingredients.Add(new Algae());
                    ingredientAmounts.Add(0);
                    ingredientsForProduction.Add(5);
                    break;

            }

        }

        public Factory(IndustryData industryData)
        {
            this.productAmount = industryData.ProductAmount;
            ingredients = new List<Product>();
            foreach(ProductEnum pe in industryData.ingredients)
            {
                ingredients.Add(ProductEnumToProduct(pe));
            }
            
            this.productionTime = industryData.productionTime;
            this.productionTimeLeft = industryData.productionTimeLeft;
            this.ingredientAmounts = industryData.ingredientAmounts;
            this.ingredientsForProduction = industryData.ingredientsForProduction;
            producing = industryData.producing;

            product = ProductEnumToProduct(industryData.product);

            switch (product)
            {
                case Burger:
                    productCapacity = 10;
                    productPerProduce = 1;
                    break;

                case Petfood:
                    productCapacity = 10;
                    productPerProduce = 1;
                    break;

            }

        }

        /// <summary>
        /// Produce
        /// Kezeli az ipari épület termelési ciklusát: ha folyik a munka, csökkenti a hátralévő időt, ha pedig áll a termelés, ellenőrzi, hogy rendelkezésre áll-e minden szükséges alapanyag (ingredients). Megfelelő alapanyagmennyiség esetén levonja azokat és elindítja a gyártást. A termelési idő lejártakor növeli a késztermék mennyiségét a tárolókapacitás erejéig, véletlenszerűen módosítja a következő ciklus hosszát a megadott minimum és maximum határok között, majd debug üzenetet küld az aktuális készletről.
        /// </summary>
        public void Produce()
        {

            if (producing)
            {
                productionTimeLeft--;
            }

            if (!producing)
            {
                for (int i = 0; i < ingredients.Count; i++)
                {
                    if (ingredientAmounts[i] < ingredientsForProduction[i]) return;
                }

                System.Diagnostics.Debug.WriteLine($"{this.product.GetType().Name} factory started producing - current product amount: {productAmount}");
                producing = true;
                productionTimeLeft = productionTime;
                
                for(int i = 0; i < ingredients.Count; i++)
                {
                    ingredientAmounts[i] -= ingredientsForProduction[i];
                }
            }
            
            if (productionTimeLeft <= 0)
            {
                producing = false;

                productAmount = Math.Min(productAmount + productPerProduce, productCapacity);

                //az uj termeles ideje vagy 1-el nagyobb, 1-el kisebb, vagy uganannyi lesz, mint eddig
                int productionTimeDifference = 1 - random.Next(3); //-1, 0, vagy 1

                productionTime = productionTime += productionTimeDifference;
                if (productionTime > maxProductionTime) productionTime = maxProductionTime;
                if (productionTime < minProductionTime) productionTime = minProductionTime;

                productionTimeLeft = productionTime;

                System.Diagnostics.Debug.WriteLine($"Produced {product.GetType().Name}. New amount: {productAmount}");
            }

        }

        /// <summary>
        /// ReceiveIngredient
        /// Fogadja és elraktározza a megadott típusú és mennyiségű alapanyagot az ipari épület számára. Ha a darabszám nulla, a metódus azonnal visszatér. Ellenkező esetben végigiterál az épület által használt alapanyagok (ingredients) listáján, és ha talál a kapott termékkel (product) megegyező típust, megnöveli annak raktárkészletét a megadott mennyiséggel, majd egy debug üzenetben naplózza az alapanyag beérkezését.
        /// </summary>
        public void ReceiveIngredient(Product product, int count) 
        {
            if(count == 0 ) return;

            for (int i = 0; i < ingredients.Count; i++) 
            {
                if (ingredients[i].GetType() != product.GetType()) continue;

                System.Diagnostics.Debug.WriteLine($"{this.product.GetType().Name} factory received {count} of {ingredients[i].GetType().Name}");
                ingredientAmounts[i] += count;
            }
        }

    }
}
