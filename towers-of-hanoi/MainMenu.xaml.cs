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

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Page
    {
        public static int DesiredWidth = 450;
        public static int DesiredHeight = 600;

        public MainMenu()
        {
            InitializeComponent();
        }

        private void ResumeClicked(object sender, EventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();
        }

        private void SingleplayerClicked(object sender, EventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToSingleplayerSetup();
        }

        private void AutomaticClicked(object sender, EventArgs e)
        {
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToAutomaticSetup();
        }

        private void QuitClicked(object sender, EventArgs e)
        {
            App.MainApp.Shutdown();
        }
    }
}
