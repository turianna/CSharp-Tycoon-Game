using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model.FieldTypes
{
    public class Forest : FieldType
    {
        public int treeCount;
        public int timeUntilNewTree;
        public int timeUntilSpread;
        public bool readyToSpread = false;
        private Random random = new Random();

        //CHANGE THESE TO MAKE FORESTS SPREAD FASTER/SLOWER
        private const int newTreeTimeMin = 120;
        private const int newTreeTimeMax = 180;
        private const int spreadTimeMin = 120;
        private const int spreadTimeMax = 180;

        public Forest()
        {
            treeCount = 1;
            timeUntilNewTree = random.Next(newTreeTimeMin, newTreeTimeMax);
            timeUntilSpread = random.Next(spreadTimeMin, spreadTimeMax);
        }

        /// <summary>
        /// GrowthTick
        /// Kezeli az erdőmező egyetlen időbeli lépését (növekedési ciklusát). Ha a fák száma (treeCount) kisebb mint 4, csökkenti az új fa növekedéséig hátralévő időt, illetve ha a fák száma nagyobb mint 2, csökkenti a terjedésig hátralévő időt. Amikor az új fa időzítője nullára ér és még van hely a mezőn, megnöveli a fák számát, beállít egy új véletlenszerű várakozási időt, és igaz (true) értékkel tér vissza (jelezve, hogy a mező megváltozott és frissíteni kell a nézetet). Ha a terjedési időzítő nullázódik le, igazra állítja a terjedésre kész (readyToSpread) jelzőt, leteszi a következő véletlenszerű terjedési időzítőt, majd (ha nem nőtt új fa) hamis (false) értékkel tér vissza.
        /// </summary>
        public bool GrowthTick() 
        {
            if(treeCount < 4)timeUntilNewTree--;
            if(treeCount > 2) timeUntilSpread--;

            if (timeUntilNewTree == 0 && treeCount < 4) 
            {
                treeCount++;
                timeUntilNewTree = random.Next(newTreeTimeMin, newTreeTimeMax);

                return true;
            }

            if (timeUntilSpread == 0) 
            {
                readyToSpread = true;
                timeUntilSpread = random.Next(spreadTimeMin, spreadTimeMax);
            }
            return false;
        }


        /// <summary>
        /// Spread
        /// Meghatározza egy erdőmező terjedésének véletlenszerű irányát a megadott i és j (sor és oszlop) koordináták alapján. A metódus az availableDirections függvény segítségével lekérdezi a játéktáblán (model.board) elérhető, szabad szomszédos irányokat. Ha nincs ilyen üres hely a terjedéshez, null értékkel tér vissza. Ellenkező esetben a lehetséges irányok listájából véletlenszerűen kiválaszt egyet, és azzal az iránnyal (Direction) tér vissza a hívó felé.
        /// </summary>
        public Direction? Spread(GameModel model, int i, int j) 
        {
            List<Direction> directions = availableDirections(model.board, i, j);

            if (directions.Count == 0) return null;

            //selecting a random direction from the ones available
            Direction spreadDir = directions[random.Next(0, directions.Count)];

            return spreadDir;
        }

        /// <summary>
        /// availableDirections
        /// Feltérképezi a megadott i és j koordináták körüli (fent, lent, jobbra, balra) szomszédos mezőket, és kigyűjti azokat az irányokat, amelyekbe az erdő továbbterjedhet (ahol a mező típusa Grass). A metódus try-catch blokkok segítségével kezeli a pálya széleit, így az IndexOutOfRangeException kivételeket elnyeli, ha a keresés a tábla határain kívülre esne, végül pedig visszatér az összes szabad irányt tartalmazó listával.
        /// </summary>
        private List<Direction> availableDirections(Field[,] fields, int i, int j) 
        {
            List<Direction> directions = new List<Direction>();

            try
            {
                if (fields[i, j-1].fieldType is Grass) { directions.Add(Direction.UP); }
            }
            catch (IndexOutOfRangeException) { }

            try
            {
                if (fields[i, j+1].fieldType is Grass) { directions.Add(Direction.DOWN); }
            }
            catch (IndexOutOfRangeException) { } 

            try
            {
                if (fields[i+1, j].fieldType is Grass) { directions.Add(Direction.RIGHT); }
            }
            catch (IndexOutOfRangeException) { }

            try
            {
                if (fields[i - 1, j].fieldType is Grass) { directions.Add(Direction.LEFT); }
            }
            catch (IndexOutOfRangeException) { }

            return directions;
        }

    }
}
