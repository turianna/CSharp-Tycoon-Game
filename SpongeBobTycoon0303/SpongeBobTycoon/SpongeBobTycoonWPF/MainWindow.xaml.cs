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
using SpongeBobTycoonWPF.ViewModel;

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
    }
}