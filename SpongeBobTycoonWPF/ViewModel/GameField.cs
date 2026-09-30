using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class GameField : ViewModelBase
    {
        private int color;
        private int roadPicture;
        private int stopPicture;
        private int vehiclePicture;
        private string content = "";
        private bool isHighlighted;

        public int Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        public int RoadPicture
        {
            get => roadPicture;
            set
            {
                roadPicture = value;
                OnPropertyChanged(nameof(RoadPicture));
            }
        }
        public int VehiclePicture
        {
            get => vehiclePicture;
            set
            {
                vehiclePicture = value;
                OnPropertyChanged(nameof(VehiclePicture));
            }
        }
        public int StopPicture
        {
            get => stopPicture;
            set
            {
                stopPicture = value;
                OnPropertyChanged(nameof(StopPicture));
            }
        }

        public int x { get; set; }
        public int y { get; set; }

        public Tuple<int, int> XY
        {
            get { return new(x, y); }
            
        }

        public string Content 
        {
            get { return content; }
            set { content = value; OnPropertyChanged(nameof(Content)); }

        }

        public bool IsHighlighted
        {
            get { return isHighlighted; }
            set { isHighlighted = value; OnPropertyChanged(nameof(IsHighlighted)); }
        }
        public DelegateCommand? ClickCommand { get; set; }
        /* ez egyelőre csak ki lett másolva
        public int x { get; set; }
        public int y { get; set; }

        public Tuple<int, int> XY
        {
            get { return new(x, y); }
        }
        private bool isClickable;
        public bool IsClickable
        {
            get { return isClickable; }
            set { isClickable = value; OnPropertyChanged(); }
        }

        private bool isSelected;
        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; OnPropertyChanged(); }
        }
        private int status;
        public int Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged(); }
        }

        
        */
    }
}
