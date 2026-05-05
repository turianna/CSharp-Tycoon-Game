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
        public FieldData[][] board;
        public int gameTime;
        public int money;
        public List<FieldData> forests;
        public List<IndustryData> industries;
        public List<Vehicle> parkingVehicles;
        public List<Vehicle> movingVehicles;

        public GameData(GameModel model)
        {
            forests = new List<FieldData>();
            industries = new List<IndustryData>();

            this.gameTime = model.gameTime;
            this.money = model.money;

            //board initializing
            board = new FieldData[50][];
            for (int i = 0; i < 50; i++)
            {
                board[i] = new FieldData[100];
            }

            //board
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    board[i][j] = new FieldData(model.board[i, j]);
                }
            }

            //industry
            foreach (Industry industry in model.GetIndustries())
            {
                IndustryData industryData = new IndustryData(industry);
                industries.Add(industryData);
            }

            //vehicle


        }

    }
}
