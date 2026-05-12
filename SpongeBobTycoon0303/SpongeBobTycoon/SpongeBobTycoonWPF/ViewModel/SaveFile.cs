using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpongeBobTycoonWPF.ViewModel
{
    public class SaveFile : ViewModelBase
    {
        private string fileName;
        private int money;
        private int gameTime;

        public string FileName
        {
            get => fileName;
            set
            {
                fileName = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        public int Money
        {
            get => money;
            set
            {
                money = value;
                OnPropertyChanged(nameof(money));
            }
        }

        public int GameTime 
        {
            get => gameTime;
            set
            {
                gameTime = value;
                OnPropertyChanged(nameof(GameTime));
            }
        }

        public DelegateCommand LoadGame { get; set; }


    }
}
