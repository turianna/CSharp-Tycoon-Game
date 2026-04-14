using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class VehicleButton : ViewModelBase
    {
        public Vehicle vehicle;
        
        public DelegateCommand? ClickCommand { get; set; }
    }
}
