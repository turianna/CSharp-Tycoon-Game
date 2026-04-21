using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class Vehicle 
    {
        public List<(int, int)>? route {  get; set; }
        public List<Product>? ingredients;
        public List<int>? ingredientAmounts;
        public List<int>? ingredientCapacities;

        public Product product;
        public string productName;
        public string typeName;
        public int productAmount;
        public int capacity;

        //public string productName;
        public int ID;
        public int route_index;
        
        public Direction direction { get; set; }
        protected int baseTime;
        public int timeUntilMoves;
        public int stepsUntilBroken;
        public int repairCost;
        
        public int cost;
        
        public int x;
        public int y;
        public int brokness;


        //public (int,int) startPoint = path[0]
        public Vehicle(Product product)
        {
            this.product = product;
            productAmount = 0;
            this.route = new List<(int, int)>();

            switch (product)
            {
                case Petfood:
                    ingredients = new List<Product>();
                    ingredientAmounts = new List<int>();
                    ingredientCapacities = new List<int>();

                    ingredients.Add(new Algae());
                    ingredientAmounts.Add(0);
                    ingredientCapacities.Add(10);
                    break;

                case Burger:
                    ingredients = new List<Product>();
                    ingredientAmounts = new List<int>();
                    ingredientCapacities = new List<int>();

                    ingredients.Add(new Spatula());
                    ingredients.Add(new Meat());
                    
                    ingredientAmounts.Add(0);
                    ingredientAmounts.Add(0);

                    ingredientCapacities.Add(5);
                    ingredientCapacities.Add(20);
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
                for (int i = 0; i < ingredients!.Count; i++)
                {
                    Factory stopFactory = (Factory)currentStop.industry;
                    stopFactory.ReceiveIngredient(ingredients[i], ingredientAmounts![i]);
                }
            }

            //ha a jarmu nem szallit ingredientet, akkor vegzett a metodus
            if (ingredients is null) return;

            //ha a megallonal a jarmu ingredientjet lehet felvenni
            foreach (Product ingredient in ingredients)
            {
                if (ingredient.GetType() == stopProduct.GetType())
                {
                    int index = ingredients.IndexOf(ingredient);
                    int ingredientAmount = ingredientAmounts![index];
                    int ingredientCapacity = ingredientCapacities![index];

                    int productToGive = Math.Min(ingredientCapacity - ingredientAmount, stopAmount);

                    ingredientAmounts[index] += productToGive;
                    currentStop.industry.productAmount -= productToGive;

                    System.Diagnostics.Debug.WriteLine($"Vehicle with id {ID} received {productToGive} of {ingredients[index].GetType().Name}. New amount: {ingredientAmounts[index]}/{ingredientCapacities[index]}");
                }
            }
        }

        public int CityStopReached()
        {
            //ha a jarmu egy varosba erkezik, akkor a benne levo termeket eladja, es a penzt megkapja
            int moneyEarned=0;

            switch (product)
            {
                case Citizen:
                    moneyEarned = productAmount * 100;
                    break;

                case Burger:
                    moneyEarned = productAmount * 400;
                    break;

                case Net:
                    moneyEarned = productAmount * 200;
                    break;

                case Wood:
                    moneyEarned = productAmount * 50;
                    break;

                case Petfood:
                    moneyEarned = productAmount * 300;
                    break;
            }

            System.Diagnostics.Debug.WriteLine($"Vehicle {ID} sold {productAmount} of {product.GetType().Name}, making {moneyEarned} dollars.");
            productAmount = 0;
            return moneyEarned;
        }

        public bool CanVehicleMove(Field[,] board, List<Vehicle> allVehicles)
        {
            int nextX = route![(route_index+1) % (route.Count - 1)].Item1;
            int nextY = route[(route_index+1) % (route.Count - 1)].Item2;
            this.direction = DecideDirection(nextX, nextY, this.x, this.y);

            Vehicle? vehicleInTheWay = null;
            if (board[nextX, nextY].isVehicle)
            {
                foreach(Vehicle v in allVehicles)
                {
                    if (v == this) continue;
                    if (v.x == nextX && v.y == nextY) vehicleInTheWay = v;
                }
                
                if(vehicleInTheWay != null && vehicleInTheWay.direction != InverseDirection(this.direction)) 
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

        public Direction DecideDirection(int nextX, int nextY, int X, int Y)
        {
            if(nextX == X-1 && nextY == Y)
            {
                return Direction.UP;
            }
            if(nextX == X + 1 && nextY == Y)
            {
                return Direction.DOWN;
            }
            if(nextX == X && nextY == Y - 1)
            {
                return Direction.LEFT;
            }
            if (nextX == X && nextY == Y + 1)
            {
                return Direction.RIGHT;
            }
            else return this.direction;


        }
    }
}
