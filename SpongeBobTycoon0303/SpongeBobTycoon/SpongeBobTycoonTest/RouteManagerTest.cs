using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;

namespace SpongeBobTycoonTest;

[TestClass]
public class RouteManagerTest
{
    private DataAccess dataAccess;
    private GameModel model;
    private RouteManager routeManager;
    private Vehicle vehicleWood;

    public RouteManagerTest()
    {
        dataAccess = new DataAccess();
        model = new GameModel(dataAccess);
        vehicleWood = new Vehicle(new Wood());
        routeManager = new RouteManager(vehicleWood);
    }

    [TestMethod]
    public void TestAddStop()
    {
        Stop stop1 = new Stop(3,4, new Farm(new Wood()));
        Stop stop2 = new Stop(7, 2, new Factory(new Burger()));
        Stop stop3 = new Stop(45, 30, new Factory(new Petfood()));
        Stop stop4 = new Stop(34, 10, new Farm(new Algae()));

        Assert.AreEqual(routeManager.Stops.Count, 0);
        routeManager.AddStop(stop1);
        Assert.AreEqual(routeManager.Stops.Count, 1);
        routeManager.AddStop(stop2);
        Assert.AreEqual(routeManager.Stops.Count, 2);
        routeManager.AddStop(stop3);
        Assert.AreEqual(routeManager.Stops.Count, 3);
        routeManager.AddStop(stop4);
        Assert.AreEqual(routeManager.Stops.Count, 4);
    }

    [TestMethod]
    public void TestValidStop()
    {
        Stop stopWood = new Stop(3, 4, new Farm(new Wood()));
        Stop stopBurger = new Stop(7, 2, new Factory(new Burger()));
        Stop stopPetfood = new Stop(45, 30, new Factory(new Petfood()));
        Stop stopAlgae = new Stop(34, 10, new Farm(new Algae()));
        Stop stopCity1 = new Stop(20, 10, new Factory(new Citizen()));
        Stop stopCity2 = new Stop(20, 10, new Factory(new Citizen()));
        Stop stopNet = new Stop(1, 65, new Farm(new Net()));
        Stop stopMeat = new Stop(41, 31, new Farm(new Meat()));
        Stop stopSpatula = new Stop(30, 14, new Farm(new Spatula()));

        Vehicle vehiclePetfood = new Vehicle(new Petfood());
        Vehicle vehicleBurger = new Vehicle(new Burger());
        Vehicle vehicleNet = new Vehicle(new Net());
        Vehicle vehicleCitizen = new Vehicle(new Citizen());

        RouteManager routeManagerPetfood = new RouteManager(vehiclePetfood);
        RouteManager routeManagerBurger = new RouteManager(vehicleBurger);
        RouteManager routeManagerNet = new RouteManager(vehicleNet);
        RouteManager routeManagerCitizen = new RouteManager(vehicleCitizen);

        Assert.IsFalse(routeManagerCitizen.ValidRoute());
        routeManagerCitizen.AddStop(stopCity1);
        Assert.IsFalse(routeManagerCitizen.ValidRoute());
        routeManagerCitizen.AddStop(stopMeat);
        routeManagerCitizen.AddStop(stopBurger);
        Assert.IsFalse(routeManagerCitizen.ValidRoute());
        routeManagerCitizen.AddStop(stopCity2);
        Assert.IsTrue(routeManagerCitizen.ValidRoute());

        Assert.IsFalse(routeManagerBurger.ValidRoute());
        routeManagerBurger.AddStop(stopCity1);
        Assert.IsFalse(routeManagerBurger.ValidRoute());
        routeManagerBurger.AddStop(stopMeat);
        Assert.IsFalse(routeManagerBurger.ValidRoute());
        routeManagerBurger.AddStop(stopBurger);
        Assert.IsFalse(routeManagerBurger.ValidRoute());
        routeManagerBurger.AddStop(stopCity2);
        routeManagerBurger.AddStop(stopNet);
        Assert.IsFalse(routeManagerBurger.ValidRoute());
        routeManagerBurger.AddStop(stopSpatula);
        Assert.IsTrue(routeManagerBurger.ValidRoute());

        Assert.IsFalse(routeManagerNet.ValidRoute());
        routeManagerNet.AddStop(stopCity1);
        Assert.IsFalse(routeManagerNet.ValidRoute());
        routeManagerNet.AddStop(stopMeat);
        routeManagerNet.AddStop(stopBurger);
        Assert.IsFalse(routeManagerNet.ValidRoute());
        routeManagerNet.AddStop(stopNet);
        Assert.IsTrue(routeManagerNet.ValidRoute());

        Assert.IsFalse(routeManagerPetfood.ValidRoute());
        routeManagerPetfood.AddStop(stopCity1);
        Assert.IsFalse(routeManagerPetfood.ValidRoute());
        routeManagerPetfood.AddStop(stopPetfood);
        Assert.IsFalse(routeManagerPetfood.ValidRoute());
        routeManagerPetfood.AddStop(stopCity2);
        routeManagerPetfood.AddStop(stopNet);
        Assert.IsFalse(routeManagerPetfood.ValidRoute());
        routeManagerPetfood.AddStop(stopAlgae);
        Assert.IsTrue(routeManagerPetfood.ValidRoute());

        Assert.IsFalse(routeManager.ValidRoute());
        routeManager.AddStop(stopCity1);
        Assert.IsFalse(routeManager.ValidRoute());
        routeManager.AddStop(stopCity2);
        routeManager.AddStop(stopNet);
        Assert.IsFalse(routeManager.ValidRoute());
        routeManager.AddStop(stopWood);
        Assert.IsTrue(routeManager.ValidRoute());



    }
}
