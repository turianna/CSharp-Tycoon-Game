using SpongeBobTycoon.Model.FieldTypes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Model
{
    public class TerraformingManager
    {
        private const int maxHeight = 4;
        public bool terraformUp;
        public int X; public int Y;
        
        public bool Terraform(Field[,] board, int x, int y, bool isUp) 
        {
            Field field = board[y, x];

            System.Diagnostics.Debug.WriteLine($"Terraforming at ({x}, {y}), isUp: {isUp}, old Z: {field.Z}");

            if (!(field.fieldType is Grass || field.fieldType is Forest)) return false;

            if(!validTerraform(board, x, y, isUp)) return false;

            if (isUp) field.Z++; else field.Z--;

            return true;


        }
        
        private bool validTerraform(Field[,] board, int x, int y, bool isUp)
        {
            if(isUp)
            {
                if(board[y, x].Z >= maxHeight) return false;          //if terraforming up and current tile is already at max height, return false
                try
                {
                    if ( (board[y, x].Z - board[y + 1, x].Z) > 1) return false;          //if terraforming up and current tile is more than 1 higher than any adjacent tile, return false
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[y, x].Z - board[y - 1, x].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ( (board[y, x].Z - board[y, x - 1].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ( (board[y, x].Z - board[y, x + 1].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }
            }
            else
            {
                if(board[y, x].Z == 0) return false;          //if terraforming down and current tile is already at min height, return false
                try
                {
                    if ((board[y+1, x].Z - board[y, x].Z) > 1) return false;          //if terraforming down and current tile is more than 1 lower than any adjacent tile, return false
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[y-1, x].Z - board[y, x].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[y, x+1].Z - board[y, x].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[y, x-1].Z - board[y, x].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }
            }


                return true;


        }
    }
}
