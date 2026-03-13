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

        public event EventHandler? GameOver;
        public event EventHandler<int>? MoneyChanged;
        public event EventHandler<(int,int)>? FieldChanged;

        public event EventHandler<int>? TimerTick;
        public ITimer timer;
        public int fastGameTime;
        public int gameTime;


        public Field[,] board;

        public GameModel()
        {
            stops = new List<Stop>();
            vehicles = new List<Vehicle>();
            movingVehicles = new List<Vehicle>();
            forests = new List<Forest>();

            GameData = new GameData();
            DataAccess = new DataAccess();

            board = DataAccess.LoadMap();

            timer = new GameTimer();
            timer.Interval = 250;
            timer.Elapsed += new EventHandler(Timer_Elapsed);
            fastGameTime = 2;
            gameTime = 4;

            timer.Start();
        }

        public Field Board(int i, int j) 
        {
            return board[i, j];
        }

        public void BuyRoad(int x, int y) { }
        public void BuyVehicle() { }
        public void BuyBridge(int x, int y) { }
        public void BuyStop(int x, int y) { }

        public void CheckBalance() { } //ha lenullázódik a pénzünk akkor OnGameOver
        public void NewGame() { }
        public async Task LoadGame(String path) { }
        public async Task SaveGame(String path) { }

        public void Produce() { }
       
        public void OnGameOver()
        {
            //GameOver?.Invoke();
        }

        public void OnMoneyChanged(int m)
        {
            MoneyChanged?.Invoke(this, m);
        }

        public void OnFieldChanged((int,int) coord)
        {
            FieldChanged?.Invoke(this, coord);
        }

        private void OnTimerTick(int gameTime)
        {
            TimerTick?.Invoke(this, gameTime);
        }

        private void Timer_Elapsed(Object? sender, EventArgs e)
        {
            

            /*foreach (Vehicle car in vehicles) {
                car.stepsUntilBroken--;
                if (car.stepsUntilBroken == 0)
                {
                    money -= 100;
                    car.stepsUntilBroken = 50000;
                }
            } ez majd a vehicle step-jébe kerül*/

        }

    }
}
