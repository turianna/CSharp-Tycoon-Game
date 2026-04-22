using Microsoft.VisualStudio.TestPlatform.ObjectModel.Client.Payloads;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Persistence;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
namespace SpongeBobTycoonTest
{
    [TestClass]
    public class GameModelTest
    {
        private DataAccess dataAccess;
        private GameModel model;

        [TestInitialize]
        public void Setup()
        {
            dataAccess = new DataAccess();
            model = new GameModel(dataAccess);
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
            Assert.AreEqual(model.money, 10000);

            Assert.IsTrue(model.board[0, 0].fieldType is Grass);
            Assert.IsTrue(model.board[3, 10].fieldType is Grass);
            Assert.IsFalse(model.board[3, 10].isStop);
        }

        [TestMethod]
        public void TestBuyRoad()
        {
            model.BuyRoad(3, 10);
            Assert.IsTrue(model.board[3, 10].fieldType is Road);
            Assert.AreEqual(model.money,9980);
            model.BuyRoad(3, 11);
            Assert.IsTrue(model.board[3, 11].fieldType is Road);
            Assert.AreEqual(model.money,9960);
            model.BuyRoad(5, 6);
            Assert.IsTrue(model.board[5, 6].fieldType is Road);
            Assert.AreEqual(model.money,9940);
            model.BuyRoad(6, 6);
            Assert.IsTrue(model.board[6, 6].fieldType is Road);
            Assert.AreEqual(model.money,9920);


            model.BuyRoad(8, 0); // Folyó
            Assert.IsFalse(model.board[8, 0].fieldType is Road);
            Assert.AreEqual(model.money, 9920);
            model.BuyRoad(14, 3); //Ipar
            Assert.IsFalse(model.board[14, 3].fieldType is Road);
            Assert.AreEqual(model.money, 9920);
            model.BuyRoad(3, 41); // Tó
            Assert.IsFalse(model.board[3, 41].fieldType is Road);
            Assert.AreEqual(model.money, 9920);
            model.BuyRoad(2, 5); // Város
            Assert.IsFalse(model.board[2, 5].fieldType is Road);
            Assert.AreEqual(model.money, 9920);

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
            Assert.AreEqual(model.money, 9800);
            model.BuyVehicle(VehicleType.SlowCar, new Citizen());
            Assert.AreEqual(model.parkingVehicles.Count, 2);
            Assert.IsTrue(model.parkingVehicles.ElementAt(1).product is Citizen);
            Assert.IsTrue(model.parkingVehicles.ElementAt(1) is SlowCar);
            Assert.AreEqual(model.money, 9600);
            model.BuyVehicle(VehicleType.FastCar, new Net());
            Assert.AreEqual(model.parkingVehicles.Count, 3);
            Assert.IsTrue(model.parkingVehicles.ElementAt(2).product is Net);
            Assert.IsTrue(model.parkingVehicles.ElementAt(2) is FastCar);
            Assert.AreEqual(model.money, 9400);
            model.BuyVehicle(VehicleType.FastCar, new Petfood());
            Assert.AreEqual(model.parkingVehicles.Count, 4);
            Assert.IsTrue(model.parkingVehicles.ElementAt(3).product is Petfood);
            Assert.IsTrue(model.parkingVehicles.ElementAt(3) is FastCar);
            Assert.AreEqual(model.money, 9200);
        }


    }
}
