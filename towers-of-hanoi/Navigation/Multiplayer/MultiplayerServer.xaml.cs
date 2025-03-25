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

        private bool otherPlayerReady;
        private bool iAmReady;
        string otherPlayerName;

        public MultiplayerServer()
        {
            InitializeComponent();
            serverName = "server name";
            discScene = new Scene3D(Viewport);
            discScene.Reset(6, 3, 0, 2f);
            otherPlayerReady = false;
            iAmReady = false;
            otherPlayerName = "other player";
        }

        public void OpenServer()
        {
            MulticastConnect();
            Multiplayer.TCP.GreetingReceived += PlayerJoined;
            Status.Text = "Waiting for player...\nYou are not ready.";
            otherPlayerReady = false;
            iAmReady = false;
            TCP.StartServer();
        }

        public void CloseServer()
        {
            MulticastDisconnect();
            Multiplayer.TCP.GreetingReceived -= PlayerJoined;
            otherPlayerReady = false;
            iAmReady = false;
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

        private void ReadyClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            TCP.SendReadyMessage();
            iAmReady = true;
            Status.Text = otherPlayerName + " is not ready.\nYou are ready.";
            if (otherPlayerReady)
            {
                ((MainWindow)(App.MainApp.MainWindow)).SwitchToMultiplayer(discs, poles);
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();

                Multiplayer.TCP.LeaveMessageReceived -= PlayerLeft;
                Multiplayer.TCP.ReadyMessageReceived -= ReadyToStart;
            }
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
                otherPlayerName = data.Value.Item2;
                MulticastDisconnect();
                TCP.Connect(ip, serverName);
                Status.Text = otherPlayerName + " is not ready.\nYou are not ready.";
                ReadyButton.IsEnabled = true;
                otherPlayerReady = false;

                Multiplayer.TCP.LeaveMessageReceived += PlayerLeft;
                Multiplayer.TCP.ReadyMessageReceived += ReadyToStart;
            }
        }

        private void PlayerLeft(object? sender, EventArgs e)
        {
            MulticastConnect();
            Status.Text = "Waiting for player...\nYou are not ready.";
            ReadyButton.IsEnabled = false;
            otherPlayerReady = false;

            Multiplayer.TCP.LeaveMessageReceived -= PlayerLeft;
            Multiplayer.TCP.ReadyMessageReceived -= ReadyToStart;
        }

        private void ReadyToStart(object? sender, EventArgs e)
        {
            otherPlayerReady = true;
            Status.Text = otherPlayerName + " is ready.\nYou are not ready";
            if (iAmReady)
            {
                ((MainWindow)(App.MainApp.MainWindow)).SwitchToMultiplayer(discs, poles);
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();

                Multiplayer.TCP.LeaveMessageReceived -= PlayerLeft;
                Multiplayer.TCP.ReadyMessageReceived -= ReadyToStart;
            }
        }

        public void UpdateDetails(string Name, int Discs, int Poles, int BestOf)
        {
            serverName = Name;
            discs = Discs;
            poles = Poles;
            bestOf = BestOf;

            // update textboxes and viewport and stuff
            discScene.Reset(discs, poles, 0, 2);
            NameBox.Text = "Name: " + serverName;
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
