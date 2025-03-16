using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
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

namespace towers_of_hanoi.Navigation
{
    /// <summary>
    /// Interaction logic for MultiplayerMenu.xaml
    /// </summary>
    public partial class MultiplayerMenu : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        public MultiplayerMenu()
        {
            InitializeComponent();
        }

        public void Initialise()
        {
            Multiplayer.MultiCast.Connect();
            Multiplayer.MultiCast.SendServerRequest();
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            Multiplayer.MultiCast.Disconnect();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }

        private void SwitchToGameCreation(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerSetup();
        }
    }
}
