using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;
using System;
using System.Collections.Frozen;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoon.Persistence
{
    public class GameData
    {
        public FieldData[][] board { get; set; }
        public int gameTime { get; set; }
        public int money { get; set; }
        public List<FieldData> forests { get; set; }
        public List<IndustryData> industries { get; set; }
        public List<VehicleData> parkingVehicles { get; set; }
        public List<VehicleData> movingVehicles { get; set; }
        public int[] news { get; set; }

        public GameData()
        {
            board = new FieldData[50][];
            forests = new List<FieldData>();
            industries = new List<IndustryData>();
            parkingVehicles = new List<VehicleData>();
            movingVehicles = new List<VehicleData>();
            news = new int[4];
        }
        public GameData(GameModel model)
        {
            forests = new List<FieldData>();
            industries = new List<IndustryData>();
            parkingVehicles = new List<VehicleData>();
            movingVehicles = new List<VehicleData>();

            this.gameTime = model.gameTime;
            this.money = model.money;
            this.news = model.getNews;

            //board initializing
            board = new FieldData[50][];
            for (int i = 0; i < 50; i++)
            {
                board[i] = new FieldData[100];
            }

            //industry
            foreach (Industry industry in model.GetIndustries())
            {
                IndustryData industryData = new IndustryData(industry);
                industries.Add(industryData);
            }

            //board
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int industryIndex = getIndustryIndex(model.board[i, j].industry, model.GetIndustries());
                    board[i][j] = new FieldData(model.board[i, j], industryIndex);
                }
            }



            //vehicle
            foreach (Vehicle parkingVehicle in model.parkingVehicles)
            {
                VehicleData vehicleData = new VehicleData(parkingVehicle);
                parkingVehicles.Add(vehicleData);
            }

            foreach (Vehicle movingVehicle in model.movingVehicles)
            {
                VehicleData vehicleData = new VehicleData(movingVehicle);
                movingVehicles.Add(vehicleData);

            }
        }

        private int getIndustryIndex(Industry industry, List<Industry> industries)
        {
            for (int i = 0; i < industries.Count; i++)
            {
                if (industries[i] == industry)
                {
                    return i;
                }
            }
            return -1;
        }
    }
}
