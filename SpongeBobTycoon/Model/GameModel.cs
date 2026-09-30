using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Formats.Tar;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class GameModel
    {
        #region Constants
        private const int GAMETIME_BASE = 4;
        private const int GAMETIME_FAST = 2;
        private const int GAMETIME_FASTEST = 1;
        private const int startingMoney = 5000;
        #endregion

        #region Properties
        public List<Stop> stops { get; set; }

        public List<Vehicle> parkingVehicles { get; set; } 

        public List<Vehicle> movingVehicles { get; set; }
        

        public Vehicle? VehicleForRoute { get; set; }
        public List<Stop> StopsForRoute { get; set; }
        public int money { get; private set; }
        public int RoadPrice = 100;
        public int TerraformPrice = 250;
        #endregion

        #region Instances of other classes
        private TerraformingManager TerraformManager = new TerraformingManager();
        private DataAccess DataAccess;
        #endregion

        #region Events
        public event EventHandler? GameOver;
        public event EventHandler<int>? MoneyChanged;
        public event EventHandler<FieldChangedEventArgs>? FieldChanged;
        public event EventHandler<Vehicle>? VehicleBought; //updatelni kell a parkoló autók listáját
        public event EventHandler? Refresh;
        public event EventHandler<(int,int,int)>? CarStepped;
        public event EventHandler<Vehicle>? AddMovingVehicle;
        public event EventHandler? CreateRouteFailed;
        public event EventHandler<Industry>? Produce;

        public event EventHandler<int>? TimerTick;
        int[] news;
        public int[] getNews => news;
        #endregion


        public ITimer timer;
        public int gameTime;
        
        public TimeStatus timeStatus { get;  private set; }
        public int gameTimer;
        private int biggestVehicleID;


        public Field[,] board { get; private set; }
        private List<Field> forests;
        private List<Industry> industries;

        public List<Field> GetForests() => forests;
        public List<Industry> GetIndustries() => industries;

        public GameModel(Field[,] board, int gameTime, int gameTimer, int money, List<Field> forests, List<Stop> stops, List<Industry> industries, List<Vehicle> parkingVehicles, List<Vehicle> movingVehicles, int[] news)
        {
            this.board = board;
            this.gameTime = gameTime;
            this.gameTimer = gameTimer;
            this.money = money;
            this.forests = forests;
            this.stops = stops;
            this.industries = industries;
            this.parkingVehicles = parkingVehicles;
            this.movingVehicles = movingVehicles;
            this.news = news;

            StopsForRoute = new List<Stop>();
            DataAccess = new DataAccess();
            timer = new GameTimer();
            timer.Interval = 250;
            timer.Elapsed += new EventHandler(Timer_Elapsed);
            timer.Start();
        }

        public GameModel(DataAccess dataAccess)
        {
            stops = new List<Stop>();
            board = new Field[50, 100];
            parkingVehicles = new List<Vehicle>();
            movingVehicles = new List<Vehicle>();
            StopsForRoute = new List<Stop>();

            DataAccess = dataAccess;
            board = DataAccess.LoadMap();
            forests = DataAccess.GetForests();
            industries = DataAccess.GetIndustries();

            timer = new GameTimer();
            timer.Interval = 250;
            timer.Elapsed += new EventHandler(Timer_Elapsed);

            gameTime = GAMETIME_BASE;
            gameTimer = 0;
            timeStatus = TimeStatus.BASE;
            money = startingMoney;
            biggestVehicleID = 0;
            news = new int[4];


            timer.Start();
        }
        
        public Field Board(int i, int j)
        {
            return board[i, j];
        }


        #region Initialization
        /// <summary>
        /// StartNewGame
        /// Elindít egy új játékot: leállítja az időzítőt, biztonságosan törli a meglévő mozgó és parkoló járműveket valamint a megállókat. Újratölti a pályát (50x100) és az erdőket a DataAccess segítségével, frissíti a nézetet, visszaállítja a játékidőt alaphelyzetbe, 10 000-re állítja az egyenleget, majd újraindítja a fő időzítőt.
        /// </summary>

        public void StartNewGame()
        {
            timer.Stop();
            List<Vehicle> removevehicle = new List<Vehicle>(movingVehicles);
            foreach (Vehicle vehicle in removevehicle)
            {
                RemoveVehicleFromRoute(vehicle);
            }

            stops.Clear();
            parkingVehicles.Clear(); 
            movingVehicles.Clear();
            board = DataAccess.LoadMap();
            forests = DataAccess.GetForests();
            OnRefresh();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    OnFieldChanged(i, j, board[i, j].fieldType!);
                }
            }
            
            gameTime = GAMETIME_BASE;

            gameTimer = 0;
            timeStatus = TimeStatus.BASE;
            money = startingMoney;
            biggestVehicleID = 0;
            OnMoneyChanged(money);

            timer.Start();
        }
        /// <summary>
        /// InitializeBoard
        /// Egy 50x100-as ciklussal végighalad a pályán, és minden cellát alaphelyzetbe állít egy teljesen üres, kezdőértékekkel (Direction.NONE, üres Industry) rendelkező Field objektummal, előkészítve a táblát a játékra.
        /// </summary>

        public void InitializeBoard()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    board[i, j] = new Field(i, j, 1, new Industry(), new FieldType(), false, false, false, 1,Direction.NONE);
                }
            }
        }
        #endregion

        #region Buying and Building
        /// <summary>
        /// BuyRoad
        /// Új utat épít a megadott x (sor) és y (oszlop) koordinátákon, ha az adott mező fű vagy erdő, nincs rajta lakos (Citizen) és csatlakozik meglévő úthoz. Fű esetén az alapértelmezett útépítési költséget (RoadPrice) vonja le, erdő esetén annak a háromszorosát (ilyenkor a fát is törli a listából). Sikeres építés után beállítja az út irányát a szomszédok alapján, levonja a pénzt és frissíti a képernyőt.
        /// </summary>
        public void BuyRoad(int x, int y) {

            if (board[x,y].fieldType is Grass && board[x,y].industry!.product is not Citizen && AdjacentRoad(x, y))
            {           
                board[x, y].fieldType = new Road();
                if ((news[0]==1 || news[3] == 1) && news[1]==0 && news[2] == 0 ) ((Road)board[x, y].fieldType!).roadType = RoadType.STRAIGHTNS;
                else if(news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((Road)board[x, y].fieldType!).roadType = RoadType.STRAIGHTWE;
                ChangeBalance(-RoadPrice);
                OnFieldChanged(x, y, board[x, y].fieldType!);
            }else if (board[x, y].fieldType is Forest && board[x, y].industry!.product is not Citizen && AdjacentRoad(x, y)) 
            {
                Field? forestToRemove = null;
                foreach (Field f in forests)
                {
                    if(f.X == x && f.Y == y)
                    {
                        forestToRemove = f;
                    }
                }
                if(forestToRemove != null) forests.Remove(forestToRemove);
                board[x, y].fieldType = new Road();
                if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((Road)board[x, y].fieldType!).roadType = RoadType.STRAIGHTNS;
                else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((Road)board[x, y].fieldType!).roadType = RoadType.STRAIGHTWE;
                ChangeBalance(-RoadPrice*3);
                OnFieldChanged(x, y, board[x, y].fieldType!);
            }

        }
        /// <summary>
        /// BuyVehicle
        /// Vásárol egy új járművet a megadott típus (FastCar vagy SlowCar) és a hozzá tartozó termék (Product) alapján. A metódus növeli a globális járműazonosítót, példányosítja a járművet és beállítja az ID-ját (ismeretlen típus esetén kivételt dob). Ezután levonja a jármű árát az egyenlegből, beleteszi a parkoló járművek (parkingVehicles) listájába, debug információkat ír ki a konzolra a járműről és annak összetevőiről (Ingredients), majd kiváltja a nézetet frissítő OnVehicleBought eseményt.
        /// </summary>
        public void BuyVehicle(VehicleType vehicleType, Product product) 
        {
            Vehicle vehicle;
            biggestVehicleID += 1;
            switch(vehicleType)
            {
                case VehicleType.FastCar:
                    vehicle = new FastCar(product);
                    vehicle.ID = biggestVehicleID;
                    break;

                case VehicleType.SlowCar:
                    vehicle = new SlowCar(product);
                    vehicle.ID = biggestVehicleID;
                    break;

                default:
                    throw new ArgumentException("Invalid vehicle purchase");
            }

            ChangeBalance(-vehicle.cost);

            parkingVehicles.Add(vehicle);
            System.Diagnostics.Debug.WriteLine($"Bought a {vehicleType} with ID {vehicle.ID} for product {product.GetType().Name}");
            if(vehicle.Ingredients != null)
            {
                System.Diagnostics.Debug.WriteLine($"Vehicle has {vehicle.Ingredients.Count} ingredients:");
                for (int i = 0; i < vehicle.Ingredients.Count; i++)
                {
                    System.Diagnostics.Debug.WriteLine($"Ingredient: {vehicle.Ingredients[i].GetType().Name}, amount: {vehicle.IngredientAmounts[i]}, capacity: {vehicle.IngredientCapacities[i]}");
                }
            }
            OnVehicleBought(vehicle);
        }

        public void ChosenVehicle(Vehicle v)
        {
            VehicleForRoute = v;
        }


        /// <summary>
        /// AdjacentRoad
        /// Megvizsgálja a megadott x és y koordináták 4 irányú (észak, dél, kelet, nyugat) szomszédságát, és visszaadja, hogy van-e velük határos út. A metódus először lenullázza a szomszédokat nyilvántartó 'news' tömböt, majd ha a pályán belül talál szomszédos utat (Road) vagy hidat (Bridge), beállítja a tömb megfelelő elemét 1-re. Csak akkor tér vissza igaz (true) értékkel, ha létezik legalább egy szomszédos közlekedési elem, és az összes megtalált szomszéd magasságkülönbsége (Z érték) az adott mezőhöz képest szigorúan kisebb, mint 2.
        /// </summary>
        public bool AdjacentRoad(int x, int y)
        {
            bool AnyRoad = false;
            bool RightHeight = true;
            for(int i = 0; i < 4; i++)
            {
                news[i] = 0;
            }
            (int dx, int dy)[] directions =
                {
                             (-1,0), 
                    (0,-1),           (0,1),
                              (1,0),  
                };

            foreach (var (dx, dy) in directions)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < board.GetLength(0) &&
                    ny >= 0 && ny < board.GetLength(1))
                {
                    if ((board[nx, ny].fieldType is Road || board[nx, ny].fieldType is Bridge))
                    {
                        if ((dx, dy) == (-1, 0)) news[0] = 1;
                        if ((dx, dy) == (0, 1)) news[1] = 1;
                        if ((dx, dy) == (0, -1)) news[2] = 1;
                        if ((dx, dy) == (1, 0)) news[3] = 1;
                        RightHeight = (Math.Abs(board[nx, ny].Z - board[x, y].Z) < 2) && RightHeight; 
                        AnyRoad = true;
                    }
                }
            }
            
            return (AnyRoad && RightHeight);
        }

        /// <summary>
        /// BuyBridge
        /// Új hidat épít a megadott x és y koordinátákon, ha az adott mező Láva (Lava), nincs rajta lakos (Citizen), csatlakozik egy meglévő úthoz, és a mező folyó státusza megegyezik az isRiver paraméterrel. Az isFast logikai értéktől függően egy gyors (FastBridge) vagy lassú (SlowBridge) hidat példányosít. Sikeres építés esetén a szomszédos mezők alapján beállítja a híd irányát (Észak-Dél vagy Kelet-Nyugat), levonja az adott híd típusához tartozó költséget a játékos egyenlegéből, majd az OnFieldChanged eseménnyel frissíti a felhasználói felületet.
        /// </summary>
        public void BuyBridge(int x, int y, bool isFast, bool isRiver) 
        {
            if (board[x, y].fieldType is Lava lavaField && board[x, y].industry!.product is not Citizen)
            {
                if (lavaField.isRiver && isRiver && AdjacentRoad(x,y))
                {
                    if (isFast)
                    {
                        board[x, y].fieldType =  new FastBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((FastBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((FastBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-((FastBridge)board[x,y].fieldType!).cost);
                        OnFieldChanged(x, y, board[x, y].fieldType!);
                    }
                    else
                    {
                        board[x, y].fieldType = new SlowBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((SlowBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((SlowBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-((SlowBridge)board[x, y].fieldType!).cost);
                        OnFieldChanged(x, y, board[x, y].fieldType!);
                    }
                }else if(lavaField.isRiver == false && isRiver == false && AdjacentRoad(x, y))
                {
                    if (isFast)
                    {
                        board[x, y].fieldType = new FastBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((FastBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((FastBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-((FastBridge)board[x, y].fieldType!).cost);
                        OnFieldChanged(x, y, board[x, y].fieldType!);
                    }
                    else
                    {
                        board[x, y].fieldType = new SlowBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((SlowBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((SlowBridge)board[x, y].fieldType!).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-((SlowBridge)board[x, y].fieldType!).cost);
                        OnFieldChanged(x, y, board[x, y].fieldType!);
                    }
                }
                else
                {
                    return;
                }
                    
            }


        }

        #endregion

        #region Routes & Stops
        /// <summary>
        /// PutStop
        /// A megadott x és y koordinátákon elhelyez egy megállót, miután az IsStopValid metódus segítségével ellenőrizte annak érvényességét és lekérte a hozzá tartozó ipart (Industry). Ha az elhelyezés érvényes, a játéktábla adott mezőjének isStop tulajdonságát igazra állítja, létrehoz egy új Stop objektumot, debug információkat ír ki róla a konzolra (pozíció, termék típusa és mennyisége), majd hozzáadja az új megállót a belső megállókat nyilvántartó (stops) listához.
        /// </summary>
        
        public void PutStop(int x, int y) 
        {
            if (board[x, y].isStop) return;

            bool isValid;
            Industry? industry;

            //if(industry is EmptyIndustry)
            (isValid, industry) = IsStopValid(x, y);

            if (isValid)
            {
                board[x, y].isStop = true;
                //board[x,y].industry = industry;

                Stop stop = new Stop(x,y, industry!);
                System.Diagnostics.Debug.WriteLine($"Stop created at ({x}, {y}) product: {industry!.product.GetType().Name}, product amount: {industry.productAmount}");
                stops.Add(stop);
            }
            
        }

        /// <summary>
        /// SelectStop
        /// Megvizsgálja, hogy a játéktábla megadott x és y koordinátáján található mező megállóként (isStop) van-e megjelölve. Ha igen, végigiterál a globális megállók (stops) listáján, megkeresi az adott koordinátákkal rendelkező Stop objektumot, és hozzáadja azt a tervezett útvonal megállóit gyűjtő (StopsForRoute) listához.
        /// </summary>
        public void SelectStop(int x, int y)
        {
            if (board[x,y].isStop == true)
            {
                foreach(Stop stop in stops)
                {
                    if (stop.X == x && stop.Y == y)
                    {
                        System.Diagnostics.Debug.WriteLine($"{stop.industry.product.GetType().Name} stop selected successfully!");
                        StopsForRoute.Add(stop);
                    }
                }
            }
        }

        public void DeSelectStop(int x, int y)
        {
            if (board[x, y].isStop == false) return;

            Stop stopToRemove;

            foreach (Stop stop in stops)
            {
                if (stop.X == x && stop.Y == y)
                {
                    stopToRemove = stop;
                    StopsForRoute.Remove(stopToRemove);
                    break;
                }
            }
        }
        /// <summary>
        /// IsStopValid
        /// Megvizsgálja, hogy a megadott x és y koordinátákon érvényes-e egy megálló elhelyezése, és visszatér egy logikai értékből és egy iparból (Industry) álló párral. A metódus azonnal hamis értékkel tér vissza, ha az adott mező nem út (Road), vagy ha az ott lévő termék lakos (Citizen). Ha ezek a feltételek teljesülnek, végigiterál a mező mind a 8 (vízszintes, függőleges és átlós) szomszédján: ha a pályán belül talál olyan szomszédot, amelynek az ipara nem üres (EmptyIndustry), akkor igaz (true) értékkel és a megtalált ipar objektumával tér vissza, ellenkező esetben pedig hamis értékkel és egy új, alapértelmezett iparral zárul.
        /// </summary>
        public (bool, Industry?) IsStopValid(int x, int y)
        {
            if (board[x, y].fieldType is not Road || board[x, y].industry!.product is Citizen) return (false, new Industry());
            (int dx, int dy)[] directions =
                {
                    (-1,-1), (-1,0), (-1,1),
                    (0,-1),           (0,1),
                    (1,-1),  (1,0),  (1,1)
                };

            foreach (var (dx, dy) in directions)
            {
                int nx = x + dx;
                int ny = y + dy;

                if (nx >= 0 && nx < board.GetLength(0) &&
                    ny >= 0 && ny < board.GetLength(1))
                {
                    if (board[nx, ny].industry is not EmptyIndustry)
                    {
                        return (true, board[nx, ny].industry!);
                    }

                }
            }
            return (false, new Industry());
        }


        /// <summary>
        /// CreateRoute
        /// Létrehoz egy új útvonalat a kiválasztott jármű (VehicleForRoute) számára. Ha nincs kiválasztott jármű, a metódus azonnal visszatér. Ellenkező esetben példányosít egy RouteManager-t, hozzáadja az útvonalhoz kijelölt összes megállót (StopsForRoute), majd kiüríti ezt a gyűjtőlistát. Ezután megkísérli megtervezni az útvonalat a DesignRoute segítségével: ha sikeres, kikeresi és eltávolítja a járművet a parkoló járművek (parkingVehicles) listájából, áthelyezi a mozgó járművek (movingVehicles) közé, beállítja a játéktábla érintett celláján az isVehicle státuszt, majd kiváltja a sikeres induláshoz tartozó eseményeket (OnAddMovingVehicle, OnCarStepped). Ha az útvonaltervezés nem lehetséges, az OnCreateRouteFailed eseményt hívja meg.
        /// </summary>
        public void CreateRoute()
        {
            if (VehicleForRoute is null) return;
            RouteManager routemanager = new RouteManager(VehicleForRoute);
            foreach(Stop stop in StopsForRoute)
            {
                routemanager.AddStop(stop);
            }
            StopsForRoute.Clear();
            if (routemanager.DesignRoute(this))
            {
                int j = -1;
                for (int i = 0; i < parkingVehicles.Count; i++)
                {
                    if (parkingVehicles[i].ID == VehicleForRoute.ID)
                    {
                        j = i;
                    }
                }
                if (j != -1) parkingVehicles.RemoveAt(j);

                movingVehicles.Add(routemanager.Vehicle);
                OnAddMovingVehicle(VehicleForRoute);
                board[VehicleForRoute.x,VehicleForRoute.y].isVehicle = true;
                OnCarStepped((VehicleForRoute.x, VehicleForRoute.y, VehicleForRoute.dir));
            }
            else
            {
                OnCreateRouteFailed();
            }
        }

        /// <summary>
        /// RemoveVehicle
        /// Eltávolítja a megadott járművet az aktív útvonaláról és a mozgó járművek listájából. A metódus megkeresi a járművet a mozgó járművek (movingVehicles) között az azonosítója (ID) alapján, és ha megtalálja, eltávolítja onnan. Ezután a játéktábla (board) aktuális pozícióján hamisra állítja az isVehicle jelzőt, meghívja az OnCarStepped eseményt a nézet frissítéséhez, lenullázza a jármű útvonal-indexét, teljesen törli a járműhöz rendelt útvonalat (ClearRoute), végül pedig visszateszi a járművet a parkoló járművek (parkingVehicles) listájába.
        /// </summary>
        public void RemoveVehicleFromRoute(Vehicle vehicle)
        {
            int j = -1;
            for (int i = 0; i < movingVehicles.Count; i++)
            {
                if (movingVehicles[i].ID == vehicle.ID)
                {
                    j = i;
                }
            }
            if (j != -1) movingVehicles.RemoveAt(j);
            board[vehicle.x, vehicle.y].isVehicle = false;
            OnCarStepped((vehicle.x, vehicle.y, vehicle.dir));
            vehicle.route_index = 0;
            vehicle.ClearRoute();
            parkingVehicles.Add(vehicle);

        }
        /// <summary>
        /// Terradorm
        /// Módosítja a megadott x és y koordinátájú mező magasságát (tereprendezés), az isUp paramétertől függően emelve vagy süllyesztve azt. A metódus a TerraformManager segítségével kísérli meg végrehajtani a módosítást a játéktáblán. Ha a művelet sikertelen (például érvénytelen szintkülönbség miatt), csak egy debug üzenetet ír ki és megszakítja a futást. Sikeres tereprendezés esetén rögzíti az új magasságot (Z érték) a konzolon, levonja a művelet költségét (TerraformPrice) a játékos egyenlegéből, végül pedig az OnFieldChanged eseménnyel frissíti a megváltozott mező grafikus megjelenítését.
        /// </summary>
        public void Terraform(int x, int y, bool isUp)
        {
            bool successfulTerraform = TerraformManager.Terraform(board, x, y, isUp);

            if (!successfulTerraform) {
                System.Diagnostics.Debug.WriteLine($"Failed to terraform at ({x}, {y}), isUp: {isUp}");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Successful terraform at ({x}, {y}), isUp: {isUp}, new Z: {board[x, y].Z}");
            ChangeBalance(-TerraformPrice);
            OnFieldChanged(x, y, board[x, y].fieldType!);
        }

        #endregion

        #region Money handling
        /// <summary>
        /// ChangeBalance
        /// Megváltoztatja a játékos egyenlegét a megadott összeggel (amount). Ha a módosítás hatására a pénz (money) 0 alá csökkenne, a metódus leállítja a fő időzítőt, az összes aktív mozgó járművet biztonságosan eltávolítja az útvonaláról, és meghívja a játék végét jelző OnGameOver eseményt. Ha a játékos nem megy csődbe, egyszerűen hozzáadja az összeget a jelenlegi egyenleghez, majd az OnMoneyChanged eseménnyel frissíti a felhasználói felületet.
        /// </summary>
        public void ChangeBalance(int amount)
        {
            if (money + amount < 0)
            {
                timer.Stop();
                List<Vehicle> removevehicle = new List<Vehicle>(movingVehicles);
                foreach (Vehicle vehicle in removevehicle)
                {
                    RemoveVehicleFromRoute(vehicle);
                }
                OnGameOver();
                return;
            }
            money += amount;
            OnMoneyChanged(money);
            
        }
        #endregion

        public void SaveGame(string path)
        {
            GameData gameData = new GameData(this);

            DataAccess.SaveGameDataToJson(gameData, path);
        }

        #region Events
        public void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }
        public void OnVehicleBought(Vehicle vehicle)
        {
            VehicleBought?.Invoke(this, vehicle); //ezzel kellene jelezni, hogy jelenjen meg a nem mozgó járművek listájában az autó
        }

        public void OnAddMovingVehicle(Vehicle v)
        {
            AddMovingVehicle?.Invoke(this, v);
        }

        public void OnMoneyChanged(int m)
        {
            MoneyChanged?.Invoke(this, m);
        }

        public void OnCreateRouteFailed()
        {
            CreateRouteFailed?.Invoke(this, EventArgs.Empty);
        }

        public void OnCarStepped((int,int,int) tuple)
        {
            CarStepped?.Invoke(this, tuple);
        }

        public void OnFieldChanged(int x, int y, FieldType type)
        {
            board[x, y].fieldType = type;
            FieldChanged?.Invoke(this, new FieldChangedEventArgs(x,y,type));
        }

        public void OnProduce(Industry industry)
        {
            Produce?.Invoke(this, industry);
        }

        private void OnTimerTick(int gameTimer)
        {
            TimerTick?.Invoke(this, gameTimer);
        }

        private void OnRefresh()
        {
            Refresh?.Invoke(this, EventArgs.Empty);
        }
        #endregion

        #region Timer & Ticks

        public void SimulateTimerElapsed()
        {
            Timer_Elapsed(this, EventArgs.Empty);
        }

        public void BaseTime()
        {
            gameTime = GAMETIME_BASE;
            timeStatus = TimeStatus.BASE;
        }
        public void FastTime()
        {
            gameTime = GAMETIME_FAST;
            timeStatus = TimeStatus.FAST;

        }

        public void FastestTime()
        {
            gameTime = GAMETIME_FASTEST;
            timeStatus = TimeStatus.FASTEST;
        }

        public void StopTimer()
        {
            timer.Stop();
        }
        public void StartTimer()
        {
            timer.Start();
        }

        /// <summary>
        /// Timer_Elapsed
        /// A fő időzítő eseménykezelője, amely minden lefutáskor csökkenti a belső gameTime számlálót. Amikor ez a számláló eléri a nullát, megtörténik egy játékbeli léptetés (tick): a rendszer megnöveli a globális játékidőt (gameTimer), kiváltja az OnTimerTick eseményt a felület frissítéséhez, majd frissíti az erdők (GameModelTickForest) és a mozgó járművek (GameModelTickVehicles) állapotát. Végül a játék aktuális sebességbeállításától (timeStatus) függően (alap, gyors vagy leggyorsabb) újraindítja a belső számlálót a megfelelő konstans értékkel, így szabályozva a játék futási sebességét.
        /// </summary>
        private void Timer_Elapsed(Object? sender, EventArgs e)
        {

            gameTime--;
            
            if(gameTime == 0)
            {
                gameTimer++;
                OnTimerTick(gameTimer);
                GameModelTickForest();
                GameModelTickVehicles();
                switch (timeStatus)
                {
                    case TimeStatus.BASE:
                        gameTime = GAMETIME_BASE;
                        break;
                    case TimeStatus.FAST:
                        gameTime = GAMETIME_FAST;
                        break;
                    case TimeStatus.FASTEST:
                        gameTime = GAMETIME_FASTEST;
                        break;
                }
            }
            
        }

        private void GameModelTickVehicles()
        {
            foreach (Vehicle v in movingVehicles)
            {
               if (v.CanVehicleMove(board, movingVehicles)) CarStep(v); 
            }
        }

        /// <summary>
        /// GameModelTickForest
        /// A metódus kezeli a játéktér időbeli frissítését (tick) az erdők növekedése és az ipari termelés tekintetében. Először végigiterál az összes erdőn, meghívja rajtuk a növekedési logikát (GrowthTick), és ha egy erdő készen áll a terjedésre, egy biztonságos átmeneti lista használatával (elkerülve az iteráció közbeni módosítást) új erdőmezőket generál. Miután hozzáfűzte az újonnan kinőtt erdőket a fő listához, a metódus végigmegy a regisztrált ipari épületeken is: a farmok (Farm) és gyárak (Factory) esetében lefuttatja a termelési ciklust (Produce), majd az OnProduce eseménnyel jelzi az új termékek létrejöttét a felhasználói felület számára.
        /// </summary>
        private void GameModelTickForest()
        {
            //forest growth
            List<Field> newForests = new List<Field>(); //ez a lista azért kell, hogy ne módosítsuk a forests listát miközben iterálunk rajta, mert az hibához vezetne
            
            foreach (Field f in forests)
            {
                if (f.fieldType is not Forest) throw new Exception();
                Forest forest = (Forest)f.fieldType;

                if (forest.GrowthTick())
                {
                    OnFieldChanged(f.X, f.Y, forest);
                }

                if (forest.readyToSpread)
                {
                    Direction? dir = forest.Spread(this, f.X, f.Y);

                    newForests = ForestSpread(dir, f, newForests);
                    forest.readyToSpread = false;

                }
            }

                
            forests = forests.Concat(newForests).ToList();

            //industry production

            foreach (Industry industry in industries)
            {
                if (industry is Farm)
                {
                    Farm farm = (Farm)industry;
                    farm.Produce();
                    OnProduce(farm);
                }
                else if (industry is Factory)
                {
                    Factory factory = (Factory)industry;
                    factory.Produce();
                    OnProduce(factory);

                }
            }
        }

        /// <summary>
        /// ForestSpread
        /// Kezeli egy meglévő erdőmező (f) terjedését a megadott irányba (dir). Ha az irány paraméter nem null, a metódus az iránynak (fel, le, balra, jobbra) megfelelő szomszédos cellára egy új erdőt (Forest) jelez a grafikus felület felé az OnFieldChanged esemény meghívásával, majd az érintett mezőt a játéktábláról (board) hozzáadja a paraméterként kapott átmeneti erdőlistához (forests), végül visszatér ezzel a kibővített listával.
        /// </summary>
        private List<Field> ForestSpread(Direction? dir, Field f, List<Field> forests)
        {
            if (dir is not null)
            {
                switch (dir)
                {
                    case Direction.UP:
                        OnFieldChanged(f.X, f.Y - 1, new Forest());
                        forests.Add(board[f.X, f.Y - 1]);
                        break;

                    case Direction.DOWN:
                        OnFieldChanged(f.X, f.Y + 1, new Forest());
                        forests.Add(board[f.X, f.Y + 1]);
                        break;

                    case Direction.LEFT:
                        OnFieldChanged(f.X - 1, f.Y, new Forest());
                        forests.Add(board[f.X - 1, f.Y]);
                        break;

                    case Direction.RIGHT:
                        OnFieldChanged(f.X + 1, f.Y, new Forest());
                        forests.Add(board[f.X + 1, f.Y]);
                        break;

                }
            }

            return forests;
        }
        #endregion

        /// <summary>
        /// CarStep
        /// Végrehajt egy mozgási lépést a megadott járművel (v) az előre megtervezett útvonalán. A metódus először kezeli a régi pozíció elhagyását (frissíti a játéktáblát és feloldja az esetleges "két jármű egy mezőn" állapotot), majd frissíti a jármű x és y koordinátáit az útvonal következő elemére. Ezután megvizsgálja a rákövetkező lépést: ha az emelkedő (nagyobb Z érték), lelassítja a járművet (timeUntilMoves növelése). Kiszámítja és beállítja a jármű új irányát (dir), és kezeli a lehetséges ütközéseket (areTwoVehicles). Ha a jármű egy megállóra (isStop) ér, lebonyolítja a fel/lerakodási tranzakciót, és lakosok (Citizen) szállítása esetén kiszámítja és jóváírja a megkeresett pénzt. Végül frissíti a nézetet (OnCarStepped), csökkenti a jármű állapotát (brokness), és ellenőrzi a lehetséges meghibásodást (CheckVehicleBrokness).
        /// </summary>
        public void CarStep(Vehicle v)
        {

            (int,int)[] localRoute = new (int,int)[v.Route.Count];
            localRoute = v.Route.ToArray();

            //before changing routeIndex
            if(board[v.x, v.y].areTwoVehicles)
            {
                board[v.x, v.y].areTwoVehicles = false;
                if(v.dir == 1)
                {
                    board[v.x, v.y].isVehicle = true;
                    OnCarStepped((v.x, v.y, 2));
                }
                else if(v.dir == 2)
                {
                    board[v.x, v.y].isVehicle = true;
                    OnCarStepped((v.x, v.y, 1));
                }
                else if(v.dir == 3)
                {
                    board[v.x, v.y].isVehicle = true;
                    OnCarStepped((v.x, v.y, 4));
                }
                else
                {
                    board[v.x, v.y].isVehicle = true;
                    OnCarStepped((v.x, v.y, 3));
                }
                
            }
            else
            {
                board[v.x, v.y].isVehicle = false;
                OnCarStepped((v.x, v.y, v.dir));
            }
            
           

            //int oldX = v.x;
            //int oldY = v.y;

            //change routeIndex, set new x and y for vehicle
            
            v.route_index++;
            v.x = localRoute[v.route_index % (localRoute.Length)].Item1;
            v.y = localRoute[v.route_index % (localRoute.Length)].Item2;

            //board[v.x, v.y].isVehicle = true;
            //On
            //((v.x, v.y, v));

            //look for next tile, to see if it's an incline
            int nextX = localRoute[(v.route_index + 1) % (localRoute.Length)].Item1;
            int nextY = localRoute[(v.route_index + 1) % (localRoute.Length)].Item2;
            
            if (board[nextX, nextY].Z > board[v.x, v.y].Z) v.timeUntilMoves += 1;

            if (v.x != nextX && v.y == nextY && board[v.x, v.y].isVehicle)
            {
                v.dir = 5;
                board[v.x, v.y].areTwoVehicles = true;
            }
            else if (v.x == nextX && v.y != nextY && board[v.x, v.y].isVehicle)
            {
                v.dir = 6;
                board[v.x, v.y].areTwoVehicles = true;
            }
            else if (v.x == nextX && v.y > nextY && board[v.x, v.y].areTwoVehicles == false)
            {
                v.dir = 4;
                board[v.x, v.y].isVehicle = true;

            }
            else if (v.x == nextX && v.y < nextY && board[v.x, v.y].areTwoVehicles == false)
            {
                v.dir = 3;
                board[v.x, v.y].isVehicle = true;
            }
            else if (v.x < nextX && v.y == nextY && board[v.x, v.y].areTwoVehicles == false)
            {
                v.dir = 2;
                board[v.x, v.y].isVehicle = true;
            }
            else if (v.x > nextX && v.y == nextY && board[v.x, v.y].areTwoVehicles == false)
            {
                v.dir = 1;
                board[v.x, v.y].isVehicle = true;
            }
           
            if (board[v.x, v.y].isStop)
            {
                Stop stop = findStopAt(v.x, v.y);

                    if (stop.industry.product is Citizen)
                    {
                        int moneyEarned = v.CityStopReached();
                        ChangeBalance(moneyEarned);
                    }
                    v.Transaction(stop);
                    OnProduce(stop.industry);
            }

            OnCarStepped((v.x, v.y, v.dir));
            v.brokness--;
            CheckVehicleBrokness(v);

            /*
            for (int i = 0; i < (localRoute.Length); i++)
            {
                if (localRoute[i].Item1 == v.x && localRoute[i].Item2 == v.y)
                {
                    if (i == (localRoute.Length) - 1)
                    {
                        board[v.x, v.y].isVehicle = false;
                        OnCarStepped((v.x, v.y));
                        v.x = localRoute[0].Item1;
                        v.y = localRoute[0].Item2;
                        board[v.x,v.y].isVehicle = true;
                        OnCarStepped((v.x, v.y));

                    }
                    else
                    {
                        board[v.x, v.y].isVehicle = false;
                        OnCarStepped((v.x, v.y));
                        v.x = localRoute[i + 1].Item1;
                        v.y = localRoute[i + 1].Item2;
                        board[v.x, v.y].isVehicle = true;

                        if (board[v.x, v.y].fieldType is Stop)
                        {
                            Stop stop = (Stop)board[v.x, v.y].fieldType!;

                            if (stop.industry.product is Citizen)
                            {
                                int moneyEarned = v.CityStopReached();
                                ChangeBalance(moneyEarned);
                            }
                            v.Transaction(stop);
                        }

                        OnCarStepped((v.x, v.y));
                        break;
                    }

                }

            }
            */
        }
        /// <summary>
        /// findStopAt
        /// Végigiterál a megállók (stops) belső listáján, és megkeresi a megadott x és y koordinátákon található megállót. Ha megtalálja, egy debug üzenet kiírása után (amely tartalmazza a pozíciót és az ottani ipar termékének típusát) visszatér az adott Stop objektummal. Ha a lista végére érve sem talál megállót a keresett helyen, kivételt (Exception) dob.
        /// </summary>
        private Stop findStopAt(int x, int y)
        {
            foreach (Stop stop in stops)
            {
                if (stop.X == x && stop.Y == y)
                {
                    System.Diagnostics.Debug.WriteLine($"Stop found at ({x}, {y}), product: {stop.industry.product.GetType()}");
                    return stop;
                }
            }

            throw new Exception($"No stop found at ({x}, {y})");
        }

        /// <summary>
        /// CheckVehicleBrokness
        /// Megvizsgálja a paraméterként kapott jármű elhasználódási szintjét (brokness). Ha ez az érték eléri a nullát (vagyis a jármű elromlott), a metódus automatikusan megjavítja azt: visszaállítja a számlálót a járműhöz tartozó maximális lépésszámra (stepsUntilBroken), és a ChangeBalance metódus segítségével levonja a specifikus javítási költséget (repairCost) a játékos egyenlegéből.
        /// </summary>
        private void CheckVehicleBrokness(Vehicle vehicle)
        {
            if(vehicle.brokness == 0)
            {
                vehicle.brokness = vehicle.stepsUntilBroken;
                ChangeBalance(-vehicle.repairCost);
            }
        }
        
    }
}
