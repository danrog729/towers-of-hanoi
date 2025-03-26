using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using System.Diagnostics;

namespace towers_of_hanoi
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public Singleplayer singleplayer;
        public Multiplayer multiplayer;
        public Automatic automatic;
        public NavigationWindow navigationWindow;
        bool mainMenuOpened;

        public MainWindow()
        {
            InitializeComponent();

            singleplayer = new Singleplayer();
            multiplayer = new Multiplayer();
            automatic = new Automatic();
            ContentFrame.Content = singleplayer;
            navigationWindow = new NavigationWindow();
            navigationWindow.Hide();
            mainMenuOpened = false;
        }

        private void WindowClosing(object sender, EventArgs e)
        {
            navigationWindow.Close();
        }

        private void WindowKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                if (navigationWindow.Visibility == Visibility.Hidden)
                {
                    if (!mainMenuOpened)
                    {
                        navigationWindow.Owner = this;
                    }
                    App.MainApp.clickSound.Play();
                    navigationWindow.ShowDialog();
                }
                else
                {
                    navigationWindow.Hide();
                }
            }
        }

        public void SwitchToSingleplayer(int DiscCount, int PoleCount)
        {
            automatic.CancelBotPlay();
            singleplayer.NewSingleplayer(DiscCount, PoleCount);
            ContentFrame.Content = singleplayer;
            navigationWindow.mainMenu.SwitchedFromMultiplayer();
        }

        public void SwitchToMultiplayer(int DiscCount, int PoleCount)
        {
            automatic.CancelBotPlay();
            multiplayer.NewMultiplayer(DiscCount, PoleCount);
            ContentFrame.Content = multiplayer;
            navigationWindow.mainMenu.SwitchedToMultiplayer();
        }

        public void SwitchToAutomatic(int DiscCount, int PoleCount)
        {
            automatic.NewAutomatic(DiscCount, PoleCount);
            ContentFrame.Content = automatic;
            navigationWindow.mainMenu.SwitchedFromMultiplayer();
        }

        private void MenuClicked(object sender, EventArgs e)
        {
            if (navigationWindow.Visibility == Visibility.Hidden)
            {
                if (!mainMenuOpened)
                {
                    navigationWindow.Owner = this;
                }
                App.MainApp.clickSound.Play();
                navigationWindow.ShowDialog();
            }
            else
            {
                navigationWindow.Hide();
            }
        }
    }
}