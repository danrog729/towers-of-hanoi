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
    /// Interaction logic for MultiplayerClient.xaml
    /// </summary>
    public partial class MultiplayerClient : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        private string serverName;
        private int discs;
        private int poles;
        private int bestOf;

        private Scene3D discScene;

        public MultiplayerClient()
        {
            InitializeComponent();
            serverName = "server name";
            discScene = new Scene3D(Viewport);
            discScene.Reset(6, 3, 0, 2f);
        }

        private void QuitClicked(object sender, RoutedEventArgs e)
        {

        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {

        }

        public void UpdateDetails(string Name, int Discs, int Poles, int BestOf)
        {
            serverName = Name;
            discs = Discs;
            poles = Poles;
            bestOf = BestOf;

            // update textboxes and viewport and stuff
            discScene.Reset(discs, poles, 0, 2);
            DiscBox.Text = "Discs: " + discs;
            PoleBox.Text = "Poles: " + poles;
            BestOfBox.Text = "Best of: " + bestOf;
        }

        public void Recolour()
        {
            discScene.Recolour();
        }
    }
}
