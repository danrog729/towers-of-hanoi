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
using towers_of_hanoi.Navigation.Multiplayer;

namespace towers_of_hanoi.Navigation
{
    /// <summary>
    /// Interaction logic for MultiplayerServer.xaml
    /// </summary>
    public partial class MultiplayerServer : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        public string serverName;
        public int discs;
        public int poles;
        public int bestOf;

        public MultiplayerServer()
        {
            InitializeComponent();
            serverName = "server name";
        }

        public void OpenServer()
        {
            // send multicast signal announcing presence
            // start listening for multicast connections and respond if receives one
            Multiplayer.MultiCast.Connect();
            Multiplayer.MultiCast.SendServerResponse(serverName,discs,poles,bestOf);
            Multiplayer.MultiCast.ServerRequestMessageReceived += SendResponseMessage;
            TCP.StartServer();
        }

        public void CloseServer()
        {
            // send multicast signal announcing leaving
            // stop listening for multicast connections
            Multiplayer.MultiCast.SendServerResignment();
            Multiplayer.MultiCast.Disconnect();
            Multiplayer.MultiCast.ServerRequestMessageReceived -= SendResponseMessage;
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToServerQuitConfirmation();
        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {

        }

        private void SendResponseMessage(object? sender, EventArgs e)
        {
            Multiplayer.MultiCast.SendServerResponse(serverName, discs, poles, bestOf);
        }
    }
}
