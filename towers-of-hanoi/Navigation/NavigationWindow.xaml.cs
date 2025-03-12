using System;
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
        private AutomaticSetup automaticSetup;
        private Settings settings;
        private QuitConfirmation quitConfirmation;

        public NavigationWindow()
        {
            InitializeComponent();
            mainMenu = new MainMenu();
            singleplayerSetup = new SingleplayerSetup();
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
                if (NavigationFrame.Content == mainMenu)
                {
                    Hide();
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
            automaticSetup.Recolour();
        }
    }
}
