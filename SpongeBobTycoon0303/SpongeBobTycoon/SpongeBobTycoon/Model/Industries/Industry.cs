using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;

namespace SpongeBobTycoon.Model.Industries
{
    public class Industry
    {
        protected static Random random = new Random();

        protected const int minProductionTime = 20;
        protected const int maxProductionTime = 40;
        protected int productionTime;
        public int getProductionTime => productionTime;

        protected int productionTimeLeft;
        public int getProductionTimeLeft => productionTimeLeft;

        protected int productCapacity;
        protected int productPerProduce;

        public Product product { get; set; }
        public int productAmount;
        public Industry()
        {
            product = new Product();
        }

        protected Product ProductEnumToProduct(ProductEnum productE)
        {
            switch (productE)
            {
                case ProductEnum.Net: return new Net();
                case ProductEnum.Algae: return new Algae();
                case ProductEnum.Burger: return new Burger();
                case ProductEnum.Citizen: return new Citizen();
                case ProductEnum.Meat: return new Meat();
                case ProductEnum.Petfood: return new Petfood();
                case ProductEnum.Spatula: return new Spatula();
                case ProductEnum.Wood: return new Wood();
            }

            throw new Exception();
        }

    }
}
