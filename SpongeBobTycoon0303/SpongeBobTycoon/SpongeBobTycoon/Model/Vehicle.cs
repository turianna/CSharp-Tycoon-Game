using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using SpongeBobTycoon.Persistence;
using System;
//using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class Vehicle
    {
        private List<(int, int)> _route = new();
        public IReadOnlyList<(int, int)> Route => _route.AsReadOnly();
        private List<Product> _ingredients = new();
        public IReadOnlyList<Product> Ingredients => _ingredients.AsReadOnly();
        private List<int> _ingredientAmounts = new();
        public IReadOnlyList<int> IngredientAmounts => _ingredientAmounts.AsReadOnly();
        private List<int> _ingredientCapacities = new();
        public IReadOnlyList<int> IngredientCapacities => _ingredientCapacities.AsReadOnly();

        public Product product { get; set; }
        public string productName { get; private set; }
        public string typeName { get; private set; }
        public int productAmount { get; private set; }
        public int capacity { get; set; }


        public int ID { get; set; }
        public int route_index { get; set; }

        public Direction direction { get; set; }
        public int dir { get; set; }
        public int time { get; protected set; }
        protected int baseTime;
        public int getBaseTime => baseTime;
        public int timeUntilMoves { get; set; }
        public int stepsUntilBroken { get; protected set; }
        public int repairCost { get; protected set; }

        public int cost { get; protected set; }

        public int x;
        public int y;
        public int brokness;


        //public (int,int) startPoint = path[0]
        public Vehicle(Product product)
        {
            this.product = product;
            productAmount = 0;
            this._route = new List<(int, int)>();
            productName = "";
            typeName = "";
            switch (product)
            {
                case Petfood:
                    _ingredients = new List<Product>();
                    _ingredientAmounts = new List<int>();
                    _ingredientCapacities = new List<int>();

                    _ingredients.Add(new Algae());
                    _ingredientAmounts.Add(0);
                    _ingredientCapacities.Add(10);
                    break;

                case Burger:
                    _ingredients = new List<Product>();
                    _ingredientAmounts = new List<int>();
                    _ingredientCapacities = new List<int>();

                    _ingredients.Add(new Spatula());
                    _ingredients.Add(new Meat());

                    _ingredientAmounts.Add(0);
                    _ingredientAmounts.Add(0);

                    _ingredientCapacities.Add(5);
                    _ingredientCapacities.Add(20);
                    break;
                case Net:
                    break;
                case Wood:
                    break;
                case Citizen:
                    break;
                default:
                    throw new Exception("Invalid product for vehicle");
            }

            switch (product)
            {
                case Algae:
                    productName = "Algee";
                    break;
                case Burger:
                    productName = "Burger";
                    break;
                case Citizen:
                    productName = "Citizen";
                    break;
                case Meat:
                    productName = "Meat";
                    break;
                case Net:
                    productName = "Net";
                    break;
                case Petfood:
                    productName = "Petfood";
                    break;
                case Spatula:
                    productName = "Spatula";
                    break;
                case Wood:
                    productName = "Wood";
                    break;
            }


        }

        public Vehicle(VehicleData vehicleData)
        {
            _route = new List<(int, int)>();
            foreach (Point p in vehicleData.route)
            {
                _route.Add((p.X, p.Y));
            }

            _ingredients = new List<Product>();
            foreach(ProductEnum pe in vehicleData.ingredients)
            {
                _ingredients.Add(ProductEnumToProduct(pe));
            }
            _ingredientAmounts = vehicleData.ingredientAmounts;
            _ingredientCapacities = vehicleData.ingredientCapacities;
            product = ProductEnumToProduct(vehicleData.product);
            productName = vehicleData.productName;
            typeName = vehicleData.typeName;
            productAmount = vehicleData.productAmount;
            capacity = vehicleData.capacity;
            ID = vehicleData.ID;
            route_index = vehicleData.route_index;
            direction = vehicleData.direction;
            dir = vehicleData.dir;
            time = vehicleData.time;
            baseTime = vehicleData.baseTime;
            timeUntilMoves = vehicleData.timeUntilMoves;
            stepsUntilBroken = vehicleData.stepsUntilBroken;
            repairCost = vehicleData.repairCost;
            cost = vehicleData.cost;
            x = vehicleData.x;
            y = vehicleData.y;
            brokness = vehicleData.brokness;
        }

        public void AssignRoute(IEnumerable<(int, int)> routeFinal)
        {
            _route.AddRange(routeFinal);
        }

        public void ClearRoute()
        {
            _route.Clear();
        }
        /// <summary>
        /// Transaction
        /// Lebonyolítja az áruforgalmat a jármű és az aktuális megálló (currentStop) között. A metódus két fő fázisból áll: először ellenőrzi, hogy a megálló termeli-e a jármű fő profiljába tartozó terméket, és ha igen, a szabad kapacitás erejéig felrakodja azt, majd (összetett termékek esetén) leadja a szállított alapanyagokat a megálló gyárának. A második fázisban a jármű végigiterál a saját alapanyag-listáján (_ingredients): ha a megálló olyan alapanyagot kínál, amelyből a járműnek még van szabad raktárhelye, elvégzi a vételezést és frissíti a készleteket. A folyamat minden lépését részletes debug üzenetekkel naplózza.
        /// </summary>
        public void Transaction(Stop currentStop)
        {
            int stopAmount = currentStop.industry.productAmount;
            Product stopProduct = currentStop.industry.product;

            //ha a megallonal a jarmu productjat lehet felvenni
            if (this.product.GetType() == stopProduct.GetType())
            {
                int productToGet = Math.Min(capacity - productAmount, stopAmount);

                productAmount += productToGet;
                currentStop.industry.productAmount -= productToGet;

                System.Diagnostics.Debug.WriteLine($"Vehicle {ID}, carrying {product.GetType().Name}: picked up product, new amount is {productAmount}/{capacity}");
                System.Diagnostics.Debug.WriteLine($"Stop with industry: {currentStop.industry.GetType().Name} gave {productToGet} of {stopProduct.GetType().Name} product amount left: {currentStop.industry.productAmount}");

                //ha a megalloban nem lehet leadni ingredientet (burgernel vagy petfoodnal), akkor kilepunk a metodusbol
                if (!(currentStop.industry.product is Burger || currentStop.industry.product is Petfood)) return;

                //ingredientek leadasa
                for (int i = 0; i < _ingredients!.Count; i++)
                {
                    Factory stopFactory = (Factory)currentStop.industry;
                    stopFactory.ReceiveIngredient(_ingredients[i], _ingredientAmounts[i]);
                }
            }

            //ha a jarmu nem szallit ingredientet, akkor vegzett a metodus
            if (_ingredients is null) return;

            //ha a megallonal a jarmu ingredientjet lehet felvenni
            foreach (Product ingredient in _ingredients)
            {
                if (ingredient.GetType() == stopProduct.GetType())
                {
                    int index = _ingredients.IndexOf(ingredient);
                    int ingredientAmount = _ingredientAmounts![index];
                    int ingredientCapacity = _ingredientCapacities![index];

                    int productToGive = Math.Min(ingredientCapacity - ingredientAmount, stopAmount);

                    _ingredientAmounts[index] += productToGive;
                    currentStop.industry.productAmount -= productToGive;

                    System.Diagnostics.Debug.WriteLine($"Vehicle with id {ID} received {productToGive} of {_ingredients[index].GetType().Name}. New amount: {_ingredientAmounts[index]}/{_ingredientCapacities[index]}");
                }
            }
        }

        public int CityStopReached()
        {
            //ha a jarmu egy varosba erkezik, akkor a benne levo termeket eladja, es a penzt megkapja
            int moneyEarned = productAmount * product.value;

            System.Diagnostics.Debug.WriteLine($"Vehicle {ID} sold {productAmount} of {product.GetType().Name}, making {moneyEarned} dollars.");
            productAmount = 0;
            return moneyEarned;
        }

        /// <summary>
        /// CanVehicleMove
        /// Ellenőrzi, hogy a jármű megkezdheti-e a mozgást a következő útvonalpont felé, figyelembe véve a forgalmi akadályokat és az időzítést. A metódus először meghatározza a következő koordinátákat és a haladási irányt, majd megvizsgálja, tartózkodik-e másik jármű a célmezőn. Amennyiben egy másik jármű blokkolja az utat (és az nem szemből érkezik), a metódus várakozásra kényszeríti az aktuális járművet a várakozási idő (timeUntilMoves) növelésével. Minden híváskor csökkenti a mozgásig hátralévő időt; ha ez eléri a nullát, visszaállítja az alapértelmezett várakozási időt (baseTime) és igaz (true) értékkel jelzi, hogy a jármű ténylegesen előreléphet, egyébként hamis (false) értékkel tér vissza.
        /// </summary>
        public bool CanVehicleMove(Field[,] board, List<Vehicle> allVehicles)
        {
            int nextX = _route![(route_index + 1) % (_route.Count - 1)].Item1;
            int nextY = _route[(route_index + 1) % (_route.Count - 1)].Item2;
            this.direction = DecideDirection(nextX, nextY, this.x, this.y);

            Vehicle? vehicleInTheWay = null;
            if (board[nextX, nextY].isVehicle)
            {
                foreach (Vehicle v in allVehicles)
                {
                    if (v == this) continue;
                    if (v.x == nextX && v.y == nextY) vehicleInTheWay = v;
                }

                if (vehicleInTheWay != null && vehicleInTheWay.direction != InverseDirection(this.direction))
                {
                    System.Diagnostics.Debug.WriteLine($"Vehicle {ID} is blocked");
                    timeUntilMoves++;
                }

            }

            timeUntilMoves--;

            if (timeUntilMoves == 0)
            {
                timeUntilMoves = baseTime;
                //stepsUntilBroken--;

                return true;
            }

            return false;
        }

        private Direction InverseDirection(Direction dir)
        {
            switch (dir)
            {
                case Direction.UP:
                    return Direction.DOWN;
                case Direction.DOWN:
                    return Direction.UP;
                case Direction.LEFT:
                    return Direction.RIGHT;
                case Direction.RIGHT:
                    return Direction.LEFT;
                default:
                    throw new Exception("Hiba: az emberiseg nem tudta hogy valojaban letezik egy otodik irany");
            }
        }

        /// <summary>
        /// DecideDirection
        /// Meghatározza a haladási irányt a jelenlegi (X, Y) és a következő (nextX, nextY) koordináták összehasonlítása alapján. A metódus megvizsgálja a két pozíció relatív helyzetét, és visszatér a megfelelő Direction értékkel (UP, DOWN, LEFT vagy RIGHT). Amennyiben a célállomás nem szomszédos a jelenlegi mezővel, vagy a koordináták megegyeznek, a jármű megőrzi az aktuális haladási irányát.
        /// </summary>
        public Direction DecideDirection(int nextX, int nextY, int X, int Y)
        {
            if (nextX == X - 1 && nextY == Y)
            {
                return Direction.UP;
            }
            if (nextX == X + 1 && nextY == Y)
            {
                return Direction.DOWN;
            }
            if (nextX == X && nextY == Y - 1)
            {
                return Direction.LEFT;
            }
            if (nextX == X && nextY == Y + 1)
            {
                return Direction.RIGHT;
            }
            else return this.direction;


        }

        protected Product ProductEnumToProduct(ProductEnum productE)
        {
            switch (productE)
            {
                case ProductEnum.Net: return new Net();
                case ProductEnum.Algae: return new Algae();
                case ProductEnum.Burger: return new Burger();
                case ProductEnum.Citizen: return new Citizen();
                case ProductEnum.Meat: return new Meat();
                case ProductEnum.Petfood: return new Petfood();
                case ProductEnum.Spatula: return new Spatula();
                case ProductEnum.Wood: return new Wood();
            }

            throw new Exception();
        }

    }
}
