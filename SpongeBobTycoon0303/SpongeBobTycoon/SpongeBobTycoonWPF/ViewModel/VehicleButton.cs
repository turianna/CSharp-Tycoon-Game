using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.Products;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class VehicleButton : ViewModelBase
    {
        private Vehicle v;
        private string vt;
        private string product;
        public Vehicle Vehicle
        {
            get { return v; }
            set
            {
                v = value;
            }
        }

        public string VehicleType
        {
            get { return vt; }
            set
            {
                vt = value;
            }
        }
        public string Product
        {
            get { return product; }
            set { product = value; }
        }

        public DelegateCommand? ClickCommand { get; set; }
    }
}
