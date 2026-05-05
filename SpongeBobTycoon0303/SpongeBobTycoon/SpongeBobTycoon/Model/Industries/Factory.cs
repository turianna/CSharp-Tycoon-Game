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
        private List<Product> ingredients;
        private List<int> ingredientAmounts;
        private List<int> ingredientsForProduction;

        public List<int> getIngredientAmounts => ingredientAmounts;
        public List<int> getIngredientsForProduction => ingredientsForProduction;
        public List<Product> getIngredients => ingredients;

        bool producing;

        public Factory(Product product) : base(product)
        {
            this.ingredients = new List<Product>();
            this.ingredientAmounts = new List<int>();
            this.ingredientsForProduction = new List<int>();

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
                    ingredientsForProduction.Add(10);
                    break;

            }

        }

        public void Produce()
        {

            for (int i = 0; i < ingredients.Count; i++)
            {
                if (ingredientAmounts[i] < ingredientsForProduction[i] && !producing) return;
            }

            if (producing)
            {
                productionTimeLeft--;
            }

            if (!producing)
            {
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

                //System.Diagnostics.Debug.WriteLine($"Produced {product.GetType().Name}. New amount: {productAmount}");
            }

        }

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
