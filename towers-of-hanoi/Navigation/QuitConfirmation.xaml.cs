﻿using System;
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

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for QuitConfirmation.xaml
    /// </summary>
    public partial class QuitConfirmation : Page
    {
        public static int DesiredWidth = 400;
        public static int DesiredHeight = 200;

        public QuitConfirmation()
        {
            InitializeComponent();
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {
            App.MainApp.clickSound.Play();
            App.MainApp.Shutdown();
        }
    }
}
