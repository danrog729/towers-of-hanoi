using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using towers_of_hanoi.Navigation.Multiplayer;

namespace towers_of_hanoi.Navigation
{
    /// <summary>
    /// Interaction logic for MultiplayerMenu.xaml
    /// </summary>
    public partial class MultiplayerMenu : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        private bool nameValid;
        private bool isServerSelected;
        private int serverSelected;

        public MultiplayerMenu()
        {
            InitializeComponent();
            nameValid = false;
            isServerSelected = false;
            serverSelected = 0;
        }

        public void Initialise()
        {
            ServerList.Children.Clear();
            Multiplayer.MultiCast.Connect();
            Multiplayer.MultiCast.SendServerRequest();
            Multiplayer.MultiCast.ServerResponseMessageReceived += AddServerListing;
            Multiplayer.MultiCast.ServerResignmentMessageReceived += RemoveServerListing;
        }

        public void Deinitialise()
        {
            Multiplayer.MultiCast.Disconnect();
            Multiplayer.MultiCast.ServerResponseMessageReceived -= AddServerListing;
            Multiplayer.MultiCast.ServerResignmentMessageReceived -= RemoveServerListing;
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            Deinitialise();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }

        private void SwitchToGameCreation(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            Deinitialise();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerSetup();
        }

        private void LoseFocusFromEntry(object sender, EventArgs e)
        {
            foreach (UIElement element in ServerList.Children)
            {
                ServerEntry? entry = element as ServerEntry;
                if (entry != null)
                {
                    entry.EntryUnclicked();
                }
            }
            isServerSelected = false;
            JoinButton.IsEnabled = false;
        }

        public void SetSelectedEntry(ServerEntry entry)
        {
            if (nameValid)
            {
                JoinButton.IsEnabled = true;
            }
            serverSelected = ServerList.Children.IndexOf(entry);
            isServerSelected = true;
            for (int index = 0; index < ServerList.Children.Count; index++)
            {
                if (index != serverSelected)
                {
                    ServerEntry? serverEntry = ServerList.Children[index] as ServerEntry;
                    if (serverEntry != null)
                    {
                        serverEntry.EntryUnclicked();
                    }
                }
            }
        }

        private void NameChanged(object sender, TextChangedEventArgs e)
        {
            nameValid = true;
            if (NameBox.Text.Trim() == "")
            {
                nameValid = false;
                JoinButton.IsEnabled = false;
            }
            else if (isServerSelected)
            {
                JoinButton.IsEnabled = true;
            }
        }

        private void JoinGame(object sender, RoutedEventArgs e)
        {
            Deinitialise();
            ServerEntry entry = ((ServerEntry)ServerList.Children[serverSelected]);
            ((MainWindow)App.MainApp.MainWindow).navigationWindow.SetServerSettings(entry.ServerName, entry.Discs, entry.Poles, entry.BestOf);
            ((MainWindow)App.MainApp.MainWindow).navigationWindow.SwitchToMultiplayerClient();
            // establish a tcp connection, which should tell the server to announcing leaving to the multicast
            TCP.Connect(entry.IPAddress, NameBox.Text);
            // start playing the game
        }

        private void AddServerListing(object? sender, EventArgs e)
        {
            (string, string, int, int, int)? details = sender as (string, string, int, int, int)?;
            if (details != null)
            {
                ServerList.Children.Add(new Multiplayer.ServerEntry() { 
                    ServerName = details.Value.Item2, 
                    IPAddress = details.Value.Item1,
                    Discs = details.Value.Item3,
                    Poles = details.Value.Item4,
                    BestOf = details.Value.Item5
                });
            }
        }

        private void RemoveServerListing(object? sender, EventArgs e)
        {
            string? ip = sender as string;
            if (ip != null)
            {
                for (int index = 0; index < ServerList.Children.Count; index++)
                {
                    UIElement control = ServerList.Children[index];
                    Multiplayer.ServerEntry? entry = control as Multiplayer.ServerEntry;
                    if (entry != null)
                    {
                        if (entry.IPAddress == ip)
                        {
                            if (serverSelected == ServerList.Children.IndexOf(entry))
                            {
                                JoinButton.IsEnabled = false;
                            }
                            ServerList.Children.Remove(entry);
                        }
                    }
                }
            }
        }
    }
}
