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

        private string serverName;
        private int discs;
        private int poles;
        private int bestOf;

        private Scene3D discScene;

        public MultiplayerServer()
        {
            InitializeComponent();
            serverName = "server name";
            discScene = new Scene3D(Viewport);
            discScene.Reset(6, 3, 0, 2f);
        }

        public void OpenServer()
        {
            MulticastConnect();
            Multiplayer.TCP.GreetingReceived += PlayerJoined;
            TCP.StartServer();
        }

        public void CloseServer()
        {
            MulticastDisconnect();
            Multiplayer.TCP.GreetingReceived -= PlayerJoined;
            TCP.CloseServer();
        }
        
        private void MulticastDisconnect()
        {
            // send multicast signal announcing leaving
            // stop listening for multicast connections
            Multiplayer.MultiCast.SendServerResignment();
            Multiplayer.MultiCast.Disconnect();
            Multiplayer.MultiCast.ServerRequestMessageReceived -= SendResponseMessage;
        }

        private void MulticastConnect()
        {

            // send multicast signal announcing presence
            // start listening for multicast connections and respond if receives one
            Multiplayer.MultiCast.Connect();
            Multiplayer.MultiCast.SendServerResponse(serverName, discs, poles, bestOf);
            Multiplayer.MultiCast.ServerRequestMessageReceived += SendResponseMessage;
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToServerQuitConfirmation();
        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).SwitchToMultiplayer(discs, poles);
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();
        }

        private void SendResponseMessage(object? sender, EventArgs e)
        {
            Multiplayer.MultiCast.SendServerResponse(serverName, discs, poles, bestOf);
        }

        private void PlayerJoined(object? sender, EventArgs e)
        {
            (string, string)? data = sender as (string, string)?;
            if (data != null)
            {
                string ip = data.Value.Item1;
                string name = data.Value.Item2;
                MulticastDisconnect();
                Status.Text = "Player joined: " + name + " at " + ip;
                StartButton.IsEnabled = true;
            }
        }

        private void PlayerLeft(object? sender, EventArgs e)
        {
            MulticastConnect();
            Status.Text = "Waiting for player...";
            StartButton.IsEnabled = false;
        }

        public void UpdateDetails(string Name, int Discs, int Poles, int BestOf)
        {
            serverName = Name;
            discs = Discs;
            poles = Poles;
            bestOf = BestOf;

            // update textboxes and viewport and stuff
            discScene.Reset(discs, poles, 0, 2);
            DiscBox.Text = "Discs: " + discs;
            PoleBox.Text = "Poles: " + poles;
            BestOfBox.Text = "Best of: " + bestOf;
        }

        public void Recolour()
        {
            discScene.Recolour();
        }
    }
}
