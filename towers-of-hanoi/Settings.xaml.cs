using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Page
    {
        public static int DesiredWidth = 450;
        public static int DesiredHeight = 600;

        public Settings()
        {
            InitializeComponent();
        }

        private void BackClicked(object sender, RoutedEventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMainMenu();
        }
    }
}
