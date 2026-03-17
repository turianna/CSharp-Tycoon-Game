using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.Industries
{
    public class Industry
    {
        public Product[]? ingredients { get; set; }
        public int[]? ingredientAmounts { get; set; }

        public Product product { get; set; }
        public int productValue { get; set; }
        public int timeToProduce { get; set; }
       

    }
}
