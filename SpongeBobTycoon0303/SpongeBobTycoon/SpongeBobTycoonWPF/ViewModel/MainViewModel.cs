using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoonWPF.View;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
//using System.DirectoryServices.ActiveDirectory;
using System.Linq;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

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

        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand QuitCommand { get; private set; }
        public DelegateCommand StopTimeCommand { get; private set; }
        public DelegateCommand StartTimeCommand { get; private set; }
        public DelegateCommand SpeedUpTimeCommand { get; private set; }

        public DelegateCommand TerraformUpCommand { get; private set; }
        public DelegateCommand TerraformDownCommand { get; private set; }

        public DelegateCommand CreateRouteCommand { get; private set; }

        public DelegateCommand BuyRoadCommand { get; private set; }
        public DelegateCommand PutStopCommand { get; private set; }
        public DelegateCommand ExitBuildCommand { get; private set; } //ehhez még kell gomb

        public DelegateCommand BuyTruckCommand { get; private set; }
        public DelegateCommand BuyVanCommand { get; private set; }
        public DelegateCommand BuyBridgeCommand { get; private set; }
        
        public DelegateCommand BuyRiverFastBridgeCommand { get; private set; }
        public DelegateCommand BuyRiverSlowBridgeCommand { get; private set; }
        public DelegateCommand BuyLakeFastBridgeCommand { get; private set; }
        public DelegateCommand BuyLakeSlowBridgeCommand { get; private set; }


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
        private void StartGame(object param)
        {
            MainWindow gameWindow = new MainWindow();
            System.Windows.Application.Current.MainWindow = gameWindow;
            gameWindow.Loaded += async (s, e) =>
            {
                await Task.Delay(50);
                gameWindow.DataContext = this;
            };
            gameWindow.Show();
            if (param is Window menuWindow)
            {
                menuWindow.Close();
            }
        }

        public MainViewModel(GameModel model)
        {
            this.model = model;
            Money = model.money;

            this.model.FieldChanged += Model_FieldChanged;
            this.model.TimerTick += Model_TimerTick;
            this.model.MoneyChanged += Model_MoneyChanged;
            this.model.VehicleBought += Model_VehicleBought;
            this.model.GameOver += Model_GameOver;
            this.model.Refresh += Model_Refresh;

            StartCommand = new DelegateCommand(param => StartGame(param));
            StopTimeCommand = new DelegateCommand(_ => StopTime());
            StartTimeCommand = new DelegateCommand(_ => StartTime());
            SpeedUpTimeCommand = new DelegateCommand(_ => SpeedUpTime());
            QuitCommand = new DelegateCommand(_ => QuitGame());
            BuyVanCommand = new DelegateCommand(param => BuyVan(param));
            BuyTruckCommand = new DelegateCommand(param => BuyTruck(param));
            BuyRoadCommand = new DelegateCommand(param => structure = BuildMode.ROAD);
            PutStopCommand = new DelegateCommand(param => structure = BuildMode.STOP);
            BuyRiverFastBridgeCommand = new DelegateCommand(param => structure = BuildMode.FASTRIVERBRIDGE);
            BuyRiverSlowBridgeCommand = new DelegateCommand(param => structure = BuildMode.SLOWRIVERBRIDGE);
            BuyLakeFastBridgeCommand = new DelegateCommand(param => structure = BuildMode.FASTLAKEBRIDGE);
            BuyLakeSlowBridgeCommand = new DelegateCommand(param => structure = BuildMode.SLOWLAKEBRIDGE);
            //ExitBuildCommand = new DelegateCommand(param => structure = BuildMode.NONE);

            MovingVehicleList = new ObservableCollection<Vehicle>();
            ParkingVehicleList = new ObservableCollection<Vehicle>();
            MovingVehicleListSize = model.movingVehicles.Count;
            ParkingVehicleListSize = model.parkingVehicles.Count;
            structure = BuildMode.NONE;


            foreach (Vehicle vehicle in model.movingVehicles)
            {
                MovingVehicleList.Add(new Vehicle(vehicle.product));
            }

            foreach (Vehicle vehicle in model.parkingVehicles)
            {
                ParkingVehicleList.Add(new Vehicle(vehicle.product));
            }

            Board = new ObservableCollection<GameField>();
            Create();
        }
           
        private void Create()
        {
            for (int i = 0; i < 50; i++) //i = y
            {
                for (int j = 0; j < 100; j++) //j = x
                {
                    int color_int = DecideColor_int(model.Board(i, j).fieldType, model.Board(i, j).industry);

                    Board.Add(new GameField
                    {
                        x = j,
                        y = i,
                        Color = color_int,
                        ClickCommand = new DelegateCommand(param =>
                        {
                            if (param is Tuple<int, int> pos)
                            {
                                HandleClick(pos.Item1, pos.Item2);
                            }
                        }),
                        Content = ""
                    });
                    if (color_int == 1) Board[i * 100 + j].Content = ((Forest)model.Board(i, j).fieldType!).treeCount.ToString();
                }

            }
        }
        
        private void StopTime()
        {
            model.StopTimer();
        }
        private void StartTime()
        {
            model.StartTimer();
        }
        private void SpeedUpTime()
        {
            model.StartTimer();
            if (model.gameTime == 4)
            {
                model.FastTime();
            }
            else if(model.gameTime == 2)
            {
                model.FastestTime();
            }
            else
            {
                model.BaseTime();
            }
        }
        private void QuitGame()
        {
            System.Windows.Application.Current.Shutdown();
        }
        private void BuyVan(object param)
        {
            switch (param)
            {
                case "Wood":
                    model.BuyVehicle(VehicleType.SlowCar, new Wood());
                    break;
                case "Burger":
                    model.BuyVehicle(VehicleType.SlowCar, new Burger());
                    break;
                case "Petfood":
                    model.BuyVehicle(VehicleType.SlowCar, new Petfood());
                    break;
                case "Net":
                    model.BuyVehicle(VehicleType.SlowCar, new Net());
                    break;
                case "Citizen":
                    model.BuyVehicle(VehicleType.SlowCar, new Citizen());
                    break;
            }
        }

        private void BuyTruck(object param)
        {
            switch (param)
            {
                case "Wood":
                    model.BuyVehicle(VehicleType.FastCar, new Wood());
                    break;
                case "Burger":
                    model.BuyVehicle(VehicleType.FastCar, new Burger());
                    break;
                case "Petfood":
                    model.BuyVehicle(VehicleType.FastCar, new Petfood());
                    break;
                case "Net":
                    model.BuyVehicle(VehicleType.FastCar, new Net());
                    break;
                case "Citizen":
                    model.BuyVehicle(VehicleType.FastCar, new Citizen());
                    break;
            }
        }


        private void Model_VehicleBought(object? sender, Vehicle e)
        {
            ParkingVehicleList.Add(new Vehicle(e.product));
            
        }

        private void Model_TimerTick(object? sender, int e)
        {
            GameTime = model.gameTimer;
        }

        private void Model_FieldChanged(object? sender, FieldChangedEventArgs e)
        {
            UpdateField(e);

            switch (e.fieldType)
            {
                case Forest:
                    //ViewModelForest(e);
                    break;

            }
                       
        }

        private void Model_Refresh(object? sender, EventArgs e)
        {
            ParkingVehicleList.Clear();
            MovingVehicleList.Clear();
            Board.Clear();
            Create();
        }
        private void Model_MoneyChanged(object? sender, int e)
        {
            Money = model.money;
        }

        private void Model_GameOver(object? sender, EventArgs e)
        {
            model.StartNewGame();
        }

        public void UpdateField(FieldChangedEventArgs e)
        {
            int color_int = DecideColor_int(model.Board(e.y, e.x).fieldType, model.Board(e.y, e.x).industry);

            Board[e.y* 100 + e.x].Color = color_int;
        }

        public void ViewModelForest(FieldChangedEventArgs e)
        {
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
            switch (structure)
            {
                case BuildMode.ROAD:
                    model.BuyRoad(x, y);
                    break;
                case BuildMode.STOP:
                    model.PutStop(x, y);
                    ViewModel_PutStop(x,y);
                    break;
                case BuildMode.FASTRIVERBRIDGE:
                    model.BuyBridge(x, y, true, true);
                    break;
                case BuildMode.SLOWRIVERBRIDGE:
                    model.BuyBridge(x, y, false, true);
                    break;
                case BuildMode.FASTLAKEBRIDGE:
                    model.BuyBridge(x, y, true, false);
                    break;
                case BuildMode.SLOWLAKEBRIDGE:
                    model.BuyBridge(x, y, false, false);
                    break;
                case BuildMode.NONE:
                    break;
            }
        }

        public void ViewModel_PutStop(int x, int y)
        {
            if (model.board[y,x].isStop== true)
            {
                Board[y * 100 + x].Content = "S";
            }
            
        }

        private int DecideColor_int(FieldType? ft, Industry? ind) 
        {
            int color_int=6;

            switch (ft)
            {
                case Forest forest:
                    if(forest.treeCount == 1)
                    {
                        color_int = 11;
                    }
                    else if(forest.treeCount == 2)
                    {
                        color_int = 12;
                    }
                    else if (forest.treeCount == 3)
                    {
                        color_int = 13;
                    }
                    else
                    {
                        color_int = 14;
                    }
                    break;

                case Lava:
                    color_int = 0;
                    break;

                case Road:
                    return 4;

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
