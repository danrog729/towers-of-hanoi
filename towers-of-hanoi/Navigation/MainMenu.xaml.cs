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

        private bool InMultiplayer;

        public MainMenu()
        {
            InitializeComponent();
            InMultiplayer = false;
        }

        private void ResumeClicked(object sender, EventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.Hide();
        }

        private void SingleplayerClicked(object sender, EventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToSingleplayerSetup();
        }

        private void MultiplayerClicked(object sender, EventArgs e)
        {
            App.MainApp.clickSound.Play();
            if (InMultiplayer)
            {
                // show confirmation menu
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerQuitConfirmation();
            }
            else
            {
                ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToMultiplayerMenu();
            }
        }

        private void AutomaticClicked(object sender, EventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToAutomaticSetup();
        }

        private void SettingsClicked(object sender, EventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToSettings();
        }

        private void QuitClicked(object sender, EventArgs e)
        {
            App.MainApp.clickSound.Play();
            ((MainWindow)(App.MainApp.MainWindow)).navigationWindow.SwitchToQuitConfirmation();
        }

        public void SwitchedToMultiplayer()
        {
            InMultiplayer = true;
            MultiplayerButton.Content = "Leave Game";
        }

        public void SwitchedFromMultiplayer()
        {
            InMultiplayer = false;
            MultiplayerButton.Content = "Multiplayer";
        }
    }
}
