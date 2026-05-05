using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        #endregion

        #region Properties
        public List<Stop> stops { get; set; }

        public List<Vehicle> parkingVehicles { get; set; } 

        public List<Vehicle> movingVehicles { get; set; }
        

        private Vehicle? VehicleForRoute { get; set; }
        private List<Stop> StopsForRoute { get; set; }
        public int money { get; private set; }
        #endregion

        #region Instances of other classes
        private TerraformingManager TerraformManager = new TerraformingManager();
        private GameData GameData { get; set; }
        private DataAccess DataAccess;
        #endregion

        #region Events
        public event EventHandler? GameOver;
        public event EventHandler<int>? MoneyChanged;
        public event EventHandler<FieldChangedEventArgs>? FieldChanged;
        public event EventHandler<Vehicle>? VehicleBought; //updatelni kell a parkoló autók listáját
        public event EventHandler? Refresh;
        public event EventHandler<(int,int, int)>? CarStepped;
        public event EventHandler<Vehicle>? AddMovingVehicle;
        public event EventHandler? CreateRouteFailed;

        public event EventHandler<int>? TimerTick;
        int[] news;
        #endregion


        public ITimer timer;
        public int gameTime;
        
        public TimeStatus timeStatus { get;  private set; }
        public int gameTimer;
        private int biggestVehicleID;


        public Field[,] board { get; private set; }
        private List<Field> forests;
        private List<Industry> industries;

        public GameModel(DataAccess dataAccess)
        {
            stops = new List<Stop>();
            board = new Field[50, 100];
            parkingVehicles = new List<Vehicle>();
            movingVehicles = new List<Vehicle>();
            StopsForRoute = new List<Stop>();

            GameData = new GameData();
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
            money = 10000;
            biggestVehicleID = 0;
            news = new int[4];


            timer.Start();
        }
        
        public Field Board(int i, int j)
        {
            return board[i, j];
        }


        #region Initialization
        public void StartNewGame()
        {
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
                    OnFieldChanged(i, j, board[i, j].fieldType);
                }
            }
            
            gameTime = GAMETIME_BASE;

            gameTimer = 0;
            timeStatus = TimeStatus.BASE;
            money = 10000;
            biggestVehicleID = 0;
            OnMoneyChanged(money);

            timer.Start();
        }

        public void InitializeBoard()
        {
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    board[i, j] = new Field(i, j, 1, new Industry(new Product()), new FieldType(), false, false, false, 1,Direction.NONE);
                }
            }
        }
        #endregion

        #region Buying and Building
        public void BuyRoad(int x, int y) {

            if (board[x,y].fieldType is Grass && board[x,y].industry.product is not Citizen && AdjacentRoad(x, y))
            {           
                board[x, y].fieldType = new Road();
                if ((news[0]==1 || news[3] == 1) && news[1]==0 && news[2] == 0 ) ((Road)board[x, y].fieldType).roadType = RoadType.STRAIGHTNS;
                else if(news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((Road)board[x, y].fieldType).roadType = RoadType.STRAIGHTWE;
                ChangeBalance(-20);
                OnFieldChanged(x, y, board[x, y].fieldType!);
            }else if (board[x, y].fieldType is Forest && board[x, y].industry.product is not Citizen && AdjacentRoad(x, y)) 
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
                if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((Road)board[x, y].fieldType).roadType = RoadType.STRAIGHTNS;
                else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((Road)board[x, y].fieldType).roadType = RoadType.STRAIGHTWE;
                ChangeBalance(-60);
                OnFieldChanged(x, y, board[x, y].fieldType!);
            }

        }
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
        
        

        public bool AdjacentRoad(int x, int y)
        {
            bool isTrue = false;
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
                    if ((board[nx, ny].fieldType is Road || board[nx, ny].fieldType is Bridge) && Math.Abs(board[nx,ny].Z - board[x,y].Z)<2)
                    {
                        if ((dx, dy) == (-1, 0)) news[0] = 1;
                        if ((dx, dy) == (0, 1)) news[1] = 1;
                        if ((dx, dy) == (0, -1)) news[2] = 1;
                        if ((dx, dy) == (1, 0)) news[3] = 1;
                        isTrue = true;
                    }
                }
            }
            
            return isTrue;
        }
        public void BuyBridge(int x, int y, bool isFast, bool isRiver) {

            if (board[x, y].fieldType is Lava lavaField && board[x, y].industry.product is not Citizen)
            {
                if (lavaField.isRiver && isRiver && AdjacentRoad(x,y))
                {
                    if (isFast)
                    {
                        board[x, y].fieldType = new FastBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((FastBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((FastBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-400);
                        OnFieldChanged(x, y, board[x, y].fieldType);
                    }
                    else
                    {
                        board[x, y].fieldType = new SlowBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((SlowBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((SlowBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-200);
                        OnFieldChanged(x, y, board[x, y].fieldType);
                    }
                }else if(lavaField.isRiver == false && isRiver == false && AdjacentRoad(x, y))
                {
                    if (isFast)
                    {
                        board[x, y].fieldType = new FastBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((FastBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((FastBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-400);
                        OnFieldChanged(x, y, board[x, y].fieldType);
                    }
                    else
                    {
                        board[x, y].fieldType = new SlowBridge(isRiver);
                        if ((news[0] == 1 || news[3] == 1) && news[1] == 0 && news[2] == 0) ((SlowBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTNS;
                        else if (news[0] == 0 && (news[1] == 1 || news[2] == 1) && news[3] == 0) ((SlowBridge)board[x, y].fieldType).roadType = RoadType.STRAIGHTWE;
                        ChangeBalance(-200);
                        OnFieldChanged(x, y, board[x, y].fieldType);
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
        public void PutStop(int x, int y) {

            bool isValid;
            Industry industry;

            //if(industry is null)
            (isValid, industry) = IsStopValid(x, y);

            if (board[x, y].fieldType is Road && isValid && board[x, y].industry!.product is not Citizen)
            {
                board[x, y].isStop = true;
                //board[x,y].industry = industry;

                Stop stop = new Stop(x,y, industry);
                System.Diagnostics.Debug.WriteLine($"Stop created at ({x}, {y}) product: {industry.product.GetType().Name}, product amount: {industry.productAmount}");
                stops.Add(stop);
            }
            
        }
        

        public void SelectStop(int x, int y)
        {
            if (board[x,y].isStop == true)
            {
                foreach(Stop stop in stops)
                {
                    if (stop.X == x && stop.Y == y)
                    {
                        StopsForRoute.Add(stop);
                    }
                }
            }
        }

        public (bool, Industry) IsStopValid(int x, int y)
        {
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
                    if (board[nx, ny].industry.product is not null)
                    {
                        return (true, board[nx, ny].industry);
                    }

                }
            }
            return (false, new Industry(new Product()));
        }

        

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

        public void Terraform(int x, int y, bool isUp)
        {
            bool successfulTerraform = TerraformManager.Terraform(board, x, y, isUp);

            if (!successfulTerraform) {
                System.Diagnostics.Debug.WriteLine($"Failed to terraform at ({x}, {y}), isUp: {isUp}");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"Successful terraform at ({x}, {y}), isUp: {isUp}, new Z: {board[x, y].Z}");
            ChangeBalance(-250);
            OnFieldChanged(x, y, board[x, y].fieldType!);
        }

        #endregion

        #region Money handling
        public void ChangeBalance(int amount)
        {
            if (money + amount < 0)
            {
                timer.Stop();
                OnGameOver();
                return;
            }
            money += amount;
            OnMoneyChanged(money);
            
        }
        #endregion

        public async Task LoadGame(String path) { }
        public async Task SaveGame(String path) { }


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
                }
                else if (industry is Factory)
                {
                    Factory factory = (Factory)industry;
                    factory.Produce();

                }
            }
        }

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

        private void CarStep(Vehicle v)
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
            v.x = localRoute[v.route_index % (localRoute.Length - 1)].Item1;
            v.y = localRoute[v.route_index % (localRoute.Length - 1)].Item2;

            //board[v.x, v.y].isVehicle = true;
            //On
            //((v.x, v.y, v));

            //look for next tile, to see if it's an incline
            int nextX = localRoute[(v.route_index + 1) % (localRoute.Length - 1)].Item1;
            int nextY = localRoute[(v.route_index + 1) % (localRoute.Length - 1)].Item2;
            
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
           
            
           
            //OnCarStepped((v.x, v.y, v));

            //determine which direction the car is going, by the previous and current tile
            //v.direction = v.DecideDirection(nextX, nextX, v.x, v.y);

            /*
            //if we arrived at a stop, handle transactions
            if (board[v.x, v.y].isStop)
            {
                Stop stop = findStopAt(v.x, v.y);
            }
            */
            /*
            else
            {
                if (board[v.x, v.y].areTwoVehicles)
                {
                    board[v.x, v.y].areTwoVehicles = false;
                }
                else
                {
                    board[v.x, v.y].isVehicle = false;
                    OnCarStepped((v.x, v.y, v));
                }
            */
                /*
                v.route_index++;
                v.x = localRoute[v.route_index].Item1;
                v.y = localRoute[v.route_index].Item2;
                board[v.x, v.y].isVehicle = true;*/

                if (board[v.x, v.y].isStop)
                {
                    Stop stop = findStopAt(v.x, v.y);

                    if (stop.industry.product is Citizen)
                    {
                        int moneyEarned = v.CityStopReached();
                        ChangeBalance(moneyEarned);
                    }
                    v.Transaction(stop);
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
        private Stop findStopAt(int x, int y)
        {
            foreach (Stop stop in stops)
            {
                if (stop.X == x && stop.Y == y)
                {
                    return stop;
                }
            }

            throw new Exception($"No stop found at ({x}, {y})");
        }

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
