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

        /// <summary>
        /// Terraform
        /// Végrehajtja a tereprendezést a játéktábla (board) adott x és y koordinátájú mezőjén. A metódus először ellenőrzi, hogy a célmező alkalmas-e a módosításra: csak fű (Grass) vagy erdő (Forest) típusú területen engedélyezi a műveletet. Ezt követően a validTerraform segédfüggvénnyel megvizsgálja, hogy a magasságmódosítás (emelés vagy süllyesztés) megfelel-e a szabályoknak (például a szomszédos mezőktől való maximális eltérésnek). Ha minden feltétel teljesül, az isUp paramétertől függően növeli vagy csökkenti a mező magassági értékét (Z), és igaz (true) értékkel tér vissza; egyébként a módosítás elmarad és hamis (false) értéket ad vissza.
        /// </summary>
        public bool Terraform(Field[,] board, int x, int y, bool isUp) 
        {
            Field field = board[x, y];

            System.Diagnostics.Debug.WriteLine($"Terraforming at ({x}, {y}), isUp: {isUp}, old Z: {field.Z}");

            if (!(field.fieldType is Grass || field.fieldType is Forest)) return false;

            if(!validTerraform(board, x, y, isUp)) return false;

            if (isUp) field.Z++; else field.Z--;

            return true;


        }
        /// <summary>
        /// validTerraform
        /// Ellenőrzi, hogy a kért tereprendezési művelet (emelés vagy süllyesztés) végrehajtható-e a játéktábla adott pontján a magassági szabályok betartásával. Emelés esetén (isUp) a metódus megvizsgálja, hogy a mező elérte-e már a maximális magasságot (maxHeight), illetve hogy az emelés után nem alakul-e ki egynél nagyobb szintkülönbség a közvetlen szomszédos mezőkhöz képest. Süllyesztés esetén ellenőrzi, hogy a mező nincs-e már a minimális (0) szinten, valamint hogy a süllyesztés nem eredményez-e egynél nagyobb különbséget a környező mezők javára. A pálya széleinél fellépő indexelési hibákat (IndexOutOfRangeException) try-catch blokkokkal kezeli, és csak akkor tér vissza igaz (true) értékkel, ha a módosítás nem sérti a terep folytonosságára vonatkozó korlátokat.
        /// </summary>
        private bool validTerraform(Field[,] board, int x, int y, bool isUp)
        {
            if(isUp)
            {
                if(board[x, y].Z >= maxHeight) return false;          //if terraforming up and current tile is already at max height, return false
                try
                {
                    if ( (board[x, y].Z - board[x + 1, y].Z) > 1) return false;          //if terraforming up and current tile is more than 1 higher than any adjacent tile, return false
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[x, y].Z - board[x - 1, y].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ( (board[x, y].Z - board[x, y - 1].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ( (board[x, y].Z - board[x, y + 1].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }
            }
            else
            {
                if(board[x, y].Z == 0) return false;          //if terraforming down and current tile is already at min height, return false
                try
                {
                    if ((board[x+1, y].Z - board[x, y].Z) > 1) return false;          //if terraforming down and current tile is more than 1 lower than any adjacent tile, return false
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[x-1, y].Z - board[x, y].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[x, y+1].Z - board[x, y].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }

                try
                {
                    if ((board[x, y-1].Z - board[x, y].Z) > 1) return false;
                }
                catch (IndexOutOfRangeException) { }
            }


                return true;


        }
    }
}
