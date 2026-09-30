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
        public required Vehicle Vehicle { get; set; }
        public required string VehicleType { get; set; }
        public required string Product { get; set; }
        
        public DelegateCommand? ClickCommand { get; set; }

    }
}
