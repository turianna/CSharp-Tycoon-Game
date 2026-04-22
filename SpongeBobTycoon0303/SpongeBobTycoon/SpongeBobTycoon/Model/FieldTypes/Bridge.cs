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
        public bool isRiver { get; set; }
        public RoadType roadType;

        public Bridge(bool isRiver) //amikor megvesszük a hidat ki kell választanunk azt is, hogy folyóra vagy tóra vesszük, majd onnan, hogy gyors vagy lassú
        {
            this.isRiver = isRiver;
        }

    }
}
