using System.Configuration;
using System.Data;
using System.Windows;
using SpongeBobTycoon.Model;
using SpongeBobTycoon.Persistence;
using SpongeBobTycoonWPF.ViewModel;

namespace SpongeBobTycoonWPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private GameModel model = null!;
        private MainViewModel viewModel = null!;
        private MainWindow view = null!;
        DataAccess access = new DataAccess();
        public App()
        {
            Startup += new StartupEventHandler(AppStartup);
        }

        private void AppStartup(object sender, StartupEventArgs e)
        {
            model = new GameModel(access);
            viewModel = new MainViewModel(model);
            view = new MainWindow();
            view.DataContext = viewModel;
            view.Show();

        }

    }

}
