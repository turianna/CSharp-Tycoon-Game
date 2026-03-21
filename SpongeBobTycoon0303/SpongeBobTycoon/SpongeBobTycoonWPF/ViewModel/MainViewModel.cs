using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;
using SpongeBobTycoon.Model.Products;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public GameModel model;
        public int color_int;
        public DelegateCommand NewGameCommand { get; private set; }
        public DelegateCommand SaveCommand { get; private set; }
        public DelegateCommand LoadCommand { get; private set; }
        public DelegateCommand StopTimeCommand { get; private set; }
        public DelegateCommand StartTimeCommand { get; private set; }
        public DelegateCommand SpeedUpTimeCommand { get; private set; }

        public DelegateCommand BuyVehicleCommand { get; private set; } //commandParameter-rel adjuk át a productot
        public DelegateCommand BuyBridgeCommand { get; private set; }
        public DelegateCommand BuyRoadCommand { get; private set; }
        public DelegateCommand PutStopCommand { get; private set; }
        public DelegateCommand TerraformUPCommand { get; private set; }
        public DelegateCommand TerraformDownCommand { get; private set; }
        public DelegateCommand CreateRouteCommand { get; private set; }



        public ObservableCollection<GameField> Board { get; private set; }


        public MainViewModel(GameModel model)
        {
            this.model = model;
            Board = new ObservableCollection<GameField>();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    switch (model.Board(i, j).fieldType)
                    {
                        case Forest:

                            color_int = 1;
                            break;

                        case Lava:
                            color_int = 0;
                            break;

                        case Road:
                            color_int = 4;
                            break;

                        case Grass:
                            color_int = 6;
                            break;
                    }
                    switch (model.Board(i, j).industry.product)
                    {
                        case Petfood: //PetfoodFactory
                            color_int = 2;
                            break;

                        case Burger:
                            color_int = 2;
                            break;

                        case Wood:
                            color_int = 2;
                            break;

                        case Algae:
                            color_int = 2;
                            break;

                        case Net:
                            color_int = 2;
                            break;

                        case Spatula:
                            color_int = 2;
                            break;

                        case Meat:
                            color_int = 2;
                            break;

                        case Citizen:
                            color_int = 3;
                            break;
                    }
                    Board.Add(new GameField { x = i, y = j, Color = color_int });
                }

            }
        }
    }
}
