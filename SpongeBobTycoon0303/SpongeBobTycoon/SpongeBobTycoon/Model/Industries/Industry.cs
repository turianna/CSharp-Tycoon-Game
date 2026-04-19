using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model.Products;

namespace SpongeBobTycoon.Model.Industries
{
    public class Industry
    {
        protected static Random random = new Random();

        protected const int minProductionTime = 20;
        protected const int maxProductionTime = 40;
        protected int productionTime;

        protected int productionTimeLeft;

        protected int productCapacity;
        protected int productPerProduce;

        public Product product { get; set; }
        protected int productAmount;
        public Industry(Product product)
        {
            this.product = product;
            productionTime = random.Next(minProductionTime, maxProductionTime + 1);
            productionTimeLeft = productionTime;
            productAmount = 0;
        }

        

    }
}
