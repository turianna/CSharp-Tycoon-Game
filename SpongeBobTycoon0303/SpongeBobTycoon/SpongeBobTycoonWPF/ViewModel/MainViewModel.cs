using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Model.FieldTypes;
using SpongeBobTycoon.Model.Industries;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        public GameModel model;
        public int color_int;

        private int time=0;

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
        public int GameTime { 
            get { 
                return time; 
            }
            set {
                time = value;
                OnPropertyChanged(nameof(GameTime));
            } 
        }


        public MainViewModel(GameModel model)
        {
            this.model = model;
            this.model.FieldChanged += Model_FieldChanged;
            this.model.TimerTick += Model_TimerTick;

            Board = new ObservableCollection<GameField>();
            for (int i = 0; i < 50; i++)
            {
                for (int j = 0; j < 100; j++)
                {
                    int color_int = DecideColor_int(model.Board(i, j).fieldType, model.Board(i, j).industry);

                    Board.Add(new GameField { x = i, y = j, Color = color_int, Content = "" });
                    if(color_int == 1) Board[i * 100 + j].Content = ((Forest)model.Board(i, j).fieldType!).treeCount.ToString();
                }

            }
        }

        private void Model_TimerTick(object? sender, int e)
        {
            GameTime = model.gameTimer;
        }

        private void Model_FieldChanged(object? sender, FieldChangedEventArgs e)
        {
            int color_int = DecideColor_int(model.Board(e.y, e.x).fieldType, model.Board(e.y, e.x).industry);

            Board[e.y*100 + e.x].Color = color_int;

            if (model.Board(e.y, e.x).fieldType is Forest)
            {
                Forest forest = (Forest)model.Board(e.y, e.x).fieldType!;
                Board[e.y * 100 + e.x].Content = forest.treeCount.ToString();
            }
            else 
            {
                Board[e.y * 100 + e.x].Content = "";
            }
        }

        private int DecideColor_int(FieldType? ft, Industry? ind) 
        {
            int color_int=6;

            switch (ft)
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
            switch (ind)
            {
                case PetfoodFactory:
                    color_int = 2;
                    break;

                case BurgerFactory:
                    color_int = 2;
                    break;

                case WoodFactory:
                    color_int = 2;
                    break;

                case AlgaeFarm:
                    color_int = 2;
                    break;

                case NetFactory:
                    color_int = 2;
                    break;

                case SpatulaFarm:
                    color_int = 2;
                    break;

                case MeatFarm:
                    color_int = 2;
                    break;

                case City:
                    color_int = 3;
                    break;
            }

            return color_int;

        } 
    }
}
