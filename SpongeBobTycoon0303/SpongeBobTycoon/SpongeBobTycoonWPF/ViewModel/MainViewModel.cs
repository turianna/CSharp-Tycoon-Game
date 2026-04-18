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
        private string VMvehicleType;
        private string VMProduct;
        
       

        private int time=0;

        private int money;
        private bool route_ison;
        public int Money
        {
            get => money;
            set { money = value; OnPropertyChanged(); }
        }
        public bool Route_IsOn
        {
            get => route_ison;
            set { route_ison = value; OnPropertyChanged(); }
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
        public DelegateCommand SelectVehicleCommand { get; private set; }

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
        public ObservableCollection<VehicleButton> MovingVehicleList { get; private set; }
        public ObservableCollection<VehicleButton> ParkingVehicleList { get; private set; }
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
            this.model.AddMovingVehicle += Model_AddMovingVehicle;
            this.model.CarStepped += Model_CarStepped;

            StartCommand = new DelegateCommand(param => StartGame(param));
            StopTimeCommand = new DelegateCommand(_ => StopTime());
            StartTimeCommand = new DelegateCommand(_ => StartTime());
            SpeedUpTimeCommand = new DelegateCommand(_ => SpeedUpTime());
            CreateRouteCommand = new DelegateCommand(_ => CreateRoute());
            QuitCommand = new DelegateCommand(_ => QuitGame());
            BuyVanCommand = new DelegateCommand(param => BuyVan(param));
            BuyTruckCommand = new DelegateCommand(param => BuyTruck(param));
            BuyRoadCommand = new DelegateCommand(param => structure = BuildMode.ROAD);
            PutStopCommand = new DelegateCommand(param => structure = BuildMode.STOP);
            BuyRiverFastBridgeCommand = new DelegateCommand(param => structure = BuildMode.FASTRIVERBRIDGE);
            BuyRiverSlowBridgeCommand = new DelegateCommand(param => structure = BuildMode.SLOWRIVERBRIDGE);
            BuyLakeFastBridgeCommand = new DelegateCommand(param => structure = BuildMode.FASTLAKEBRIDGE);
            BuyLakeSlowBridgeCommand = new DelegateCommand(param => structure = BuildMode.SLOWLAKEBRIDGE);
            TerraformUpCommand = new DelegateCommand(param => structure = BuildMode.TERRAFORM_UP);
            TerraformDownCommand = new DelegateCommand(param => structure = BuildMode.TERRAFORM_DOWN);

            //SelectVehicleCommand = new DelegateCommand(param => VehicleForRoute(param));
            //ExitBuildCommand = new DelegateCommand(param => structure = BuildMode.NONE);

            MovingVehicleList = new ObservableCollection<VehicleButton>();
            ParkingVehicleList = new ObservableCollection<VehicleButton>();
            MovingVehicleListSize = MovingVehicleList.Count;
            ParkingVehicleListSize = ParkingVehicleList.Count;
            structure = BuildMode.NONE;

            /*
            foreach (Vehicle v in model.movingVehicles)
            {
                MovingVehicleList.Add(new VehicleButton
                {
                    vehicle = v,
                    ClickCommand = new DelegateCommand(param =>
                    {
                        if (param is Vehicle v)
                        {
                            VehicleClick(v);
                        }
                    })
                });
            }

            foreach (Vehicle v in model.parkingVehicles)
            {
                ParkingVehicleList.Add(new VehicleButton
                {
                    vehicle = v,
                    ClickCommand = new DelegateCommand(param =>
                    {
                       if (param is Vehicle v)
                       {
                           VehicleClick(v);
                       }
                    })
                });
            }
            */
            Board = new ObservableCollection<GameField>();
            Create();
        }
           
        private void Create()
        {
            for (int i = 0; i < 50; i++) //i = y
            {
                for (int j = 0; j < 100; j++) //j = x
                {
                    int color_int = DecideColor_int(model.Board(i, j).fieldType, model.Board(i, j).industry, model.Board(i,j).Z);

                    Board.Add(new GameField
                    {
                        x = i,
                        y = j,
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

        private void VehicleForRoute(object v)
        {
            Vehicle vehicle = (Vehicle)v;
            RouteManager rm = new RouteManager(vehicle);
            int j = -1;
            for(int i = 0; i < ParkingVehicleList.Count; i++)
            {
                if (ParkingVehicleList[i].Vehicle.ID == vehicle.ID)
                {
                    j = i;
                }
            }
            if (j != -1)
            {
                ParkingVehicleList.RemoveAt(j);

            }
            model.ChosenVehicle(vehicle);
            structure = BuildMode.ROUTE;
            Route_IsOn = true;//jelezni kell a viewnak hogy jelenítsen meg egy gombot
            /*
            MovingVehicleList.Add(new VehicleButton
            {
                Vehicle = vehicle,
                Product = VMProduct, //át kell írni a vehicle.product.tostring()re
                VehicleType = VMvehicleType, //ezt is át kell írni
                ClickCommand = new DelegateCommand(param =>
                {
                    if (param is Vehicle v)
                    {
                        VehicleForRoute(v); //ClickCommand, Ezt is át kell írni valamire ami illik arra mit kell csinálnia ha rákattintunk egy moving vehiclere
                    }
                })
            }); ; //nem ide kell
            */
        }

        public void CreateRoute()
        {
            model.ModelCreateRoute();
            Route_IsOn = false;
        }

        private void Model_AddMovingVehicle(object? sender, Vehicle e)
        {
            if (e is FastCar)
            {
                VMvehicleType = "Truck";
            }
            else
            {
                VMvehicleType = "Van";
            }
            switch (e.product)
            {
                case Algae:
                    VMProduct = "Algee";
                    break;
                case Burger:
                    VMProduct = "Burger";
                    break;
                case Citizen:
                    VMProduct = "Citizen";
                    break;
                case Meat:
                    VMProduct = "Meat";
                    break;
                case Net:
                    VMProduct = "Net";
                    break;
                case Petfood:
                    VMProduct = "Petfood";
                    break;
                case Spatula:
                    VMProduct = "Spatula";
                    break;
                case Wood:
                    VMProduct = "Wood";
                    break;
                    //Kéredes: Lehet a vehiclenek egy string productja? Mert ez nem jó itt.
            }
            MovingVehicleList.Add(new VehicleButton
            {
                Vehicle = e,
                Product = VMProduct, //át kell írni a vehicle.product.tostring()re
                VehicleType = VMvehicleType, //ezt is át kell írni
                ClickCommand = new DelegateCommand(param =>
                {
                    if (param is Vehicle v)
                    {
                        VehicleForRoute(v); //ClickCommand, Ezt is át kell írni valamire ami illik arra mit kell csinálnia ha rákattintunk egy moving vehiclere
                    }
                })
            });
        }
        private void Model_VehicleBought(object? sender, Vehicle e)
        {
            if (e is FastCar)
            {
                VMvehicleType = "Truck";
            }
            else
            {
                VMvehicleType= "Van";
            }
            switch(e.product)
            {
                case Algae:
                    VMProduct = "Algee";
                    break;
                case Burger:
                    VMProduct = "Burger";
                    break;
                case Citizen:
                    VMProduct = "Citizen";
                    break;
                case Meat:
                    VMProduct = "Meat";
                    break;
                case Net:
                    VMProduct = "Net";
                    break;
                case Petfood:
                    VMProduct = "Petfood";
                    break;
                case Spatula:
                    VMProduct = "Spatula";
                    break;
                case Wood:
                    VMProduct = "Wood";
                    break;
                //Kéredes: Lehet a vehiclenek egy string productja? Mert ez nem jó itt.
            }
            ParkingVehicleList.Add(new VehicleButton
            {
                Vehicle = e,
                Product = VMProduct,
                VehicleType = VMvehicleType,
                ClickCommand = new DelegateCommand(param =>
                {
                    if (param is Vehicle v)
                    {
                        VehicleForRoute(v); //ClickCommand, ez csak akkor fog lefutni ha rákattintunk a gombra
                    }
                })
            });
            //ParkingVehicleList.Add(e);
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
        private void UpdateField(FieldChangedEventArgs e)
        {
            int color_int = DecideColor_int(model.Board(e.x, e.y).fieldType, model.Board(e.x, e.y).industry, model.Board(e.x, e.y).Z);

            Board[e.x * 100 + e.y].Color = color_int;
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


        public void ViewModelForest(FieldChangedEventArgs e)
        {
            if (model.Board(e.x, e.y).fieldType is Forest)
            {
                Forest forest = (Forest)model.Board(e.x, e.y).fieldType!;
                Board[e.x * 100 + e.y].Content = forest.treeCount.ToString();
            }
            else
            {
                Board[e.x * 100 + e.y].Content = "";
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
                case BuildMode.TERRAFORM_UP:
                    model.Terraform(x, y, true);
                    break;
                case BuildMode.TERRAFORM_DOWN:
                    model.Terraform(x, y, false);
                    break;
                case BuildMode.ROUTE:
                    model.SelectStop(x,y);
                    break;
                case BuildMode.NONE:
                    break;
            }
        }

        /* this was only used to test terraforming, but it can be useful later to check the z coordinates, so I left it in
        private void showZCoordinates()
        {
            for (int i = 0; i < 50; i++) //i = y
                {
                    for (int j = 0; j < 100; j++) //j = x
                    {
                        Board[i * 100 + j].Content = model.Board(i, j).Z.ToString();
                    }
            }
        }
        */

        public void ViewModel_PutStop(int x, int y)
        {
            if (model.board[x,y].isStop== true)
            {
                Board[x * 100 + y].Content = "S";
            }
            
        }

        public void Model_CarStepped(object sender, (int, int)coord)
        {
            if (model.board[coord.Item1, coord.Item2].isVehicle == true)
            {
                Board[coord.Item1 * 100 + coord.Item2].Content = "C";
            }
            else
            {
                Board[coord.Item1 * 100 + coord.Item2].Content = "";
            }

        }


        private int DecideColor_int(FieldType? ft, Industry? ind, int z) 
        {
            int color_int=6;

            switch (ft)
            {
                case Forest forest:
                    switch (forest.treeCount)
                    {
                        case 1:
                            switch (z)
                            {
                                case 0:
                                    color_int = 110;
                                    break;
                                case 1:
                                    color_int = 111;
                                    break;
                                case 2:
                                    color_int = 112;
                                    break;
                                case 3:
                                    color_int = 113;
                                    break;
                                case 4:
                                    color_int = 114;
                                    break;
                            }
                            break;
                        case 2:
                            switch (z)
                            {
                                case 0:
                                    color_int = 120;
                                    break;
                                case 1:
                                    color_int = 121;
                                    break;
                                case 2:
                                    color_int = 122;
                                    break;
                                case 3:
                                    color_int = 123;
                                    break;
                                case 4:
                                    color_int = 124;
                                    break;
                            }
                            break;
                        case 3:
                            switch (z)
                            {
                                case 0:
                                    color_int = 130;
                                    break;
                                case 1:
                                    color_int = 131;
                                    break;
                                case 2:
                                    color_int = 132;
                                    break;
                                case 3:
                                    color_int = 133;
                                    break;
                                case 4:
                                    color_int = 134;
                                    break;
                            }
                            break;
                        case 4:
                            switch (z)
                            {
                                case 0:
                                    color_int = 140;
                                    break;
                                case 1:
                                    color_int = 141;
                                    break;
                                case 2:
                                    color_int = 142;
                                    break;
                                case 3:
                                    color_int = 143;
                                    break;
                                case 4:
                                    color_int = 144;
                                    break;
                            }
                            break;
                    }
                    break;

                case Lava:
                    color_int = 0;
                    break;

                case Road:
                    return 4;

                case Grass:
                    switch (z)
                    {
                        case 0:
                            color_int = 60;
                            break;
                        case 1:
                            color_int = 61;
                            break;
                        case 2:
                            color_int= 62;
                            break;
                        case 3:
                            color_int = 63;
                            break;
                        case 4:
                            color_int = 64;
                            break;
                    }
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
