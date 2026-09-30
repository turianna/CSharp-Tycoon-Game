using Microsoft.Win32;
using SpongeBobTycoonWPF.ViewModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Runtime.CompilerServices;

namespace SpongeBobTycoonWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.Focus();
        }
        private void GameScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Csak akkor zoomolunk, ha a bal vagy jobb Ctrl le van nyomva
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                double zoomStep = 0.1; // Mennyivel nagyítson/kicsinyítsen egy görgetésre

                if (e.Delta > 0)
                {
                    // Felfelé görgetés: Nagyítás
                    BoardScaleTransform.ScaleX += zoomStep;
                    BoardScaleTransform.ScaleY += zoomStep;
                }
                else
                {
                    // Lefelé görgetés: Kicsinyítés (ne engedjük, hogy 0 vagy negatív legyen!)
                    if (BoardScaleTransform.ScaleX > 0.2)
                    {
                        BoardScaleTransform.ScaleX -= zoomStep;
                        BoardScaleTransform.ScaleY -= zoomStep;
                    }
                }

                // Ez megakadályozza, hogy a zoomolással egy időben görgessen is a ScrollViewer
                e.Handled = true;
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            
            var saveFileDialog = new Microsoft.Win32.SaveFileDialog();

            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string savesPath = System.IO.Path.Combine(appData, "SpongeBobTycoon", "saves");

            System.IO.Directory.CreateDirectory(savesPath);

            saveFileDialog.InitialDirectory = savesPath;
            saveFileDialog.Title = "Mentsd el a sárga szivacs birodalmadat! xd";

            saveFileDialog.Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*";

            if (saveFileDialog.ShowDialog() == true)
            {
                string selectedFileName = saveFileDialog.FileName;

                var viewModel = (MainViewModel)this.DataContext;
                viewModel.SaveGame(selectedFileName);

            }

        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            double scrollSpeed = 30;

            switch (e.Key)
            {
                case Key.W:
                    GameScrollViewer.ScrollToVerticalOffset(GameScrollViewer.VerticalOffset - scrollSpeed);
                    break;
                case Key.S:
                    GameScrollViewer.ScrollToVerticalOffset(GameScrollViewer.VerticalOffset + scrollSpeed);
                    break;
                case Key.A:
                    GameScrollViewer.ScrollToHorizontalOffset(GameScrollViewer.HorizontalOffset - scrollSpeed);
                    break;
                case Key.D:
                    GameScrollViewer.ScrollToHorizontalOffset(GameScrollViewer.HorizontalOffset + scrollSpeed);
                    break;
            }
        }
    }
}