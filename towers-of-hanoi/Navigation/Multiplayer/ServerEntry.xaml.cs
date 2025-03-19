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

namespace towers_of_hanoi.Navigation.Multiplayer
{
    /// <summary>
    /// Interaction logic for ServerEntry.xaml
    /// </summary>
    public partial class ServerEntry : UserControl
    {
        private string _serverName = "server name";
        public string ServerName
        {
            get => _serverName;
            set
            {
                _serverName = value;
                ServerNameBox.Text = value;
            }
        }

        private string _ipAddress = "[PUT TCP ENDPOINT HERE]";
        public string IPAddress
        {
            get => _ipAddress;
            set
            {
                _ipAddress = value;
            }
        }

        private int _discs = 0;
        public int Discs
        {
            get => _discs;
            set
            {
                _discs = value;
                DiscCountBox.Text = "Discs: " + value;
            }
        }

        private int _poles = 0;
        public int Poles
        {
            get => _poles;
            set
            {
                _poles = value;
                PoleCountBox.Text = "Poles: " + value;
            }
        }

        private int _bestOf = 0;
        public int BestOf
        {
            get => _bestOf;
            set
            {
                _bestOf = value;
                BestOfBox.Text = "Best of: " + value;
            }
        }

        public ServerEntry()
        {
            InitializeComponent();
        }

        private void ThisEntryClicked(object sender, RoutedEventArgs e)
        {
            EntryClicked();
        }

        public void EntryClicked()
        {
            BackgroundGrid.Background = (Brush)App.MainApp.FindResource("FocusedForeground");
            ServerNameBox.Foreground = (Brush)App.MainApp.FindResource("FocusedText");
            DiscCountBox.Foreground = (Brush)App.MainApp.FindResource("FocusedText");
            PoleCountBox.Foreground = (Brush)App.MainApp.FindResource("FocusedText");
            BestOfBox.Foreground = (Brush)App.MainApp.FindResource("FocusedText");
            ((MainWindow)App.MainApp.MainWindow).navigationWindow.SetSelectedServerEntry(this);
        }

        public void EntryUnclicked()
        {
            BackgroundGrid.Background = (Brush)App.MainApp.FindResource("Foreground");
            ServerNameBox.Foreground = (Brush)App.MainApp.FindResource("StandardText");
            DiscCountBox.Foreground = (Brush)App.MainApp.FindResource("StandardText");
            PoleCountBox.Foreground = (Brush)App.MainApp.FindResource("StandardText");
            BestOfBox.Foreground = (Brush)App.MainApp.FindResource("StandardText");
        }
    }
}
