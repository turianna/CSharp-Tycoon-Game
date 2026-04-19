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
        public List<Stop> stops { get; set; }

        public List<Vehicle> parkingVehicles { get; set; } 

        public List<Vehicle> movingVehicles { get; set; }
        public List<Field> forests { get; set; }

        private Vehicle VehicleForRoute { get; set; }
        private List<Stop> StopsForRoute { get; set; }
        public int money { get; set; }

        private TerraformingManager TerraformManager = new TerraformingManager();

        private GameData GameData { get; set; }
        private DataAccess DataAccess;

        public event EventHandler GameOver;
        public event EventHandler<int>? MoneyChanged;
        public event EventHandler? InsufficientFunds;
        public event EventHandler<FieldChangedEventArgs>? FieldChanged;
        public event EventHandler<Vehicle>? VehicleBought; //updatelni kell a parkoló autók listáját
        public event EventHandler Refresh;
        public event EventHandler<(int,int)>? CarStepped;
        public event EventHandler<Vehicle>? AddMovingVehicle;

        public event EventHandler<int>? TimerTick;
        public ITimer timer;
        public int gameTime;
        public int fastCarTime;
        public int slowCarTime;
        public TimeStatus timeStatus;
        public int gameTimer;
        public int biggestVehicleID;


        public Field[,] board;
        public Field Board(int i, int j)
        {
            return board[i, j];
        }

        public GameModel(DataAccess dataAccess)
        {
            stops = new List<Stop>();
            board = new Field[50, 100];
            parkingVehicles = new List<Vehicle>();
            movingVehicles = new List<Vehicle>();
            StopsForRoute = new List<Stop>();

            GameData = new GameData();
            DataAccess = dataAccess;
            InitializeBoard();
            board = DataAccess.LoadMap();
            forests = DataAccess.GetForests();

            timer = new GameTimer();
            timer.Interval = 250;
            timer.Elapsed += new EventHandler(Timer_Elapsed);

            gameTime = 4;
            fastCarTime = 1;
            slowCarTime = 3;
            gameTimer = 0;
            timeStatus = TimeStatus.BASE;
            money = 10000;
            biggestVehicleID = 0;

            
            timer.Start();
        }
       

        public void StartNewGame()
        {
            stops.Clear();
            parkingVehicles.Clear(); 
            movingVehicles.Clear();
            InitializeBoard();
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
            
            gameTime = 4;
            fastCarTime = 1;
            slowCarTime = 3;

            gameTimer = 0;
            timeStatus = TimeStatus.BASE;
            money = 10000;
            biggestVehicleID = 0;
            OnMoneyChanged(money);

            timer.Start();
        }

        
        public void InitializeBoard()
        {
            for(int i = 0; i < 50; i++)
            {
                for(int j = 0; j < 100; j++)
                {
                    board[i, j] = new Field(i, j, 0, new Industry(), new FieldType(), false, false,0);
                }
            }
        }

        public void BuyRoad(int x, int y) {

            if (board[x,y].fieldType is Grass && board[x,y].industry.product is not Citizen)
            {           
                board[x, y].fieldType = new Road();
                ChangeBalance(-20);
                OnFieldChanged(x, y, board[x, y].fieldType);

            }else if (board[x, y].fieldType is Forest && board[x, y].industry.product is not Citizen)
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
                ChangeBalance(-60);
                OnFieldChanged(x, y, board[x, y].fieldType);
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
            OnVehicleBought(vehicle);
        }

        public void ChosenVehicle(Vehicle v)
        {
            movingVehicles.Add(v); // ennek is a routemanagerbe kell kerülnie
            int j = -1;
            for (int i = 0; i < parkingVehicles.Count; i++)
            {
                if (parkingVehicles[i].ID == v.ID)
                {
                    j = i;
                }
            }
            if (j != -1) parkingVehicles.RemoveAt(j);
            VehicleForRoute = v;
        }
        
        public void OnVehicleBought(Vehicle vehicle)
        {
            VehicleBought?.Invoke(this, vehicle); //ezzel kellene jelezni, hogy jelenjen meg a nem mozgó járművek listájában az autó
        }

        public void BuyBridge(int x, int y, bool isFast, bool isRiver) {

            if (board[x, y].fieldType is Lava lavaField && board[x, y].industry.product is not Citizen)
            {
                if (lavaField.isRiver && isRiver)
                {
                    if (isFast)
                    {
                        board[x, y].fieldType = new FastBridge(isRiver);
                        ChangeBalance(-400);
                        OnFieldChanged(x, y, board[x, y].fieldType);
                    }
                    else
                    {
                        board[x, y].fieldType = new SlowBridge(isRiver);
                        ChangeBalance(-200);
                        OnFieldChanged(x, y, board[x, y].fieldType);
                    }
                }else if(lavaField.isRiver == false && isRiver == false)
                {
                    if (isFast)
                    {
                        board[x, y].fieldType = new FastBridge(isRiver);
                        ChangeBalance(-400);
                        OnFieldChanged(x, y, board[x, y].fieldType);
                    }
                    else
                    {
                        board[x, y].fieldType = new SlowBridge(isRiver);
                        ChangeBalance(-200);
                        OnFieldChanged(x, y, board[x, y].fieldType);
                    }
                }
                    
            }


        } 
        public void PutStop(int x, int y) {

            if (board[x, y].fieldType is Road && IsStopValid(x,y) && board[x, y].industry!.product is not Citizen)
            {
                board[x, y].isStop = true;
                Stop stop = new Stop(x,y);
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

        public void ModelCreateRoute()
        {
            RouteManager routemanager = new RouteManager(VehicleForRoute);
            foreach(Stop stop in StopsForRoute)
            {
                routemanager.stops.Add(stop);
            }
            StopsForRoute.Clear();
            if (routemanager.DesignRoute(this))
            {
                movingVehicles.Add(routemanager.vehicle);
                OnAddMovingVehicle(VehicleForRoute);
                board[VehicleForRoute.x,VehicleForRoute.y].isVehicle = true;
                OnCarStepped((VehicleForRoute.x, VehicleForRoute.y));
            }
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

        public bool IsStopValid(int x, int y)
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
                    if (board[nx,ny].industry.product is not null)
                    {
                        return true;
                    }
                
                }
            }
            return false;
        }


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

        public async Task LoadGame(String path) { }
        public async Task SaveGame(String path) { }

        public void Produce() { }
       
        public void OnGameOver()
        {
            GameOver?.Invoke(this, EventArgs.Empty);
        }

        public void OnAddMovingVehicle(Vehicle v)
        {
            AddMovingVehicle?.Invoke(this, v);
        }

        public void OnMoneyChanged(int m)
        {
            MoneyChanged?.Invoke(this, m);
        }

        public void OnInsufficientFunds()
        {
            InsufficientFunds?.Invoke(this, EventArgs.Empty);
        }

        public void OnCarStepped((int,int) coord)
        {
            CarStepped?.Invoke(this, coord);
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
        public void BaseTime()
        {
            gameTime = 4;
            timeStatus = TimeStatus.BASE;
        }
        public void FastTime()
        {
            gameTime = 2;
            timeStatus = TimeStatus.FAST;

        }

        public void FastestTime()
        {
            gameTime = 1;
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
            slowCarTime--;
            if(slowCarTime == 0)
            {
                GameModelTickSlowVehicles();
                switch (timeStatus)
                {
                    case TimeStatus.BASE:
                        slowCarTime = 12;
                        break;
                    case TimeStatus.FAST:
                        slowCarTime = 6;
                        break;
                    case TimeStatus.FASTEST:
                        slowCarTime = 3;
                        break;
                }
            }
            if(gameTime == 0)
            {
                gameTimer++;
                OnTimerTick(gameTimer);
                GameModelTickForest();
                GameModelTickFastVehicles();
                switch (timeStatus)
                {
                    case TimeStatus.BASE:
                        gameTime = 4;
                        break;
                    case TimeStatus.FAST:
                        gameTime = 2;
                        break;
                    case TimeStatus.FASTEST:
                        gameTime = 1;
                        break;
                }
            }
            
        }

        private void CarStep(Vehicle v)
        {

            (int,int)[] localRoute = new (int,int)[v.route.Count];
            localRoute = v.route.ToArray();
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
                        OnCarStepped((v.x, v.y));
                    }

                }

            }
        }

        private void GameModelTickFastVehicles()
        {
            foreach (Vehicle v in movingVehicles)
            {
                if(v is SlowCar)
                {
                    CarStep(v);
                }
            }
        }
        private void GameModelTickSlowVehicles()
        {
            foreach(Vehicle v in movingVehicles)
            {
                
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
        }

        private List<Field> ForestSpread(Direction? dir, Field f, List<Field> forests) 
        {
            if (dir is not null)
            {
                switch (dir)
                {
                    case Direction.UP:
                        OnFieldChanged(f.X, f.Y-1, new Forest());
                        forests.Add(board[f.X, f.Y-1]);
                        break;

                    case Direction.DOWN:
                        OnFieldChanged(f.X, f.Y+1, new Forest());
                        forests.Add(board[f.X, f.Y+1]);
                        break;

                    case Direction.LEFT:
                        OnFieldChanged(f.X-1, f.Y, new Forest());
                        forests.Add(board[f.X-1, f.Y]);
                        break;

                    case Direction.RIGHT:
                        OnFieldChanged(f.X+1, f.Y, new Forest());
                        forests.Add(board[f.X+1, f.Y]);
                        break;

                }
            }

            return forests;
        }

    }
}
