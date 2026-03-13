using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public GameModel model;

        public DelegateCommand NewGameCommand {  get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand StopTimeCommand { get; private set; }
        public DelegateCommand StartTimeCommand { get; private set; }
        public DelegateCommand SpeedUpTimeCommand { get; private set; }

        public DelegateCommand BuyVehicleCommand {  get; private set; } //commandParameter-rel adjuk át a productot
        public DelegateCommand BuyBridgeCommand { get; private set; }
        public DelegateCommand BuyRoadCommand { get; private set; }
        public DelegateCommand PutStopCommand { get; private set; }
        public DelegateCommand TerraformCommand { get; private set; }
        public DelegateCommand CreateRouteCommand { get; private set; }

       

        public ObservableCollection<GameField> Board { get; private set; }

        public MainViewModel(GameModel model)
        {
            this.model = model;
        }
    }
}
