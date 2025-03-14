using System.Windows;
using System.Windows.Controls;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for MultiplayerSetup.xaml
    /// </summary>
    public partial class MultiplayerSetup : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        Scene3D discScene;

        public MultiplayerSetup()
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
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerMenu();
        }

        private void CreateClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerServer();
        }

        public void Recolour()
        {
            discScene.Recolour();
        }
    }
}
