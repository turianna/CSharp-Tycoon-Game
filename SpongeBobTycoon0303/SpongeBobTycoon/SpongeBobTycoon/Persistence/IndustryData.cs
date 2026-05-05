using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    //fat DTO - stores factory and farm data at the same time
    public class IndustryData
    {
        public int ProductAmount { get; set; }
        public int ProductValue { get; set; }
        public List<ProductEnum> ingredients { get; set; }
        public int productionTimeLeft { get; set; }
        public List<int> ingredientAmounts { get; set; }
        public List<int> ingredientsForProduction { get; set; }

        public IndustryData(Industry industry)
        {
            ingredientAmounts = new List<int>();
            ingredientsForProduction = new List<int>();
            ingredients = new List<ProductEnum>();

            if(industry is null) return;

            this.ProductAmount = industry.productAmount;
            this.ProductValue = industry.product.value;
            this.productionTimeLeft = industry.getProductionTimeLeft;

            if (industry is Factory)
            {
                Factory factory = (Factory)industry;

                this.ingredientAmounts = factory.getIngredientAmounts;
                this.ingredientsForProduction = factory.getIngredientsForProduction;
                foreach (Product p in factory.getIngredients)
                {
                    this.ingredients.Add(productToEnum(p));
                }
            }

        }

        private ProductEnum productToEnum(Product p)
        {
            switch (p)
            {
                case Algae:
                    return ProductEnum.Algae;
                case Burger:
                    return ProductEnum.Burger;
                case Citizen:
                    return ProductEnum.Citizen;
                case Meat:
                    return ProductEnum.Meat;
                case Net:
                    return ProductEnum.Net;
                case Petfood:
                    return ProductEnum.Petfood;
                case Spatula:
                    return ProductEnum.Spatula;
                case Wood:
                    return ProductEnum.Wood;
            }

            return 0;
        }
    }
}
