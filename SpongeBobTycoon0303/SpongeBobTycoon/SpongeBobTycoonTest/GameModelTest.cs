using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Persistence;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using System.Net.Http.Headers;
namespace SpongeBobTycoonTest
{
    [TestClass]
    public class GameModelTest
    {
        private DataAccess dataAccess;
        private GameModel model;

        public GameModelTest()
        {
            this.dataAccess = new DataAccess();
            this.model = new GameModel(dataAccess);
        }
        /*
        [TestInitialize]
        public void Setup()
        {
            dataAccess = new DataAccess();
            model = new GameModel(dataAccess);
        }*/

        [TestMethod]
        public void TestTreeGrow()
        {
            model.timer.Stop();

            Forest forest = (Forest)model.GetForests()[0].fieldType;

            int initialForestCount = model.GetForests().Count;

            Assert.IsTrue(forest.treeCount == 1);
            Assert.IsFalse(forest.readyToSpread);

            for (int i = 0; i < 4*119; i++)
            {
                model.SimulateTimerElapsed();
            }

            Assert.IsFalse(forest.treeCount == 2);
            Assert.IsFalse(forest.readyToSpread);

            for (int i = 0; i < 4 * 62; i++)
            {
                model.SimulateTimerElapsed();
            }

            Assert.IsTrue(forest.treeCount == 2);

            for(int i = 0; i < 4 * 400; i++)
            {
                model.SimulateTimerElapsed();
            }

            Assert.IsTrue(model.GetForests().Count > initialForestCount);
            bool newForestGrew = model.board[33, 2].fieldType is Forest || model.board[32, 3].fieldType is Forest;
            Assert.IsTrue(newForestGrew);

        }

        [TestMethod]

        public void TestFarms()
        {
            Farm woodFarm = new Farm(new Wood());
            Farm algaeFarm = new Farm(new Algae());
            Farm meatFarm = new Farm(new Meat());
            int oldProductionTime = woodFarm.getProductionTime;

            Assert.IsTrue(woodFarm.product is Wood);
            Assert.IsTrue(algaeFarm.product is Algae);
            Assert.IsTrue(meatFarm.product is Meat);

            Assert.IsTrue(woodFarm.productAmount == 0);
            Assert.IsTrue(algaeFarm.productAmount == 0);
            Assert.IsTrue(meatFarm.productAmount == 0);

            for(int i = 0; i < 26; i++)
            {
                woodFarm.Produce();
                algaeFarm.Produce();
                meatFarm.Produce();
            }

            Assert.IsTrue(woodFarm.productAmount == 5);
            Assert.IsTrue(algaeFarm.productAmount == 5);
            Assert.IsTrue(meatFarm.productAmount == 4);
            int newProductionTime = woodFarm.getProductionTime;

            Assert.IsTrue(Math.Abs(oldProductionTime - newProductionTime) <= 1);

            for(int i = 0; i < 125; i++)
            {
                meatFarm.Produce();
            }

            Assert.IsFalse(meatFarm.productAmount > 20);
        }

        [TestMethod]

        public void TestFactories()
        {
            Assert.ThrowsException<Exception>(() => new Factory(new Wood()));
            Factory burgerFactory = new Factory(new Burger());
            Factory petFoodFactory = new Factory(new Petfood());

            Assert.IsTrue(burgerFactory.getIngredients.Count == 2);
            Assert.IsTrue(petFoodFactory.getIngredients.Count == 1);

            for(int i = 0; i < 25; i++)
            {
                burgerFactory.Produce();
                petFoodFactory.Produce();
            }

            Assert.IsTrue(burgerFactory.productAmount == 0);
            Assert.IsTrue(petFoodFactory.productAmount == 0);

            burgerFactory.ReceiveIngredient(new Meat(), 8);
            burgerFactory.ReceiveIngredient(new Spatula(), 1);
            petFoodFactory.ReceiveIngredient(new Algae(), 5);

            for (int i = 0; i < 26; i++)
            {
                burgerFactory.Produce();
                petFoodFactory.Produce();
            }

            Assert.IsTrue(burgerFactory.productAmount == 1);
            Assert.IsTrue(petFoodFactory.productAmount == 1);

        }

        [TestMethod]

        public void TestInitializeBoard()
        {
            model.InitializeBoard();
            for(int i = 0; i<50; i++)
            {
                for(int j = 0; j<100; j++)
                {
                    Assert.IsNotNull(model.board[i, j]);
                }
            }
           
        }
        [TestMethod]
        public void TestStartNewGame()
        {
           
            model.BuyVehicle(VehicleType.SlowCar, new Burger());
            Assert.AreEqual(model.parkingVehicles.Count, 1);
            model.BuyRoad(3, 10);
            Assert.IsTrue(model.board[3, 10].fieldType is Road);
            model.BuyRoad(3, 11);
            Assert.IsTrue(model.board[3, 11].fieldType is Road);
            model.PutStop(3, 10);
            Assert.IsTrue(model.board[3, 10].isStop);
            Assert.AreEqual(model.stops.Count, 1);

            model.StartNewGame();

            Assert.AreEqual(model.stops.Count, 0);
            Assert.AreEqual(model.parkingVehicles.Count, 0);
            Assert.AreEqual(model.movingVehicles.Count, 0);
            Assert.AreEqual(model.gameTime, 4);
            Assert.AreEqual(model.gameTimer, 0);
            Assert.AreEqual(model.timeStatus, TimeStatus.BASE);
            Assert.AreEqual(model.money, 5000);

            Assert.IsTrue(model.board[0, 0].fieldType is Grass);
            Assert.IsTrue(model.board[3, 10].fieldType is Grass);
            Assert.IsFalse(model.board[3, 10].isStop);
        }

        [TestMethod]
        public void TestBuyRoad()
        {
            model.StartNewGame();
            model.BuyRoad(3, 10);
            Assert.IsTrue(model.board[3, 10].fieldType is Road);
            Assert.AreEqual(model.money,4900);
            model.BuyRoad(3, 11);
            Assert.IsTrue(model.board[3, 11].fieldType is Road);
            Assert.AreEqual(model.money,4800);
            model.BuyRoad(5, 6);
            Assert.IsTrue(model.board[5, 6].fieldType is Road);
            Assert.AreEqual(model.money,4700);
            model.BuyRoad(6, 6);
            Assert.IsTrue(model.board[6, 6].fieldType is Road);
            Assert.AreEqual(model.money,4600);


            model.BuyRoad(8, 0); // Folyó
            Assert.IsFalse(model.board[8, 0].fieldType is Road);
            Assert.AreEqual(model.money, 4600);
            model.BuyRoad(14, 3); //Ipar
            Assert.IsFalse(model.board[14, 3].fieldType is Road);
            Assert.AreEqual(model.money, 4600);
            model.BuyRoad(3, 41); // Tó
            Assert.IsFalse(model.board[3, 41].fieldType is Road);
            Assert.AreEqual(model.money, 4600);
            model.BuyRoad(2, 5); // Város
            Assert.IsFalse(model.board[2, 5].fieldType is Road);
            Assert.AreEqual(model.money, 4600);

            model.BuyRoad(9,27);
            Assert.IsTrue(model.board[9, 27].fieldType is Road);
            model.BuyRoad(8, 27);
            model.BuyRoad(7, 27);
            model.BuyRoad(6, 27);
            model.BuyRoad(5, 27);
            model.BuyRoad(4, 27);
            model.BuyRoad(4, 28);
            model.BuyRoad(4, 29);
            model.BuyRoad(4, 30);
            Assert.IsTrue(model.board[9, 27].fieldType is Road); // erdő
            Assert.AreEqual(model.money, 3500);
            /*
            model.BuyRoad(2, 31); // erdő
            Assert.IsTrue(model.board[2, 31].fieldType is Road);
            Assert.AreEqual(model.money,9860);*/
        }

        [TestMethod]
        public void TestBuyVehicle()
        {
            model.StartNewGame();
            model.BuyVehicle(VehicleType.SlowCar, new Burger());
            Assert.AreEqual(model.parkingVehicles.Count, 1);
            Assert.IsTrue(model.parkingVehicles.ElementAt(0).product is Burger);
            Assert.IsTrue(model.parkingVehicles.ElementAt(0) is SlowCar);
            Assert.AreEqual(model.money, 4500);
            model.BuyVehicle(VehicleType.SlowCar, new Citizen());
            Assert.AreEqual(model.parkingVehicles.Count, 2);
            Assert.IsTrue(model.parkingVehicles.ElementAt(1).product is Citizen);
            Assert.IsTrue(model.parkingVehicles.ElementAt(1) is SlowCar);
            Assert.AreEqual(model.money, 4000);
            model.BuyVehicle(VehicleType.FastCar, new Net());
            Assert.AreEqual(model.parkingVehicles.Count, 3);
            Assert.IsTrue(model.parkingVehicles.ElementAt(2).product is Net);
            Assert.IsTrue(model.parkingVehicles.ElementAt(2) is FastCar);
            Assert.AreEqual(model.money, 3500);
            model.BuyVehicle(VehicleType.FastCar, new Petfood());
            Assert.AreEqual(model.parkingVehicles.Count, 4);
            Assert.IsTrue(model.parkingVehicles.ElementAt(3).product is Petfood);
            Assert.IsTrue(model.parkingVehicles.ElementAt(3) is FastCar);
            Assert.AreEqual(model.money, 3000);
        }

        [TestMethod]
        public void TestBuyBridge()
        {
            //folyón tesztelés
            model.StartNewGame();
            model.BuyRoad(5, 6);
            Assert.AreEqual(model.Board(6, 6).Z, 3);
            model.Terraform(6, 6, false);
            Assert.AreEqual(model.Board(6, 6).Z, 2);
            model.BuyRoad(6, 6);
            model.Terraform(6, 7, false);
            model.Terraform(6, 7, false);
            Assert.AreEqual(model.Board(6, 7).Z, 1);
            model.BuyRoad(6, 7);
            model.BuyBridge(7, 7, false, false);
            Assert.IsFalse(model.board[7, 7].fieldType is Bridge);
            Assert.AreEqual(model.money, 3950);
            model.BuyBridge(7, 7, true, false);
            Assert.IsFalse(model.board[7, 7].fieldType is Bridge);
            Assert.AreEqual(model.money, 3950);

            model.BuyBridge(7, 7, false, true);
            Assert.IsTrue(model.board[7, 7].fieldType is SlowBridge);
            Assert.AreEqual(model.money, 3750);

            model.BuyBridge(8, 7, true, true);
            Assert.IsTrue(model.board[8, 7].fieldType is FastBridge);
            Assert.AreEqual(model.money, 3350);

            //tavon tesztelés
            model.BuyRoad(9, 27);
            model.BuyRoad(9, 28);
            model.BuyRoad(9, 29);
            model.BuyRoad(9, 30);
            model.BuyRoad(9, 31);
            model.BuyRoad(9, 32);
            model.BuyRoad(9, 33);
            model.BuyRoad(9, 34);
            model.BuyRoad(9, 35);
            model.BuyRoad(9, 36);
            model.BuyRoad(9, 37);
            model.BuyRoad(9, 38);

            model.BuyBridge(9, 39, false, true);
            Assert.IsFalse(model.board[9, 39].fieldType is Bridge);
            Assert.AreEqual(model.money, 2150);
            model.BuyBridge(9, 39, true, true);
            Assert.IsFalse(model.board[9, 39].fieldType is Bridge);
            Assert.AreEqual(model.money, 2150);

            model.BuyBridge(9, 39, false, false);
            Assert.IsTrue(model.board[9, 39].fieldType is SlowBridge);
            Assert.AreEqual(model.money, 1950);

            model.BuyBridge(9, 40, true, false);
            Assert.IsTrue(model.board[9, 40].fieldType is FastBridge);
            Assert.AreEqual(model.money, 1550);
        }
        [TestMethod]
        public void TestIsStopValid()
        {
            model.StartNewGame();
            model.BuyRoad(13, 22);
            model.BuyRoad(13, 21);
            model.BuyRoad(13, 20);
            model.BuyRoad(13, 19);
            model.BuyRoad(13, 18);
            model.BuyRoad(13, 17);
            model.BuyRoad(13, 16);
            model.BuyRoad(13, 15);
            model.BuyRoad(13, 14);
            model.BuyRoad(13, 13);
            model.BuyRoad(13, 12);
            model.BuyBridge(13, 11,true,true);
            model.BuyBridge(13, 10, true, true);
            model.BuyRoad(13, 9);
            model.BuyRoad(13, 8);
            model.BuyRoad(13, 7);
            Assert.IsTrue(model.board[13, 22].fieldType is Road);
            Assert.IsTrue(model.board[13, 7].fieldType is Road);

            //Homokon
            Assert.IsFalse(model.IsStopValid(0,0).Item1);
            //Lavan
            Assert.IsFalse(model.IsStopValid(7, 0).Item1);
            //Erdőn
            Assert.IsFalse(model.IsStopValid(9, 27).Item1);
            //Varoson
            Assert.IsFalse(model.IsStopValid(12, 21).Item1);
            //Iparon
            Assert.IsFalse(model.IsStopValid(13, 6).Item1);
            //Olyan úton ami mellett nincs város vagy ipar
            Assert.IsFalse(model.IsStopValid(13, 18).Item1);

            Assert.IsTrue(model.IsStopValid(13, 22).Item1);
            Assert.IsTrue(model.IsStopValid(13, 7).Item1);
        }

        [TestMethod]
        public void TestPutStop()
        {
            model.BuyRoad(13, 22);
            model.BuyRoad(13, 21);
            model.BuyRoad(13, 20);
            model.BuyRoad(13, 19);
            model.BuyRoad(13, 18);
            model.BuyRoad(13, 17);
            model.BuyRoad(13, 16);
            model.BuyRoad(13, 15);
            model.BuyRoad(13, 14);
            model.BuyRoad(13, 13);
            model.BuyRoad(13, 12);
            model.BuyBridge(13, 11, true, true);
            model.BuyBridge(13, 10, true, true);
            model.BuyRoad(13, 9);
            model.BuyRoad(13, 8);
            model.BuyRoad(13, 7);

            model.PutStop(0, 0);
            Assert.IsFalse(model.Board(0, 0).isStop);
            model.PutStop(7, 0);
            Assert.IsFalse(model.Board(7, 0).isStop);
            model.PutStop(9, 27);
            Assert.IsFalse(model.Board(9, 27).isStop);
            model.PutStop(12, 21);
            Assert.IsFalse(model.Board(12, 21).isStop);
            model.PutStop(13, 6);
            Assert.IsFalse(model.Board(13, 6).isStop);
            model.PutStop(13, 18);
            Assert.IsFalse(model.Board(13, 18).isStop);

            Assert.IsTrue(model.board[13, 22].fieldType is Road);
            Assert.IsTrue(model.board[13, 7].fieldType is Road);
            model.PutStop(13, 22);
            Assert.IsTrue(model.Board(13, 22).isStop);
            model.PutStop(13, 7);
            Assert.IsTrue(model.Board(13, 7).isStop);

        }


        [TestMethod]
        public void TestSelectStop()
        {
            model.BuyRoad(13, 22);
            model.BuyRoad(13, 21);
            model.BuyRoad(13, 20);
            model.BuyRoad(13, 19);
            model.BuyRoad(13, 18);
            model.BuyRoad(13, 17);
            model.BuyRoad(13, 16);
            model.BuyRoad(13, 15);
            model.BuyRoad(13, 14);
            model.BuyRoad(13, 13);
            model.BuyRoad(13, 12);
            model.BuyBridge(13, 11, true, true);
            model.BuyBridge(13, 10, true, true);
            model.BuyRoad(13, 9);
            model.BuyRoad(13, 8);
            model.BuyRoad(13, 7);

            Assert.IsTrue(model.board[13, 22].fieldType is Road);
            Assert.IsTrue(model.board[13, 7].fieldType is Road);
            model.PutStop(13, 22);
            Assert.IsTrue(model.Board(13, 22).isStop);
            model.PutStop(13, 7);
            Assert.IsTrue(model.Board(13, 7).isStop);

            model.SelectStop(0, 0);
            model.SelectStop(0, 1);
            Assert.AreEqual(model.StopsForRoute.Count, 0);
            model.SelectStop(13,22);
            Assert.AreEqual(model.StopsForRoute.Count, 1);
            model.SelectStop(13, 7);
            Assert.AreEqual(model.StopsForRoute.Count, 2);
        }


        [TestMethod]
        public void TestRemoveFromRoute()
        {
            model.StartNewGame();
            Vehicle vehicle1 = new SlowCar(new Burger());
            Vehicle vehicle2 = new SlowCar(new Citizen());
            Vehicle vehicle3 = new FastCar(new Net());
            
            model.movingVehicles.Add(vehicle1);
            model.movingVehicles.Add(vehicle2);
            model.movingVehicles.Add(vehicle3);
            Assert.AreEqual(model.movingVehicles.Count, 3);
            Assert.AreEqual(model.parkingVehicles.Count, 0);

            model.RemoveVehicleFromRoute(vehicle1);
            Assert.AreEqual(model.movingVehicles.Count, 2);
            Assert.AreEqual(model.parkingVehicles.Count, 1);

            model.RemoveVehicleFromRoute(vehicle2);
            Assert.AreEqual(model.movingVehicles.Count, 1);
            Assert.AreEqual(model.parkingVehicles.Count, 2);

            model.RemoveVehicleFromRoute(vehicle3);
            Assert.AreEqual(model.movingVehicles.Count, 0);
            Assert.AreEqual(model.parkingVehicles.Count, 3);

        }

        [TestMethod]
        public void TestChangeBalance()
        {
            model.StartNewGame();
            Assert.AreEqual(model.money, 5000);
            model.ChangeBalance(100);
            Assert.AreEqual(model.money, 5100);
            model.ChangeBalance(-200);
            Assert.AreEqual(model.money, 4900);
            model.ChangeBalance(-250);
            Assert.AreEqual(model.money, 4650);
            model.ChangeBalance(150);
            Assert.AreEqual(model.money, 4800);
        }


        [TestMethod]

        public void TestCreateRoute()
        {
            //Correect Route
            model.StartNewGame();

            model.BuyVehicle(VehicleType.FastCar, new Citizen());
            Vehicle v = model.parkingVehicles[0];
            
            model.BuyRoad(3, 10);
            model.BuyRoad(3, 11);
            model.BuyRoad(3, 12);
            model.BuyRoad(3, 13);
            model.BuyRoad(3, 14);
            model.BuyRoad(3, 15);
            model.BuyRoad(3, 16);
            model.BuyRoad(4, 16);
            model.BuyRoad(5, 16);
            model.BuyRoad(6, 16);
            model.BuyRoad(7, 16);
            model.BuyRoad(8, 16);
            model.BuyRoad(9, 16);
            model.BuyRoad(10, 16);
            model.BuyRoad(11, 16);
            model.BuyRoad(12, 16);
            model.BuyRoad(13, 16);
            model.BuyRoad(13, 17);
            model.BuyRoad(13, 18);
            model.BuyRoad(13, 19);
            model.BuyRoad(13, 20);
            model.BuyRoad(13, 21);
            model.BuyRoad(13, 22);

            model.PutStop(3, 10);
            model.PutStop(13, 21);

            model.SelectStop(3, 10);
            model.SelectStop(13, 21);


            //No chosen vehicle
            model.CreateRoute();
            Assert.AreEqual(model.StopsForRoute.Count, 2);
            Assert.AreEqual(model.parkingVehicles.Count, 1);
            Assert.AreEqual(model.movingVehicles.Count, 0);
            Assert.IsFalse(model.board[v.x, v.y].isVehicle);

            //Correct Route
            bool addMovingVehicleEventWasFired = false;
            model.AddMovingVehicle += (sender, e) =>
            {
                addMovingVehicleEventWasFired = true;
            };

            bool carSteppedWasFired = false;
            model.CarStepped += (sender, e) =>
            {
                carSteppedWasFired = true;
            };

            model.ChosenVehicle(v);
            model.CreateRoute();
            Assert.IsTrue(addMovingVehicleEventWasFired);
            Assert.IsTrue(carSteppedWasFired);
            Assert.AreEqual(model.StopsForRoute.Count, 0);
            Assert.AreEqual(model.parkingVehicles.Count, 0);
            Assert.AreEqual(model.movingVehicles.Count, 1);
            Assert.IsTrue(model.board[v.x, v.y].isVehicle);

            //Invalid Route
            model.StartNewGame();

            model.BuyVehicle(VehicleType.FastCar, new Citizen());
            Vehicle v2 = model.parkingVehicles[0];

            model.BuyRoad(3, 10);
            model.BuyRoad(3, 11);
            model.BuyRoad(3, 12);
            model.BuyRoad(3, 13);
            model.BuyRoad(3, 14);
            model.BuyRoad(3, 15);
            model.BuyRoad(3, 16);
            model.BuyRoad(4, 16);
            model.BuyRoad(5, 16);
            model.BuyRoad(6, 16);

            model.BuyRoad(9, 16);
            model.BuyRoad(10, 16);
            model.BuyRoad(11, 16);
            model.BuyRoad(12, 16);
            model.BuyRoad(13, 16);
            model.BuyRoad(13, 17);
            model.BuyRoad(13, 18);
            model.BuyRoad(13, 19);
            model.BuyRoad(13, 20);
            model.BuyRoad(13, 21);
            model.BuyRoad(13, 22);

            model.PutStop(3, 10);
            model.PutStop(13, 21);

            model.SelectStop(3, 10);
            model.SelectStop(13, 21);

            bool eventWasFired = false;

            model.CreateRouteFailed += (sender,e) =>
            {
                eventWasFired = true;
            };
            model.CreateRoute();
            Assert.IsTrue(eventWasFired);
            Assert.AreEqual(model.StopsForRoute.Count, 0);
            Assert.AreEqual(model.parkingVehicles.Count, 1);
            Assert.AreEqual(model.movingVehicles.Count, 0);
            Assert.IsFalse(model.board[v.x, v.y].isVehicle);
        }

        
        [TestMethod]

        public void TestCarStep()
        {
            model.StartNewGame();

            model.BuyVehicle(VehicleType.FastCar, new Citizen());

            model.BuyRoad(3, 10);
            model.BuyRoad(3, 11);
            model.BuyRoad(3, 12);
            model.BuyRoad(3, 13);
            model.BuyRoad(3, 14);
            model.BuyRoad(3, 15);
            model.BuyRoad(3, 16);
            model.BuyRoad(4, 16);
            model.BuyRoad(5, 16);
            model.BuyRoad(6, 16);
            model.BuyRoad(7, 16);
            model.BuyRoad(8, 16);
            model.BuyRoad(9, 16);
            model.BuyRoad(10, 16);
            model.BuyRoad(11, 16);
            model.BuyRoad(12, 16);
            model.BuyRoad(13, 16);
            model.BuyRoad(13, 17);
            model.BuyRoad(13, 18);
            model.BuyRoad(13, 19);
            model.BuyRoad(13, 20);
            model.BuyRoad(13, 21);
            model.BuyRoad(13, 22);

            model.PutStop(3, 10);
            model.PutStop(13, 21);
            Assert.IsTrue(model.board[3, 10].isStop);
            Assert.IsTrue(model.board[13, 21].isStop);

            model.SelectStop(3, 10);
            model.SelectStop(13, 21);

            Vehicle v1 = model.parkingVehicles[0];
            model.ChosenVehicle(v1);
            model.CreateRoute();

            Vehicle v = model.movingVehicles[0];
            Assert.AreNotEqual(v.Route.Count, 0);

            bool carSteppedWasFired = false;
            model.CarStepped += (sender, e) =>
            {
                carSteppedWasFired = true;
            };
            model.CarStep(v);
            
            Assert.IsTrue(model.board[3 ,12].isVehicle);
            Assert.AreEqual(v.x, 3);
            Assert.AreEqual(v.y, 12);
            Assert.AreEqual(v.route_index, 1);
            Assert.IsTrue(carSteppedWasFired);

            
            //Moving Uphill
            int initialTime = v.timeUntilMoves;
            model.board[3, 14].Z += 1;
            model.CarStep(v);
            Assert.AreEqual(initialTime + 1, v.timeUntilMoves);
            

            //Two cars
            model.board[3, 14].isVehicle = true;
            model.CarStep(v);
            Assert.IsTrue(model.board[3, 14].areTwoVehicles);
            model.CarStep(v);
            Assert.IsTrue(model.board[3, 14].isVehicle);
            Assert.IsFalse(model.board[3, 14].areTwoVehicles);
            Assert.IsTrue(model.board[3, 15].isVehicle);

            
            //City stop reached
            int initialBalance = model.money -2000;
            bool produceFired = false;
            model.Produce += (sender, e) =>
            {
                produceFired = true;
            };
            while (!(v.x == 13 && v.y == 21))
            {
                model.CarStep(v);
            }
            Assert.IsTrue(model.board[13, 21].isVehicle);
            while(!(v.x == 3 && v.y == 10))
            {
                model.CarStep(v);
            }
            
            Assert.IsTrue(model.board[3, 10].isVehicle);
            Assert.IsTrue(model.money > initialBalance);
            Assert.IsTrue(produceFired);

            //Breaking car
            initialBalance = model.money;
            v.brokness = 1;
            model.CarStep(v);
            Assert.IsTrue(initialBalance > model.money);

        }
    }
}
