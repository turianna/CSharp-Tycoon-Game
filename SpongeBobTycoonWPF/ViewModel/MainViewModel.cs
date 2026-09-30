
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;
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
using System.Windows.Media.Animation;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public GameModel model;
        public int MovingVehicleListSize;
        public int ParkingVehicleListSize;
        public bool infoIsOpen;
       
        //private string VMvehicleType;
        
        private int time=0;

        private int money;

        private int roadPrice;
        private int truckPrice;
        private int vanPrice;
        private int woodBridgeLakePrice;
        private int woodBridgeRiverPrice;
        private int modernBridgeLakePrice;
        private int modernBridgeRiverPrice;
        private int terraformPrice;

        private string? algaeFactoryInfo;
        private string? petfoodFactoryInfo;
        private string? meatFactoryInfo;
        private string? burgerFactoryInfo;
        private string? spatulaFactoryInfo;
        private string? netFactoryInfo;
        private string? woodFactoryInfo;

        private bool route_ison;

        #region Properties
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

        public bool InfoIsOpen
        {
            get => infoIsOpen;
            set { infoIsOpen = value; OnPropertyChanged(); }
        }
        public int RoadPrice
        {
            get => roadPrice;
            set { roadPrice = value; OnPropertyChanged(); }
        }
        public int TruckPrice
        {
            get => truckPrice;
            set { truckPrice = value; OnPropertyChanged(); }
        }
        public int VanPrice
        {
            get => vanPrice;
            set { vanPrice = value; OnPropertyChanged(); }
        }
        public int WoodBridgeLakePrice
        {
            get => woodBridgeLakePrice;
            set { woodBridgeLakePrice = value; OnPropertyChanged(); }
        }
        public int WoodBridgeRiverPrice
        {
            get => woodBridgeRiverPrice;
            set { woodBridgeRiverPrice = value; OnPropertyChanged(); }
        }
        public int ModernBridgeLakePrice
        {
            get => modernBridgeLakePrice;
            set { modernBridgeLakePrice = value; OnPropertyChanged(); }
        }
        public int ModernBridgeRiverPrice
        {
            get => modernBridgeRiverPrice;
            set { modernBridgeRiverPrice = value; OnPropertyChanged(); }
        }
        public int TerraformPrice
        {
            get => terraformPrice;
            set { terraformPrice = value; OnPropertyChanged(); }
        }

        public string? AlgaeFactoryInfo
        {
            get => algaeFactoryInfo;
            set { algaeFactoryInfo = value!; OnPropertyChanged(); }
        }

        public string? PetfoodFactoryInfo
        {
            get => petfoodFactoryInfo;
            set { petfoodFactoryInfo = value!; OnPropertyChanged(); }
        }
        public string? MeatFactoryInfo
        {
            get => meatFactoryInfo;
            set { meatFactoryInfo = value!; OnPropertyChanged(); }
        }
        public string? BurgerFactoryInfo
        {
            get => burgerFactoryInfo;
            set { burgerFactoryInfo = value!; OnPropertyChanged(); }
        }
        public string? SpatulaFactoryInfo
        {
            get => spatulaFactoryInfo;
            set { spatulaFactoryInfo = value!; OnPropertyChanged(); }
        }
        public string? NetFactoryInfo
        {
            get => netFactoryInfo;
            set { netFactoryInfo = value!; OnPropertyChanged(); }
        }
        public string? WoodFactoryInfo
        {
            get => woodFactoryInfo;
            set { woodFactoryInfo = value!; OnPropertyChanged(); }
        }
        public string? NetFactoryText
        {
            get { return $"Net factory - Available net: {NetFactoryInfo}"; }
        }
        public string? AlgaeFactoryText
        {
            get { return $"Algae factory - Available algae: {NetFactoryInfo}"; }
        }
        public string? PetfoodFactoryText
        {
            get { return $"Petfood factory - Available petfood: {NetFactoryInfo}"; }
        }
        public string? BurgerFactoryText
        {
            get { return $"Burger factory - Available burger: {NetFactoryInfo}"; }
        }
        public string? SpatulaFactoryText
        {
            get { return $"Spatula factory - Available spatula: {NetFactoryInfo}"; }
        }

        public string? MeatFactoryText
        {
            get { return $"Meat factory - Available meat: {NetFactoryInfo}"; }
        }
        public string? WoodFactoryText
        {
            get { return $"Wood factory - Available wood: {NetFactoryInfo}"; }
        }
        #endregion

        #region Commands
        public DelegateCommand StartCommand { get; private set; }
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand QuitCommand { get; private set; } 
        public DelegateCommand StopTimeCommand { get; private set; }
        public DelegateCommand StartTimeCommand { get; private set; }
        public DelegateCommand SpeedUpTimeCommand { get; private set; }
        public DelegateCommand InfoCommand { get; private set; }
        public DelegateCommand CloseInfoCommand { get; private set; }

        public DelegateCommand TerraformUpCommand { get; private set; }
        public DelegateCommand TerraformDownCommand { get; private set; }

        public DelegateCommand CreateRouteCommand { get; private set; }
        //public DelegateCommand SelectVehicleCommand { get; private set; }

        public DelegateCommand BuyRoadCommand { get; private set; }
        public DelegateCommand PutStopCommand { get; private set; }

        public DelegateCommand BuyTruckCommand { get; private set; }
        public DelegateCommand BuyVanCommand { get; private set; }
        
        public DelegateCommand BuyRiverFastBridgeCommand { get; private set; }
        public DelegateCommand BuyRiverSlowBridgeCommand { get; private set; }
        public DelegateCommand BuyLakeFastBridgeCommand { get; private set; }
        public DelegateCommand BuyLakeSlowBridgeCommand { get; private set; }

        #endregion

        public BuildMode structure;

        public ObservableCollection<GameField> Board { get; private set; }
        public ObservableCollection<VehicleButton> MovingVehicleList { get; private set; }
        public ObservableCollection<VehicleButton> ParkingVehicleList { get; private set; }
        public ObservableCollection<SaveFile> SaveFiles { get; private set; }
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

            foreach (Window window in System.Windows.Application.Current.Windows.Cast<Window>().ToList())
            {
                if (window != gameWindow)
                {
                    window.Close();
                }
            }
        }

        #region Game loading
        private void LoadGame()
        {
            SaveFiles.Clear();
            GameLoadWindow loadWindow = new GameLoadWindow();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string saveFolder = System.IO.Path.Combine(appData, "SpongeBobTycoon", "saves");

            string fileName;
            int gameTime;
            int money;
            foreach(String saveFile in System.IO.Directory.GetFiles(saveFolder))
            {
                (fileName, gameTime, money) = DataAccess.GetSaveFileData(saveFile);
                SaveFiles.Add(new SaveFile
                {
                    FileName = fileName,
                    GameTime = gameTime,
                    Money = money,
                    LoadGame = new DelegateCommand(param => LoadGameFromFile(param, loadWindow))
                });
            }
            

            loadWindow.DataContext = this;

            loadWindow.ShowDialog();
        }

        private void LoadGameFromFile(object? param, GameLoadWindow loadWindow)
        {
            if (param is null) throw new Exception();

            string fileName = (string)param;

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string saveFolder = System.IO.Path.Combine(appData, "SpongeBobTycoon", "saves");
            string filePath = System.IO.Path.Combine(saveFolder, fileName);
            filePath += ".json";

            DataAccess dataAccess = new DataAccess();
            this.model = dataAccess.LoadGameFromJson(filePath);
            this.model.FieldChanged += Model_FieldChanged;
            this.model.TimerTick += Model_TimerTick;
            this.model.MoneyChanged += Model_MoneyChanged;
            this.model.VehicleBought += Model_VehicleBought;
            this.model.GameOver += Model_GameOver;
            this.model.Refresh += Model_Refresh;
            this.model.AddMovingVehicle += Model_AddMovingVehicle;
            this.model.CarStepped += Model_CarStepped;

            MovingVehicleList = new ObservableCollection<VehicleButton>();
            ParkingVehicleList = new ObservableCollection<VehicleButton>();
            MovingVehicleListSize = MovingVehicleList.Count;
            ParkingVehicleListSize = ParkingVehicleList.Count;
            structure = BuildMode.NONE;
            Board = new ObservableCollection<GameField>();
            SaveFiles = new ObservableCollection<SaveFile>();

            AddVehicles();

            Money = model.money;

            Board.Clear();
            Create();

            StartGame(loadWindow);

        }

        private void AddVehicles()
        {
            foreach(Vehicle v in model.parkingVehicles)
            {
                Model_VehicleBought(null, v);
            }

            foreach(Vehicle v in model.movingVehicles)
            {
                Model_AddMovingVehicle(null, v);
            }
        }
        #endregion

        public MainViewModel(GameModel model)
        {
            Vehicle fastvehicle = new FastCar(new Wood());
            Vehicle slowvehicle = new SlowCar(new Wood());
            SlowBridge slowbridge = new SlowBridge(true);
            FastBridge fastbridge = new FastBridge(true);

            this.model = model;
            Money = model.money;
            RoadPrice = model.RoadPrice;
            TruckPrice = fastvehicle.cost;
            VanPrice = slowvehicle.cost;
            WoodBridgeLakePrice = slowbridge.cost;
            WoodBridgeRiverPrice = slowbridge.cost;
            ModernBridgeLakePrice = fastbridge.cost;
            ModernBridgeRiverPrice = fastbridge.cost;
            TerraformPrice = model.TerraformPrice;

            
            AlgaeFactoryInfo = model.Board(30,27).industry!.productAmount.ToString();
            PetfoodFactoryInfo = model.Board(30, 93).industry!.productAmount.ToString();
            MeatFactoryInfo = model.Board(24, 66).industry!.productAmount.ToString();
            BurgerFactoryInfo = model.Board(1, 53).industry!.productAmount.ToString();
            SpatulaFactoryInfo = model.Board(4, 91).industry!.productAmount.ToString();
            NetFactoryInfo = model.Board(14, 3).industry!.productAmount.ToString();
            WoodFactoryInfo = model.Board(48, 70).industry!.productAmount.ToString();

            this.model.FieldChanged += Model_FieldChanged;
            this.model.TimerTick += Model_TimerTick;
            this.model.MoneyChanged += Model_MoneyChanged;
            this.model.VehicleBought += Model_VehicleBought;
            this.model.GameOver += Model_GameOver;
            this.model.Refresh += Model_Refresh;
            this.model.AddMovingVehicle += Model_AddMovingVehicle;
            this.model.CarStepped += Model_CarStepped;
            this.model.Produce += Model_Produce;

            LoadCommand = new DelegateCommand(_ => LoadGame());
            StartCommand = new DelegateCommand(param => StartGame(param!));
            StopTimeCommand = new DelegateCommand(_ => StopTime());
            StartTimeCommand = new DelegateCommand(_ => StartTime());
            SpeedUpTimeCommand = new DelegateCommand(_ => SpeedUpTime());
            CreateRouteCommand = new DelegateCommand(_ => CreateRoute());
            QuitCommand = new DelegateCommand(_ => QuitGame());
            NewGameCommand = new DelegateCommand(_ => NewGame());
            InfoCommand = new DelegateCommand(_ => OpenInfo());
            CloseInfoCommand = new DelegateCommand(_ => CloseInfo());
            BuyVanCommand = new DelegateCommand(param => BuyVan(param!));
            BuyTruckCommand = new DelegateCommand(param => BuyTruck(param!));
            BuyRoadCommand = new DelegateCommand(param => structure = BuildMode.ROAD);
            PutStopCommand = new DelegateCommand(param => structure = BuildMode.STOP);
            BuyRiverFastBridgeCommand = new DelegateCommand(param => structure = BuildMode.FASTRIVERBRIDGE);
            BuyRiverSlowBridgeCommand = new DelegateCommand(param => structure = BuildMode.SLOWRIVERBRIDGE);
            BuyLakeFastBridgeCommand = new DelegateCommand(param => structure = BuildMode.FASTLAKEBRIDGE);
            BuyLakeSlowBridgeCommand = new DelegateCommand(param => structure = BuildMode.SLOWLAKEBRIDGE);
            TerraformUpCommand = new DelegateCommand(param => structure = BuildMode.TERRAFORM_UP);
            TerraformDownCommand = new DelegateCommand(param => structure = BuildMode.TERRAFORM_DOWN);
            //SaveCommand = new DelegateCommand(param => SaveGame());

            //ExitBuildCommand = new DelegateCommand(param => structure = BuildMode.NONE);

            MovingVehicleList = new ObservableCollection<VehicleButton>();
            ParkingVehicleList = new ObservableCollection<VehicleButton>();
            MovingVehicleListSize = MovingVehicleList.Count;
            ParkingVehicleListSize = ParkingVehicleList.Count;
            structure = BuildMode.NONE;
            Board = new ObservableCollection<GameField>();
            SaveFiles = new ObservableCollection<SaveFile>();
            Create();
        }


        /// <summary>
        /// Create
        /// Inicializálja a grafikus játéktáblát egy 50x100-as rács létrehozásával. A metódus végigiterál a modell összes mezőjén, és minden cellához kiszámítja a megjelenítendő színt, az út típusát és az esetleges megállókat jelző grafikákat. Minden egyes mezőhöz létrehoz egy GameField objektumot, amely tartalmazza a koordinátákat, a vizuális állapotot, valamint egy kattintáskezelő parancsot (ClickCommand), amely a HandleClick metódust hívja meg. Erdő típusú mezők esetén a cella tartalmát (Content) a fákat jelző számmal is kitölti, majd a kész mezőket hozzáadja a nézetmodell által kezelt Board gyűjteményhez.
        /// </summary>
        private void Create()
        {
            for (int i = 0; i < 50; i++) 
            {
                for (int j = 0; j < 100; j++)
                {
                    int color_int = DecideColor_int(model.Board(i, j).fieldType, model.Board(i, j).industry!, model.Board(i,j).Z, model.Board(i,j).id);
                    int roadPicture = DecideRoadPicture(model.Board(i,j).fieldType);
                    int vehiclePicture = 0;
                    int stopPicture = 0;
                    if (model.Board(i, j).isStop) { //perzisztenciához
                        stopPicture = 1;
                    }
                        Board.Add(new GameField
                        {
                            x = i,
                            y = j,
                            Color = color_int,
                            RoadPicture = roadPicture,
                            VehiclePicture = vehiclePicture,
                            StopPicture = stopPicture,
                            ClickCommand = new DelegateCommand(param =>
                            {
                                if (param is Tuple<int, int> pos)
                                {
                                    HandleClick(pos.Item1, pos.Item2);
                                }
                            }),
                            Content = "",
                            IsHighlighted = false
                        });
                    if (color_int == 1) Board[i * 100 + j].Content = ((Forest)model.Board(i, j).fieldType!).treeCount.ToString();
                }

            }
        }
        #region Command handlers
        public void SaveGame(string path)
        {
            model.SaveGame(path);
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

        private void NewGame()
        {
            model.StartNewGame();
        }

        private void OpenInfo()
        {
            InfoIsOpen = true;
        }

        private void CloseInfo()
        {
            InfoIsOpen = false;
        }

        private void BuyVan(object param)
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

        private void BuyTruck(object param)
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
        #endregion

        #region Vehicle route management
        private void VehicleForRoute(object v)
        {
            Vehicle vehicle = (Vehicle)v;
            RouteManager rm = new RouteManager(vehicle);
            model.ChosenVehicle(vehicle);
            structure = BuildMode.ROUTE;
            Route_IsOn = true;
 
        }

        /// <summary>
        /// VehicleFromRoute
        /// Eltávolítja a megadott járművet az aktív forgalomból, és visszahelyezi azt a parkoló járművek listájába. A metódus megkeresi a járművet az egyedi azonosítója (ID) alapján a MovingVehicleList gyűjteményben, törli onnan, majd a modell szintjén is megszünteti az útvonalhoz rendelését. Ezt követően a jármű konkrét típusa alapján (FastCar esetén "Van", egyébként "Truck") meghatározza a megjelenítendő megnevezést, és létrehoz egy új VehicleButton objektumot a ParkingVehicleList számára, amely tartalmazza a jármű adatait és egy újraaktiválást lehetővé tevő parancsot.
        /// </summary>
        private void VehicleFromRoute(object v)
        {
            Vehicle vehicle = (Vehicle)v;
            int j = -1;
            for (int i = 0; i < MovingVehicleList.Count; i++)
            {
                if (MovingVehicleList[i].Vehicle.ID == vehicle.ID)
                {
                    j = i;
                }
            }
            if (j != -1)
            {
                MovingVehicleList.RemoveAt(j);

            }
            model.RemoveVehicleFromRoute(vehicle);

            string VMvehicleType = v is FastCar ? "Van" : "Truck";

            ParkingVehicleList.Add(new VehicleButton
            {
                Vehicle = vehicle,
                Product = vehicle.productName,
                VehicleType = VMvehicleType,
                ClickCommand = new DelegateCommand(param =>
                {
                    if (param is Vehicle v)
                    {
                        VehicleForRoute(v); 
                    }
                })
            });

        }
        public void CreateRoute()
        {
            model.CreateRoute();
            foreach(GameField gf in Board)
            {
                gf.IsHighlighted = false;
            }
            structure = BuildMode.NONE;
            Route_IsOn = false;
        }
        #endregion
        
        #region Model event handlers

        public void Model_CarStepped(object? sender, (int x, int y, int v) coord)
        {
            // Your board is a 1D array representing a 2D grid (x * 100 + y)
            int index = coord.x * 100 + coord.y;

            // Check the model to see if a vehicle is currently on this specific tile
            if (model.board[coord.x, coord.y].isVehicle)
            {
                // Draw the vehicle
                Board[index].VehiclePicture = DecideVehiclePicture(coord.v);
            }
            else
            {
                // Clear the vehicle from the tile (0 = empty)
                Board[index].VehiclePicture = 0;
            }
        }
        /// <summary>
        /// Model_AddMovingVehicle
        /// Eseménykezelő metódus, amely egy jármű forgalomba állításakor fut le. A metódus megkeresi az adott járművet az egyedi azonosítója (ID) alapján a várakozó járművek listájában (ParkingVehicleList), és ha megtalálja, eltávolítja onnan. Meghatározza a jármű típusát (FastCar esetén "Van", egyébként "Truck"), majd létrehoz és hozzáad egy új VehicleButton objektumot a MovingVehicleList gyűjteményhez. Az újonnan létrehozott gombhoz egy parancsot is rendel, amely lehetővé teszi, hogy a felhasználó a későbbiekben – a VehicleFromRoute metóduson keresztül – visszahívja a járművet az útvonaláról.
        /// </summary>
        private void Model_AddMovingVehicle(object? sender, Vehicle e)
        {
            string VMvehicleType;
            int j = -1;
            for (int i = 0; i < ParkingVehicleList.Count; i++)
            {
                if (ParkingVehicleList[i].Vehicle.ID == e.ID)
                {
                    j = i;
                }
            }
            if (j != -1)
            {
                ParkingVehicleList.RemoveAt(j);

            }

            if (e is FastCar)
            {
                VMvehicleType = "Van";
            }
            else
            {
                VMvehicleType = "Truck";
            }
           
            MovingVehicleList.Add(new VehicleButton
            {
                Vehicle = e,
                Product = e.productName, 
                VehicleType = VMvehicleType, 
                ClickCommand = new DelegateCommand(param =>
                {
                    if (param is Vehicle v)
                    {
                        VehicleFromRoute(v);
                    }
                })
            });
        }
        /// <summary>
        /// Model_VehicleBought
        /// Eseménykezelő metódus, amely egy új jármű megvásárlásakor fut le. A metódus a jármű konkrét típusa alapján meghatározza annak kategóriáját (FastCar esetén "Van", minden más esetben "Truck"), majd létrehoz egy új VehicleButton objektumot. Ezt az objektumot hozzáadja a várakozó járművek listájához (ParkingVehicleList), rögzítve a jármű adatait és a szállított termék megnevezését. A gombhoz egy parancsot is rendel, amely lehetővé teszi a felhasználó számára, hogy a jármű kiválasztásával megkezdje annak útvonaltervezését a VehicleForRoute metódus segítségével.
        /// </summary>
        private void Model_VehicleBought(object? sender, Vehicle e)
        {
            string VMvehicleType;
            if (e is FastCar)
            {
                VMvehicleType = "Van";
            }
            else
            {
                VMvehicleType= "Truck";
            }
           
            ParkingVehicleList.Add(new VehicleButton
            {
                Vehicle = e,
                Product = e.productName,
                VehicleType = VMvehicleType,
                ClickCommand = new DelegateCommand(param =>
                {
                    if (param is Vehicle v)
                    {
                        VehicleForRoute(v); 
                    }
                })
            });
        }

        private void Model_Produce(object? sender, Industry industry)
        {
            switch (industry.product)
            {
                case Net:
                    NetFactoryInfo = industry.productAmount.ToString();
                    OnPropertyChanged(nameof(NetFactoryText));
                    break;
                case Petfood:
                    PetfoodFactoryInfo = industry.productAmount.ToString();
                    OnPropertyChanged(nameof(PetfoodFactoryText));
                    break;
                case Algae:
                    AlgaeFactoryInfo = industry.productAmount.ToString();
                    OnPropertyChanged(nameof(AlgaeFactoryText));
                    break;
                case Burger:
                    BurgerFactoryInfo = industry.productAmount.ToString();
                    OnPropertyChanged(nameof(BurgerFactoryText));
                    break;
                case Meat:
                    MeatFactoryInfo = industry.productAmount.ToString();
                    OnPropertyChanged(nameof(MeatFactoryText));
                    break;
                case Spatula:
                    SpatulaFactoryInfo = industry.productAmount.ToString();
                    OnPropertyChanged(nameof(SpatulaFactoryText));
                    break;
                case Wood:
                    WoodFactoryInfo = industry.productAmount.ToString();
                    OnPropertyChanged(nameof(WoodFactoryText));
                    break;

                default:
                    break;
            }
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
        #endregion

        #region ViewModel update methods
        private void UpdateField(FieldChangedEventArgs e)
        {

            int color_int = DecideColor_int(model.Board(e.x, e.y).fieldType, model.Board(e.x, e.y).industry!, model.Board(e.x, e.y).Z, model.Board(e.x, e.y).id);
            Board[e.x * 100 + e.y].Color = color_int;
            int roadPicture = DecideRoadPicture(model.Board(e.x, e.y).fieldType);
            Board[e.x * 100 + e.y].RoadPicture = roadPicture;
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

                    if (model.board[x,y].isStop == false)
                    {
                        MessageBox.Show("You can only set a route on a stop!");
                        break;
                    }

                    if (Board[x * 100 + y].IsHighlighted)
                    {
                        Board[x * 100 + y].IsHighlighted = false;
                        model.DeSelectStop(x, y);
                    }
                    else
                    {
                        Board[x * 100 + y].IsHighlighted = true;
                        model.SelectStop(x, y);
                    }
                    break;
                case BuildMode.NONE:
                    break;
            }
        }  

        public void ViewModel_PutStop(int x, int y)
        {
            if (model.board[x,y].isStop== true)
            {
                Board[x * 100 + y].StopPicture = 1;
            }
            else
            {
                Board[x * 100 + y].StopPicture = 0;
            }
            
        }
        #endregion

        #region comment
        /*
        public void Model_CarStepped(object sender, (int, int,Vehicle)coord)
        {
            if (coord.Item3.dir == 1)
            {
                vehiclePicture = 1;
            }else if( coord.Item3.dir == 2)
            {
                vehiclePicture = 2;
            }else if(coord.Item3.dir == 3)
            {
                vehiclePicture = 3;
            }else if(coord.Item3.dir == 4)
            {
                vehiclePicture = 4;
            }
            else if (coord.Item3.dir == 5)
            {
                vehiclePicture = 5;
            }
            else if (coord.Item3.dir == 6)
            {
                vehiclePicture = 6;
            }


        }

        private int DecideVehiclePicture(Vehicle v)
        {
             //0-empty, 1 -up, 2-down, 3-right, 4-left, 5-2NS, 6-2WE
            switch (v.dir)
            {
                case 0:
                    vehiclePicture = 0; 
                    break;
                case 1: 
                    vehiclePicture = 1; break;
                case 2: 
                    vehiclePicture = 2; break;
                    case 3: vehiclePicture = 3; break;
                    case 4: vehiclePicture = 4; break;
                 
            }
            return vehiclePicture;
        }*/
        #endregion

        

        #region Decision methods for pictures
        private int DecideVehiclePicture(int v)
        {
            // Return the correct int directly based on direction
            switch (v)
            {
                case 1: return 1;
                case 2: return 2;
                case 3: return 3;
                case 4: return 4;
                case 5: return 5;
                case 6: return 6;
                default: return 0;
            }
        }

        private int DecideRoadPicture(FieldType? ft)
        {
            

            int curroad = 0;
            switch (ft)
            {
                case Road road:
                    switch (road.roadType)
                    {
                        case RoadType.STRAIGHTNS:
                            curroad = 401;
                            break;
                        case RoadType.STRAIGHTWE:
                            curroad = 402;
                            break;
                    } break;
                case SlowBridge sb:
                    switch (sb.roadType)
                    {
                        case RoadType.STRAIGHTNS:
                            curroad = 401;
                            break;
                        case RoadType.STRAIGHTWE:
                            curroad = 402;
                            break;
                    } break;

                case FastBridge sb:
                    switch (sb.roadType)
                    {
                        case RoadType.STRAIGHTNS:
                            curroad = 401;
                            break;
                        case RoadType.STRAIGHTWE:
                            curroad = 402;
                            break;
                    }
                    break;
                    /*
                    case RoadType.CROSSROAD:
                        roadPicture = 444;
                        break;
                    case RoadType.TURNNW:
                        roadPicture = 401;
                        break;
                    case RoadType.TURNNE:
                        roadPicture = 401;
                        break;
                    case RoadType.TURNSW:
                        roadPicture = 402;
                        break;
                    case RoadType.TURNSE:
                        roadPicture = 402;
                        break;
                    case RoadType.TN:
                        roadPicture = 401;
                        break;
                    case RoadType.TE:
                        roadPicture = 402;
                        break;
                    case RoadType.TW:
                        roadPicture = 402;
                        break;
                    case RoadType.TS:
                        roadPicture = 401;
                        break;*/
            
             
            }
            return curroad;
        }
        private int DecideColor_int(FieldType? ft, Industry ind, int z, int id) 
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
                                    return color_int = 110;
                                case 1:
                                    return color_int = 111;
                                case 2:
                                    return color_int = 112;
                                case 3:
                                    return color_int = 113;
                                case 4:
                                    return color_int = 114;
                            }
                            break;
                        case 2:
                            switch (z)
                            {
                                case 0:
                                    return color_int = 120;
                                case 1:
                                    return color_int = 121;
                                case 2:
                                    return color_int = 122;
                                case 3:
                                    return color_int = 123;
                                case 4:
                                    return color_int = 124;
                            }
                            break;
                        case 3:
                            switch (z)
                            {
                                case 0:
                                    return color_int = 130;
                                case 1:
                                    return color_int = 131;
                                case 2:
                                    return color_int = 132;
                                case 3:
                                    return color_int = 133;
                                case 4:
                                    return color_int = 134;
                            }
                            break;
                        case 4:
                            switch (z)
                            {
                                case 0:
                                    return color_int = 140;
                                case 1:
                                    return color_int = 141;
                                case 2:
                                    return color_int = 142;
                                case 3:
                                    return color_int = 143;
                                case 4:
                                    return color_int = 144;
                            }
                            break;
                    }
                    break;

                case Lava:
                    color_int = 0;
                    break;

                case SlowBridge:
                    color_int = 51;
                    break;

                case FastBridge:
                    color_int = 52;
                    break;

                case Road:
                    return 402;

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

                
            switch(ind.product){
                    case Burger:
                            switch (id) // burger 0. industry,burger,id
                            {

                                case 1:
                                   color_int = 2001;
                                   break;
                                default:
                                    color_int = 2002;
                                    break;
                            }
                            break;

                        case Wood:
                            switch (id)
                            {
                                case 1:
                                    color_int = 211;
                                    break;
                                default:
                                    color_int = 212;
                                    break;
                            }

                         break;

                        case Algae: // ALgee: 2
                            switch (id)
                            {
                                case 1:
                                    color_int = 2201;
                                    break;
                                default:
                                    color_int = 2202;
                                    break;  
                            }
                            break;

                        case Net:
                            switch (id)
                            {
                                case 1:
                                    color_int = 231;
                                    break;
                                default:
                                    color_int = 232;
                                    break;
                            }

                            break;

                        case Spatula:
                            switch (id)
                            {
                                case 1:
                                    color_int = 241;
                                    break;
                                default:
                                    color_int = 242;
                                    break;
                            }

                             break;

                        case Meat:
                            switch (id)
                            {
                                case 1:
                                    color_int = 261;
                                    break;
                                default:
                                    color_int = 262;
                                    break;
                            }

                        break;
                        case Petfood:
                            switch (id)
                            {
                                case 1:
                                    color_int = 251;
                                    break;
                                default:
                                    color_int = 252;
                                    break;
                            }

                            break;

                case Citizen:
                            if (ft is Road) break;
                            switch (z)
                            {
                                case 1:
                                    color_int = 31;
                                    break;
                                case 2:
                                    color_int = 32;
                                    break;
                                case 3:
                                    color_int = 33;
                                    break;
                            }
                            break;
                }
                                   
            return color_int;
        }
        #endregion
    }
}
