using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public GameModel model;
        public int color_int;
        public int MovingVehicleListSize;
        public int ParkingVehicleListSize;

        private int time=0;

        private int money;
        public int Money
        {
            get => money;
            set { money = value; OnPropertyChanged(); }
        }

        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand StopTimeCommand { get; private set; }
        public DelegateCommand StartTimeCommand { get; private set; }
        public DelegateCommand SpeedUpTimeCommand { get; private set; }

        public DelegateCommand BuyVehicleCommand { get; private set; } //commandParameter-rel adjuk át a productot
        public DelegateCommand BuyBridgeCommand { get; private set; }
        public DelegateCommand BuyRoadCommand { get; private set; }
        public DelegateCommand PutStopCommand { get; private set; }
        public DelegateCommand TerraformUPCommand { get; private set; }
        public DelegateCommand TerraformDownCommand { get; private set; }
        public DelegateCommand CreateRouteCommand { get; private set; }

        public BuildMode structure;

        public ObservableCollection<GameField> Board { get; private set; }
        public ObservableCollection<Vehicle> MovingVehicleList { get; private set; }
        public ObservableCollection<Vehicle> ParkingVehicleList { get; private set; }
        public int GameTime { 
            get { 
                return time; 
            }
            set {
                time = value;
                OnPropertyChanged(nameof(GameTime));
            } 
        }


        public MainViewModel(GameModel model)
        {
            this.model = model;
            this.model.FieldChanged += Model_FieldChanged;
            this.model.TimerTick += Model_TimerTick;
            this.model.MoneyChanged += Model_MoneyChanged;
            Money = model.money;
            this.model.VehicleBought += Model_VehicleBought;
            MovingVehicleList = new ObservableCollection<Vehicle>();
            ParkingVehicleList = new ObservableCollection<Vehicle>();
            MovingVehicleListSize = model.movingVehicles.Count;
            ParkingVehicleListSize = model.parkingVehicles.Count;
            structure = BuildMode.NONE;

            BuyRoadCommand = new DelegateCommand(_ => { structure = BuildMode.ROAD; Console.WriteLine("BuildMode.Road"); });

            foreach (Vehicle vehicle in model.movingVehicles)
            {
                MovingVehicleList.Add(new Vehicle(vehicle.product));
            }
            foreach (Vehicle vehicle in model.parkingVehicles)
            {
                ParkingVehicleList.Add(new Vehicle(vehicle.product));
            }
            Board = new ObservableCollection<GameField>();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int color_int = DecideColor_int(model.Board(i, j).fieldType, model.Board(i, j).industry);

                    Board.Add(new GameField 
                    { 
                        x = i, 
                        y = j, 
                        Color = color_int, 
                        ClickCommand = new DelegateCommand(param =>
                        {
                            if (param is Tuple<int, int> pos)
                                HandleClick(pos.Item1, pos.Item2);
                        }),
                        Content = "" });
                    if(color_int == 1) Board[i * 100 + j].Content = ((Forest)model.Board(i, j).fieldType!).treeCount.ToString();
                }

            }
        }

        private void Model_VehicleBought(object? sender, Vehicle e)
        {
            foreach (Vehicle vehicle in model.parkingVehicles)
            {
                ParkingVehicleList.Add(new Vehicle(vehicle.product));
            }
        }

        private void Model_TimerTick(object? sender, int e)
        {
            GameTime = model.gameTimer;
        }

        private void Model_FieldChanged(object? sender, FieldChangedEventArgs e)
        {

            switch (e.fieldType)
            {
                case Forest:
                    ViewModelForest(e);
                    break;
                case Road:
                    Board[e.y * 100 + e.x].Color = 4;
                    break;

            }
                       
        }

        private void Model_MoneyChanged(object? sender, int e)
        {
            Money = model.money;
        }

        public void UpdateField(FieldChangedEventArgs e)
        {
            int color_int = DecideColor_int(model.Board(e.x, e.y).fieldType, model.Board(e.x, e.y).industry);

            Board[e.x * 100 + e.y].Color = color_int;
        }

        public void ViewModelForest(FieldChangedEventArgs e)
        {
            int color_int = DecideColor_int(model.Board(e.y, e.x).fieldType, model.Board(e.y, e.x).industry);

            Board[e.y * 100 + e.x].Color = color_int;

            if (model.Board(e.y, e.x).fieldType is Forest)
            {
                Forest forest = (Forest)model.Board(e.y, e.x).fieldType!;
                Board[e.y * 100 + e.x].Content = forest.treeCount.ToString();
            }
            else
            {
                Board[e.y * 100 + e.x].Content = "";
            }
        }

        public void HandleClick(int x, int y)
        {
            Console.WriteLine("HandleClick");
            switch (structure)
            {
                case BuildMode.ROAD:
                    model.BuyRoad(x, y);
                    break;

                case BuildMode.NONE:
                    break;
            }
        }

        private int DecideColor_int(FieldType? ft, Industry? ind) 
        {
            int color_int=6;

            switch (ft)
            {
                case Forest:
                    color_int = 1;
                    break;

                case Lava:
                    color_int = 0;
                    break;

                case Road:
                    color_int = 4;
                    break;

                case Grass:
                     color_int = 6;
                     break;
            }
                    switch (ind.product)
                    {
                        case Petfood:
                            color_int = 2;
                            break;

                        case Burger:
                            color_int = 2;
                            break;

                        case Wood:
                            color_int = 2;
                            break;

                        case Algae:
                            color_int = 2;
                            break;

                        case Net:
                            color_int = 2;
                            break;

                        case Spatula:
                            color_int = 2;
                            break;

                        case Meat:
                            color_int = 2;
                            break;

                        case Citizen:
                            color_int = 3;
                            break;
                    }               
                return color_int;

        } 
    }
}
