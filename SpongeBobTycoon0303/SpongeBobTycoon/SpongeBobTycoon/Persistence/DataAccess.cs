using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography.X509Certificates;

namespace SpongeBobTycoon.Persistence
{
    public class InvalidMapDataException : Exception { };
    public class DataAccess
    {
        private List<Field> forests = new List<Field>();


        public async Task<GameData> Load(String path) { return new GameData(); }
        public async Task Save(String path, GameModel model) { }

        public Field[,] LoadMap()
        {
            forests.Clear();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; //map.txt is always copied to this directory on building the project. this is to avoid using absolute paths, because they only work on one computer

            Field[,] fields = new Field[50, 100];

            using (StreamReader sr = new StreamReader(Path.Combine(baseDirectory, "Data" , "map.txt")))
            {
                string? line;
                string[] line_seperated; //this variable will store the data of fields seperately

                int i = 0; //indexes to know where to put a new field into the field matrix
                int j = 0;
                int algee_id = 0;
                int burger_id = 0;
                int meat_id = 0;
                int net_id = 0;
                int petfood_id = 0;
                int spatula_id = 0;
                int wood_id = 0;
                int id = 0;


                while ((line = sr.ReadLine()) != null)
                {

                    //line = line.Trim();
                    line_seperated = line.Split(" ");
                   
                    j = 0;
                    foreach (string field_data in line_seperated)
                    {
                        //data needed to create a field
                        int x = i;
                        int y = j;
                        int z;
                        FieldType fieldType;
                        Industry industry;
                        bool isVehicle = false;
                        bool isStop = false;
                       

                        char height = field_data[0];
                        z = height - '0'; //converting char to int by exploiting unicode values

                        char fieldType_char;
                        char factoryType_char = ' ';

                        fieldType_char = field_data[1];
                        if (fieldType_char == 'F')
                        {
                            if (field_data.Length == 3) factoryType_char = field_data[2];
                            else 
                            {
                                System.Diagnostics.Debug.WriteLine($"Factory with not enough data found at line {i + 1}, column {j + 1}");
                                throw new InvalidMapDataException();
                            } 
                        }

                        (fieldType, industry) = DecideFieldTypeAndIndustry(fieldType_char, factoryType_char);

                        switch (industry.product)
                        {
                            case Burger:
                                burger_id++;
                                id = burger_id;
                                break;
                            case Meat:
                                meat_id++;
                                id = meat_id;
                                break;
                            case Algae:
                                algee_id++;
                                id = algee_id;
                                break;
                            case Net:
                                net_id++;
                                id = net_id;
                                break;
                            case Petfood:
                                petfood_id++;
                                id = petfood_id;
                                break;
                            case Spatula:
                                spatula_id++;
                                id = spatula_id;
                                break;
                            case Wood:
                                wood_id++;
                                id = wood_id;
                                break;
                            default:
                                id = 0; 
                                break;

                        }

                        if (i < 50 && j < 100)
                        {
                            Field newField = new Field(x, y, z, industry, fieldType, isVehicle, isStop,id);
                            fields[i, j] = newField;

                            if (fieldType is Forest) forests.Add(newField);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid map data at line {i + 1}, column {j + 1}"); //this is to help find the error in the map data, if there is one
                            throw new InvalidMapDataException(); //if there are more than 50 rows or 100 columns, the map data is invalid
                        }

                        j++;
                    }

                    i++;
                }


                return fields;
            }

        }

        private (FieldType, Industry?) DecideFieldTypeAndIndustry(char fieldType, char factoryType) //helper method to keep clean code
        {

            List<Product> ingredients = new List<Product>();
            switch (fieldType)
            {
                case 'E':
                    return (new Grass(), new Industry());
                case 'R':
                    return (new Lava(), new Industry());
                case 'L':
                    return (new Lava(), new Industry());
                case 'C':
                    ingredients.Clear();
                    ingredients.Add(new Citizen());
                    return (new Grass(), new Factory(ingredients, new Citizen())); //nem volt jobb megoldasom, mivel a citynek nincs sajat fieldypeja
                case 'D':
                    ingredients.Clear();
                    return (new Road(), new Factory(ingredients, new Citizen()));
                case 'T':
                    return (new Forest(), new Industry());
                case 'F':
                    switch (factoryType)
                    {
                        case 'H':
                            ingredients.Clear();
                            ingredients.Add(new Spatula());
                            ingredients.Add(new Meat());
                            return (new FieldType(), new Factory(ingredients, new Burger())); //a factoryknek sincsen mostmar fieldtypeja
                        case 'S':
                            return (new FieldType(), new Farm(new Spatula()));
                        case 'N':
                            return (new FieldType(), new Farm(new Net()));
                        case 'M':
                            return (new FieldType(), new Farm(new Meat()));
                        case 'K':
                            ingredients.Clear();
                            ingredients.Add(new Algae());
                            return (new FieldType(), new Factory(ingredients, new Petfood()));
                        case 'F':
                            return (new FieldType(), new Farm(new Wood()));
                        case 'A':
                            return (new FieldType(), new Farm(new Algae()));
                        default:
                            throw new InvalidCastException();
                    }

                default:
                    throw new InvalidMapDataException();
            }
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
        public List<Field> GetForests()
        {
            return forests;
        }

    }

}
