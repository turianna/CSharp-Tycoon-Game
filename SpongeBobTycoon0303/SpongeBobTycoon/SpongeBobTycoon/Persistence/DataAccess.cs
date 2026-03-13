using SpongeBobTycoon.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    public class InvalidMapDataException : Exception { };
    public class DataAccess
    {

        public async Task<GameData> Load(String path) { }
        public async Task Save(String path, GameModel model) { }

        public Field[,] LoadMap()
        {
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; //map.txt is always copied to this directory on building the project. this is to avoid using absolute paths, because they only work on one computer

                Field[,] fields = new Field[50, 100];

                using (StreamReader sr = new StreamReader(Path.Combine(baseDirectory, "map.txt")))
                {
                    string? line;
                    string[] line_seperated; //this variable will store the data of fields seperately

                    int i = 0; //indexes to know where to put a new field into the field matrix
                    int j = 0;

                    while ((line = sr.ReadLine()) != null) 
                    {

                        line = line.Trim();
                        line_seperated = line.Split(" ");

                        j = 0;
                        foreach (string field_data in line_seperated) 
                        {
                            //data needed to create a field
                            int x = j;
                            int y = i;
                            int z;
                            FieldType fieldType;
                            bool isVehicle = false;

                            char height = field_data[0];
                            z = height - '0'; //converting char to int by exploiting unicode values

                            char fieldType_char;
                            char factoryType_char = ' ';
                            
                            fieldType_char = field_data[1];
                            if (fieldType_char == 'F')
                            {
                                if (field_data.Length == 3) factoryType_char = field_data[2];
                                else throw new InvalidMapDataException();
                            }

                            fieldType = DecideFieldType(fieldType_char, factoryType_char);

                            if (i < 50 && j < 100)
                            {
                                fields[i, j] = new Field(x, y, z, fieldType, isVehicle);
                            }
                            else {
                                throw new InvalidMapDataException(); //if there are more than 50 rows or 100 columns, the map data is invalid
                            }

                            j++;
                        } 

                        i++;
                    }


                    return fields;
                }

            }
            catch (Exception e) 
            {
                Console.WriteLine("Error on loading file");
                return null;
            }
        
        }

        private FieldType DecideFieldType(char fieldType, char factoryType) //helper method to keep clean code
        {
            switch (fieldType)
            {
                case 'E':
                    return FieldType.EMPTY;
                case 'R':
                    return FieldType.LAVA;
                case 'L':
                    return FieldType.LAVA;
                case 'C':
                    return FieldType.CITY;
                case 'D':
                    return FieldType.ROAD;
                case 'T':
                    return FieldType.TREE;
                case 'F':
                    switch (factoryType)
                    {
                        case 'H':
                            return FieldType.BURGERFACTORY;
                        case 'S':
                            return FieldType.SPATULAFARM;
                        case 'N':
                            return FieldType.NETFACTORY;
                        case 'M':
                            return FieldType.MEATFARM;
                        case 'K':
                            return FieldType.PETFOODFACTORY;
                        case 'F':
                            return FieldType.WOODFACTORY;
                        case 'A':
                            return FieldType.ALGAEFARM;
                        default:
                            throw new InvalidCastException ();
                    }

                default:
                    throw new InvalidMapDataException();
            }

            /*
                E - empty
                R - river
                L - lake
                C - city
                D - road
                T - tree
                F - factory
                    H - hamburger
                    S - spatula
                    N - net
                    M - meat
                    K - pet food
                    F - wood
                    A - algae
            */
        }

    }
}
