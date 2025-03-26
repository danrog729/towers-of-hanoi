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
    /// Interaction logic for MultiplayerClient.xaml
    /// </summary>
    public partial class MultiplayerClient : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        private string serverName;
        private int discs;
        private int poles;
        private int bestOf;

        private Scene3D discScene;

        private bool iAmReady;
        private bool otherPlayerReady;

        public MultiplayerClient()
        {
            InitializeComponent();
            serverName = "server name";
            discScene = new Scene3D(Viewport);
            discScene.Reset(6, 3, 0, 2f);
            otherPlayerReady = false;
            iAmReady = false;
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToClientQuitConfirmation();
        }

        private void ReadyClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            TCP.SendReadyMessage();
            iAmReady = true;
            Status.Text = serverName + " is not ready.\nYou are ready.";
            if (otherPlayerReady)
            {
                ((MainWindow)(App.MainApp.MainWindow)).SwitchToMultiplayer(discs, poles);
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();
            }
        }

        public void JoinServer(string ip, string playerName)
        {
            TCP.Connect(ip, playerName);
            TCP.StartServer();
            TCP.ReadyMessageReceived += ReadyToStart;
            TCP.LeaveMessageReceived += ServerClosed;
            otherPlayerReady = false;
            iAmReady = false;
            Status.Text = serverName + " is not ready.\nYou are not ready.";
        }

        public void LeaveServer()
        {
            TCP.Disconnect();
            TCP.CloseServer();
            TCP.ReadyMessageReceived -= ReadyToStart;
            TCP.LeaveMessageReceived -= ServerClosed;
            otherPlayerReady = false;
            iAmReady = false;
        }

        private void ServerClosed(object? sender, EventArgs e)
        {
            LeaveServer();
            MessageBox.Show("Server closed.");
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerMenu();
        }

        private void ReadyToStart(object? sender, EventArgs e)
        {
            otherPlayerReady = true;
            Status.Text = serverName + " is ready.\nYou are not ready.";
            if (iAmReady)
            {
                ((MainWindow)(App.MainApp.MainWindow)).SwitchToMultiplayer(discs, poles);
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();
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
