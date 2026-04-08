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
        private int timeUntilNewTree;
        private int timeUntilSpread;
        public bool readyToSpread = false;
        private Random random = new Random();

        //CHANGE THESE TO MAKE FORESTS SPREAD FASTER/SLOWER
        private const int newTreeTimeMin = 12;
        private const int newTreeTimeMax = 18;
        private const int spreadTimeMin = 12;
        private const int spreadTimeMax = 18;

        public Forest()
        {
            treeCount = 1;
            timeUntilNewTree = random.Next(newTreeTimeMin, newTreeTimeMax);
            timeUntilSpread = random.Next(spreadTimeMin, spreadTimeMax);
        }

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



        public Direction? Spread(GameModel model, int i, int j) 
        {
            List<Direction> directions = availableDirections(model.board, i, j);

            if (directions.Count == 0) return null;

            //selecting a random direction from the ones available
            Direction spreadDir = directions[random.Next(0, directions.Count)];

            return spreadDir;
        }

        private List<Direction> availableDirections(Field[,] fields, int i, int j) 
        {
            List<Direction> directions = new List<Direction>();

            try
            {
                if (fields[i - 1, j].fieldType is Grass) { directions.Add(Direction.UP); }
            }
            catch (IndexOutOfRangeException) { }

            try
            {
                if (fields[i + 1, j].fieldType is Grass) { directions.Add(Direction.DOWN); }
            }
            catch (IndexOutOfRangeException) { }

            try
            {
                if (fields[i, j - 1].fieldType is Grass) { directions.Add(Direction.LEFT); }
            }
            catch (IndexOutOfRangeException) { }

            try
            {
                if (fields[i, j + 1].fieldType is Grass) { directions.Add(Direction.RIGHT); }
            }
            catch (IndexOutOfRangeException) { }
            
            return directions;
        }

    }
}
