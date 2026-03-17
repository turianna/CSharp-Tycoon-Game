using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class Bridge : FieldType
    {
        public int cost { get; set; }
        public int speed { get; set; }
        public bool forRiver { get; set; }
        public Bridge() //amikor megvesszük a hidat ki kell választanunk azt is, hogy folyóra vagy tóra vesszük, majd onnan, hogy gyors vagy lassú
        {
            
        }

    }
}
