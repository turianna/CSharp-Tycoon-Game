using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class GameModel
    {
        public List<Stop> stops { get; set; }

        public List<Vehicle> vehicles { get; set; } 

        public List<Vehicle> movingVehicles { get; set; }
        public List<Forest> forests { get; set; }
        public int money { get; set; }

        private GameData GameData { get; set; }
        private DataAccess DataAccess;

        public event EventHandler<bool>? GameOver;
        public event EventHandler<int>? MoneyChanged;
        public event EventHandler<FieldChangedEventArgs>? FieldChanged;
        public event EventHandler<Vehicle>? VehicleBought; //updatelni kell a parkoló autók listáját

        public event EventHandler<int>? TimerTick;
        public ITimer timer;
        public int gameTime;
        public TimeStatus timeStatus;
        public int gameTimer;


        public Field[,] board;

        public GameModel(DataAccess dataAccess)
        {
            stops = new List<Stop>();
            board = new Field[100, 100];
            vehicles = new List<Vehicle>();
            movingVehicles = new List<Vehicle>();
            forests = new List<Forest>();

            GameData = new GameData();
            DataAccess = dataAccess;

            timer = new GameTimer();
            timer.Interval = 250;
            timer.Elapsed += new EventHandler(Timer_Elapsed);
            gameTime = 4;
            gameTimer = 0;
            timeStatus = TimeStatus.BASE;

            timer.Start();
        }

        public Field Board(int i, int j) 
        {
            return board[i, j];
        }

        public void BuyRoad(int x, int y) {

            if (board[x,y].fieldType is Grass)
            {
                board[x, y].fieldType = new Road();
                money -= 20;
                CheckBalance();
                OnFieldChanged(x, y, board[x,y].fieldType);
                OnMoneyChanged(money);
            }

        }
        public void BuyVehicle(VehicleType vehicleType, Product product) 
        {
            Vehicle vehicle;
            
            switch(vehicleType)
            {
                case VehicleType.FastCar:
                    vehicle = new FastCar(product);
                    break;

                case VehicleType.SlowCar:
                    vehicle = new SlowCar(product);
                    break;

                default:
                    throw new ArgumentException("Invalid vehicle purchase");
            }
            money -= vehicle.cost;
            CheckBalance();
            OnMoneyChanged(money);
            vehicles.Add(vehicle);
            OnVehicleBought(vehicle);

        }
        
        public void OnVehicleBought(Vehicle vehicle)
        {
            VehicleBought?.Invoke(this, vehicle); //ezzel kellene jelezni, hogy jelenjen meg a nem mozgó járművek listájában az autó
        }

        //public void BuyBridge(int x, int y) { } kell több bridge
        public void PutStop(int x, int y) {

            if (board[x,y].fieldType is Road && IsStopValid(x,y))
            {
                board[x,y].isStop = true;
                Stop stop = new Stop();
                stops.Add(stop);
            }
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
                    if (board[nx,ny].industry is not null)
                    {
                        return true;
                    }
                
                }
            }
            return false;
        }

        public void CheckBalance() 
        {
            if (money == 0)
            {
                OnGameOver(true);
            }
        
        } //ha lenullázódik a pénzünk akkor OnGameOver
        public void NewGame() { }
        public async Task LoadGame(String path) { }
        public async Task SaveGame(String path) { }

        public void Produce() { }
       
        public void OnGameOver(bool gameOver)
        {
            GameOver?.Invoke(this, gameOver);
        }

        public void OnMoneyChanged(int m)
        {
            MoneyChanged?.Invoke(this, m);
        }

        public void OnFieldChanged(int x, int y, FieldType type)
        {
            FieldChanged?.Invoke(this, new FieldChangedEventArgs(x,y,type));
        }

        private void OnTimerTick(int gameTimer)
        {
            TimerTick?.Invoke(this, gameTimer);
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


        private void Timer_Elapsed(Object? sender, EventArgs e)
        {

            gameTime--;
            if(gameTime == 0)
            {
                gameTimer++;
                OnTimerTick(gameTimer);
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

    }
}
