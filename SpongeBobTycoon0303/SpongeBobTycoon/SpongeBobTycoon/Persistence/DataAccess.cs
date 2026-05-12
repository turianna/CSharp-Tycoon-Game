using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    public class InvalidMapDataException : Exception { };
    public class DataAccess
    {
        private List<Field> forests = new List<Field>();
        private List<Industry> industries = new List<Industry>();

        public void SaveGameDataToJson(GameData data, string fileName)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string jsonString = System.Text.Json.JsonSerializer.Serialize<GameData>(data, options);

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string saveFolder = Path.Combine(appData, "SpongeBobTycoon", "saves");
            string filePath = Path.Combine(saveFolder, fileName);

            System.Diagnostics.Debug.WriteLine($"Saving game data to: {saveFolder}");
            // This line prevents the DirectoryNotFoundException
            Directory.CreateDirectory(saveFolder);

            File.WriteAllText(filePath, jsonString);
        }

        public static (string, int, int) GetSaveFileData(string file)
        {
            string jsonString = File.ReadAllText(file);
            GameData? gameData = System.Text.Json.JsonSerializer.Deserialize<GameData>(jsonString);
            if (gameData is null) throw new Exception("Failed to deserialize game data.");
            
            string fileName = Path.GetFileNameWithoutExtension(file);
            return (fileName, gameData.gameTimer, gameData.money);
        }

        public GameModel LoadGameFromJson(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            GameData? gameData = System.Text.Json.JsonSerializer.Deserialize<GameData>(jsonString);
            if (gameData is null) throw new Exception("Failed to deserialize game data.");

            Field[,] board = new Field[50, 100];
            int gameTimer = gameData.gameTimer;
            int gameTime = gameData.gameTime;
            int money = gameData.money;
            int[] news = gameData.news;
            List<Field> forests = new List<Field>();
            List<Stop> stops = new List<Stop>();
            List<Industry> industries = new List<Industry>();
            List<Vehicle> parkingVehicles = new List<Vehicle>();
            List<Vehicle> movingVehicles = new List<Vehicle>();

            //industries
            foreach (IndustryData industryData in gameData.industries)
            {
                if (industryData.isFactory) industries.Add(new Factory(industryData));
                else industries.Add(new Farm(industryData));
            }

            //board
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    board[i,j] = new Field(gameData.board[i][j], industries);
                }
            }

            //forests
            foreach(Field field in board)
            {
                if (field.fieldType is Forest) forests.Add(field);
            }

            //stops
            foreach (StopData stop in gameData.stops)
            {
                stops.Add(new Stop(stop.x, stop.y, industries[stop.industryIndex]));
            }

            //parking vehicles
            foreach (VehicleData vehicleData in gameData.parkingVehicles)
            {
                if (vehicleData.isFastCar) parkingVehicles.Add(new FastCar(vehicleData));
                else parkingVehicles.Add(new SlowCar(vehicleData));
            }

            //moving vehicles
            foreach (VehicleData vehicleData in gameData.movingVehicles)
            {
                if (vehicleData.isFastCar) movingVehicles.Add(new FastCar(vehicleData));
                else movingVehicles.Add(new SlowCar(vehicleData));
            }

            return new GameModel(board, gameTime, gameTimer, money, forests, stops, industries, parkingVehicles, movingVehicles, news);

        }

        public Field[,] LoadMap()
        {
            forests.Clear();
            string baseDirectory = AppDomain.CurrentDomain.BaseDirectory; //map.txt is always copied to this directory on building the project. this is to avoid using absolute paths, because they only work on one computer

            Field[,] fields = new Field[50, 100];

            using (StreamReader sr = new StreamReader(Path.Combine(baseDirectory, "Data", "map.txt")))
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
                        Industry? industry;
                        bool isVehicle = false;
                        bool areTwoVehicles = false;
                        bool isStop = false;
                        Direction direction = Direction.NONE;

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

                            switch (industry!.product)
                            {
                                case null:
                                    break;
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
                        
                        

                        //ha uj industry-hoz ertunk a feldolgozasban, akkor hozzaadjuk az industry listahoz
                        //uj industry = a folotte, es tole balra levo mezo nem ugyanaz, mint a mostani industry
                        if ((industry is Factory || industry is Farm) && isNewIndustry(industry, i, j, fields))
                        {
                            industries.Add(industry);
                        }
                        else if ((industry is Factory || industry is Farm) && !isNewIndustry(industry, i, j, fields))
                        {
                            industry = getNeighbouringIndustry(i, j, fields);
                            //ha nem uj industry-nal vagyunk, akkor valamelyik szomszedja biztos, hogy a vizsgalt mezohoz azonos industryhoz tartozik
                        }

                        if (i < 50 && j < 100)
                        {
                            Field newField = new Field(x, y, z, industry, fieldType, isVehicle, areTwoVehicles, isStop,id, direction);
                            fields[i, j] = newField;

                            if (fieldType is Forest) forests.Add(newField);
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid map data at line {i + 1}, column {j + 1}"); //this is to help    find the error in the map data, if there is one
                            throw new InvalidMapDataException(); //if there are more than 50 rows or 100 columns, the map data is invalid
                        }
                        

                        j++;
                    }

                    i++;
                }

                for (int n = 0; n < industries.Count; n++)
                {
                    System.Diagnostics.Debug.WriteLine($"Industry {n + 1}: {industries[n].product.GetType()}");
                }


                return fields;
            }

        }

        private Industry getNeighbouringIndustry(int i, int j, Field[,] fields)
        {
            //left
            if(j != 0 && (fields[i, j - 1].industry is Factory || fields[i, j - 1].industry is Farm)) return fields[i, j - 1].industry!;
            //top
            if(i != 0 && (fields[i - 1, j].industry is Factory || fields[i - 1, j].industry is Farm)) return fields[i - 1, j].industry!;
            //right
            if(j != fields.GetLength(1) - 1 && (fields[i, j + 1].industry is Factory || fields[i, j + 1].industry is Farm)) return fields[i, j + 1].industry!;
            //bottom
            if(i != fields.GetLength(0) - 1 && (fields[i + 1, j].industry is Factory || fields[i + 1, j].industry is Farm)) return fields[i + 1, j].industry!;

            throw new InvalidMapDataException(); //ha nincsen a szomszedban industry, akkor valami nem okes
        }

        private bool isNewIndustry(Industry industry, int i, int j, Field[,] fields)
        {
            //if (industry is null) return false;

            bool leftIsSame = fields[i, j - 1].industry is not null && j != 0 && fields[i, j - 1].industry!.GetType() == industry.GetType();
            bool isTopSame = fields[i - 1, j].industry is not null && i != 0 && fields[i - 1, j].industry!.GetType() == industry.GetType();

            if (!isTopSame && !leftIsSame) return true;
            else return false;
        }

        private (FieldType, Industry) DecideFieldTypeAndIndustry(char fieldType, char factoryType) //helper method to keep clean code
        {
            
            switch (fieldType)
            {
                case 'E':
                    return (new Grass(), new EmptyIndustry());
                case 'R':
                    return (new Lava(true), new EmptyIndustry());
                case 'L':
                    return (new Lava(false), new EmptyIndustry());
                case 'C':
                    return (new Grass(), new Farm(new Citizen())); 
                case 'D':
                    return (new Road(), new Farm(new Citizen()));
                case 'T':
                    return (new Forest(), new EmptyIndustry());
                case 'F':
                    switch (factoryType)
                    {
                        case 'H':
                            return (new FactoryField(), new Factory(new Burger()));
                        case 'S':
                            return (new FactoryField(), new Farm(new Spatula()));
                        case 'N':
                            return (new FactoryField(), new Farm(new Net()));
                        case 'M':
                            return (new FactoryField(), new Farm(new Meat()));
                        case 'K':
                            return (new FactoryField(), new Factory(new Petfood()));
                        case 'F':
                            return (new FactoryField(), new Farm(new Wood()));
                        case 'A':
                            return (new FactoryField(), new Farm(new Algae()));
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
        public List<Industry> GetIndustries()
        {
            return industries;

        }

        
    }
}
