﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using towers_of_hanoi.Navigation;
using towers_of_hanoi.Navigation.Multiplayer;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        public MainMenu mainMenu;
        private SingleplayerSetup singleplayerSetup;

        private MultiplayerMenu multiplayerMenu;
        private MultiplayerSetup multiplayerSetup;
        private MultiplayerServer multiplayerServer;
        private ServerQuitConfirmation serverQuitConfirmation;
        private MultiplayerClient multiplayerClient;
        private ClientQuitConfirmation clientQuitConfirmation;
        private MultiplayerQuitConfirmation multiplayerQuitConfirmation;

        private AutomaticSetup automaticSetup;
        private Settings settings;
        private QuitConfirmation quitConfirmation;

        public NavigationWindow()
        {
            InitializeComponent();
            mainMenu = new MainMenu();
            singleplayerSetup = new SingleplayerSetup();
            multiplayerMenu = new MultiplayerMenu();
            multiplayerSetup = new MultiplayerSetup();
            multiplayerServer = new MultiplayerServer();
            serverQuitConfirmation = new ServerQuitConfirmation();
            multiplayerClient = new MultiplayerClient();
            clientQuitConfirmation = new ClientQuitConfirmation();
            multiplayerQuitConfirmation = new MultiplayerQuitConfirmation();
            automaticSetup = new AutomaticSetup();
            settings = new Settings();
            quitConfirmation = new QuitConfirmation();
            SwitchToMainMenu();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                App.MainApp.clickSound.Play();
                if (NavigationFrame.Content == mainMenu)
                {
                    Hide();
                }
                else if (NavigationFrame.Content == multiplayerSetup)
                {
                    SwitchToMultiplayerMenu();
                }
                else if (NavigationFrame.Content == multiplayerServer)
                {
                    SwitchToServerQuitConfirmation();
                }
                else if (NavigationFrame.Content == serverQuitConfirmation)
                {
                    SwitchToMultiplayerServer();
                }
                else if (NavigationFrame.Content == multiplayerClient)
                {
                    SwitchToClientQuitConfirmation();
                }
                else if (NavigationFrame.Content == clientQuitConfirmation)
                {
                    SwitchToMultiplayerClient();
                }
                else
                {
                    SwitchToMainMenu();
                }
            }
        }

        public void SwitchToMainMenu()
        {
            Left += Width / 2 - MainMenu.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - MainMenu.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            mainMenu.Width = MainMenu.DesiredWidth;
            mainMenu.Height = MainMenu.DesiredHeight;

            NavigationFrame.Content = mainMenu;
        }

        public void SwitchToSingleplayerSetup()
        {
            Left += Width / 2 - SingleplayerSetup.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - SingleplayerSetup.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            singleplayerSetup.Width = SingleplayerSetup.DesiredWidth;
            singleplayerSetup.Height = SingleplayerSetup.DesiredHeight;

            NavigationFrame.Content = singleplayerSetup;
        }

        public void SwitchToMultiplayerMenu()
        {
            Left += Width / 2 - MultiplayerMenu.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - MultiplayerMenu.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            multiplayerSetup.Width = MultiplayerMenu.DesiredWidth;
            multiplayerSetup.Height = MultiplayerMenu.DesiredHeight;

            NavigationFrame.Content = multiplayerMenu;

            multiplayerMenu.Initialise();
        }

        public void SwitchToMultiplayerSetup()
        {
            Left += Width / 2 - MultiplayerSetup.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - MultiplayerSetup.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            multiplayerSetup.Width = MultiplayerSetup.DesiredWidth;
            multiplayerSetup.Height = MultiplayerSetup.DesiredHeight;

            NavigationFrame.Content = multiplayerSetup;
        }

        public void SwitchToMultiplayerServer()
        {
            Left += Width / 2 - MultiplayerServer.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - MultiplayerServer.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            multiplayerServer.Width = MultiplayerServer.DesiredWidth;
            multiplayerServer.Height = MultiplayerServer.DesiredHeight;

            NavigationFrame.Content = multiplayerServer;
        }

        public void SwitchToServerQuitConfirmation()
        {
            Left += Width / 2 - ServerQuitConfirmation.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - ServerQuitConfirmation.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            serverQuitConfirmation.Width = ServerQuitConfirmation.DesiredWidth;
            serverQuitConfirmation.Height = ServerQuitConfirmation.DesiredHeight;

            NavigationFrame.Content = serverQuitConfirmation;
        }

        public void SwitchToMultiplayerClient()
        {
            Left += Width / 2 - MultiplayerClient.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - MultiplayerClient.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            multiplayerClient.Width = MultiplayerClient.DesiredWidth;
            multiplayerClient.Height = MultiplayerClient.DesiredHeight;

            NavigationFrame.Content = multiplayerClient;
        }

        public void SwitchToClientQuitConfirmation()
        {
            Left += Width / 2 - ClientQuitConfirmation.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - ClientQuitConfirmation.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            clientQuitConfirmation.Width = ClientQuitConfirmation.DesiredWidth;
            clientQuitConfirmation.Height = ClientQuitConfirmation.DesiredHeight;

            NavigationFrame.Content = clientQuitConfirmation;
        }

        public void SwitchToMultiplayerQuitConfirmation()
        {
            Left += Width / 2 - MultiplayerQuitConfirmation.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - MultiplayerQuitConfirmation.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            multiplayerQuitConfirmation.Width = MultiplayerQuitConfirmation.DesiredWidth;
            multiplayerQuitConfirmation.Height = MultiplayerQuitConfirmation.DesiredHeight;

            NavigationFrame.Content = multiplayerQuitConfirmation;
        }

        public void SwitchToAutomaticSetup()
        {
            Left += Width / 2 - AutomaticSetup.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - AutomaticSetup.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            automaticSetup.Width = AutomaticSetup.DesiredWidth;
            automaticSetup.Height = AutomaticSetup.DesiredHeight;

            NavigationFrame.Content = automaticSetup;
        }

        public void SwitchToSettings()
        {
            Left += Width / 2 - Settings.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - Settings.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            automaticSetup.Width = Settings.DesiredWidth;
            automaticSetup.Height = Settings.DesiredHeight;

            NavigationFrame.Content = settings;
        }

        public void SwitchToQuitConfirmation()
        {
            Left += Width / 2 - QuitConfirmation.DesiredWidth / 2 - NavigationFrame.Margin.Left;
            Top += Height / 2 - QuitConfirmation.DesiredHeight / 2 - NavigationFrame.Margin.Top;

            quitConfirmation.Width = QuitConfirmation.DesiredWidth;
            quitConfirmation.Height = QuitConfirmation.DesiredHeight;

            NavigationFrame.Content = quitConfirmation;
        }

        public void Recolour()
        {
            singleplayerSetup.Recolour();
            multiplayerSetup.Recolour();
            automaticSetup.Recolour();
            multiplayerServer.Recolour();
            multiplayerClient.Recolour();
        }

        public void OpenMultiplayerServer()
        {
            multiplayerServer.OpenServer();
        }

        public void CloseMultiplayer()
        {
            multiplayerServer.CloseServer();
        }

        public void JoinMultiplayerServer(string ip, string name)
        {
            multiplayerClient.JoinServer(ip, name);
        }

        public void LeaveMultiplayerServer()
        {
            multiplayerClient.LeaveServer();
        }

        public void SetServerSettings(string name, int discs, int poles, int bestOf)
        {
            multiplayerServer.UpdateDetails(name, discs, poles, bestOf);
            multiplayerClient.UpdateDetails(name, discs, poles, bestOf);
        }

        public void SetSelectedServerEntry(ServerEntry entry)
        {
            multiplayerMenu.SetSelectedEntry(entry);
        }
    }
}
