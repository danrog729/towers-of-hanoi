using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace towers_of_hanoi.Navigation.Multiplayer
{
    /// <summary>
    /// Interaction logic for ServerQuitConfirmation.xaml
    /// </summary>
    public partial class ServerQuitConfirmation : Page
    {
        public static int DesiredWidth = 400;
        public static int DesiredHeight = 200;

        public ServerQuitConfirmation()
        {
            InitializeComponent();
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerServer();
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.CloseMultiplayer();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerSetup();
        }
    }
}
