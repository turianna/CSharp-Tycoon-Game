using SpongeBobTycoon.Model.FieldTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class RouteManager
    {
        public Vehicle? vehicle {  get; set; }
        public List<Stop>? stops { get; set; }

        public RouteManager() 
        { 
        
        }

        public bool ValidRoute()
        {
            //min 2 és muszáj érintenie azt a megállót amit szállít az autó és körút-e és jó irányba megy-e az útvonal
        }

        public void DesignRoute()
        {
            //meghívja a legrövidebb utas metódust a kiválasztott megállókra

        }
    }
}
