using System.Configuration;
using System.Data;
using System.Windows;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Persistence;
using SpongeBobTycoonWPF.ViewModel;
using SpongeBobTycoonWPF.View;

namespace SpongeBobTycoonWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private GameModel model = null!;
        private MainViewModel viewModel = null!;
        private MenuWindow menuview = null!;
        private MainWindow gameview = null!;
        DataAccess access = new DataAccess();
        public App()
        {
            Startup += new StartupEventHandler(AppStartup);
        }

        private void AppStartup(object sender, StartupEventArgs e)
        {
            model = new GameModel(access);
            viewModel = new MainViewModel(model);
            menuview = new MenuWindow();
            gameview = new MainWindow();
            menuview.DataContext = viewModel;
            gameview.DataContext = viewModel;
            menuview.Show();

        }

    }

}
