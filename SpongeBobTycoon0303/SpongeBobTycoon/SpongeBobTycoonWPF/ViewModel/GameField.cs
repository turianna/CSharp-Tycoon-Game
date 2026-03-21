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
        private string content = "";

        public int Color
        {
            get => color;
            set
            {
                color = value;
                OnPropertyChanged(nameof(Color));
            }
        }
        public int x { get; set; }
        public int y { get; set; }

        public string Content 
        {
            get { return content; }
            set { content = value; OnPropertyChanged(nameof(Content)); }

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
