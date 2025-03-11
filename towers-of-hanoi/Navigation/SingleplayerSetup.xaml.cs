using System.Windows;
using System.Windows.Controls;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for SingleplayerSetup.xaml
    /// </summary>
    public partial class SingleplayerSetup : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        Scene3D discScene;

        public SingleplayerSetup()
        {
            InitializeComponent();

            // setup 3d viewport
            discScene = new Scene3D(Viewport);
            discScene.Reset(DiscCount.Value, PoleCount.Value, 0, 2f);
        }

        private void CountsChanged(object sender, EventArgs e)
        {
            if (DiscCount != null && PoleCount != null && discScene != null)
            {
                discScene.Reset(DiscCount.Value, PoleCount.Value, 0, 2f);
            }
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).SwitchToSingleplayer(DiscCount.Value, PoleCount.Value);
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();
        }
    }
}
