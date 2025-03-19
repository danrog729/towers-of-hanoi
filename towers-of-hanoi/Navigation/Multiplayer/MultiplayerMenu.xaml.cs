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
using System.Windows.Documents.DocumentStructures;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

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

        private void AddServerListing(object? sender, EventArgs e)
        {
            (string, string, int, int, int)? details = sender as (string, string, int, int, int)?;
            if (details != null)
            {
                ServerList.Children.Add(new Multiplayer.ServerEntry() { ServerName = details.Value.Item2, IPAddress = details.Value.Item1 });
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
                            ServerList.Children.Remove(entry);
                            return;
                        }
                    }
                }
            }
        }
    }
}
