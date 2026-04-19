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
        private List<Industry> industries = new List<Industry>();


        public async Task<GameData> Load(String path) { return new GameData(); }
        public async Task Save(String path, GameModel model) { }

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

                        if(i>= 50 || j >= 100)
                        {
                            System.Diagnostics.Debug.WriteLine($"Invalid map data at line {i + 1}, column {j + 1}"); //this is to help find the error in the map data, if there is one
                            throw new InvalidMapDataException(); //if there are more than 50 rows or 100 columns, the map data is invalid
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


                        Field newField = new Field(x, y, z, industry!, fieldType, isVehicle, isStop);
                        fields[i, j] = newField;

                        if (fieldType is Forest) forests.Add(newField);
                        
                        

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

        private (FieldType, Industry?) DecideFieldTypeAndIndustry(char fieldType, char factoryType) //helper method to keep clean code
        {
            
            switch (fieldType)
            {
                case 'E':
                    return (new Grass(), null);
                case 'R':
                    return (new Lava(), null);
                case 'L':
                    return (new Lava(), null);
                case 'C':
                    return (new Grass(), new Farm(new Citizen())); 
                case 'D':
                    return (new Road(), new Farm(new Citizen()));
                case 'T':
                    return (new Forest(), null);
                case 'F':
                    switch (factoryType)
                    {
                        case 'H':
                            return (new FieldType(), new Factory(new Burger()));
                        case 'S':
                            return (new FieldType(), new Farm(new Spatula()));
                        case 'N':
                            return (new FieldType(), new Farm(new Net()));
                        case 'M':
                            return (new FieldType(), new Farm(new Meat()));
                        case 'K':
                            return (new FieldType(), new Factory(new Petfood()));
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
        public List<Industry> GetIndustries()
        {
            return industries;

        }
    }
}
