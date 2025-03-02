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

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for NavigationWindow.xaml
    /// </summary>
    public partial class NavigationWindow : Window
    {
        private MainMenu mainMenu;
        private SingleplayerSetup singleplayerSetup;

        public NavigationWindow()
        {
            InitializeComponent();
            mainMenu = new MainMenu();
            singleplayerSetup = new SingleplayerSetup();
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
                if (NavigationFrame.Content == mainMenu)
                {
                    Hide();
                }
                else if (NavigationFrame.Content == singleplayerSetup)
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
    }
}
