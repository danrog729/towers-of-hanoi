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

namespace towers_of_hanoi.Navigation
{
    /// <summary>
    /// Interaction logic for MultiplayerServer.xaml
    /// </summary>
    public partial class MultiplayerServer : Page
    {
        public static int DesiredWidth = 800;
        public static int DesiredHeight = 450;

        public MultiplayerServer()
        {
            InitializeComponent();
        }

        public void OpenServer()
        {
            // send multicast signal announcing presence

            // start listening for multicast connections and respond if receives one
        }

        public void CloseServer()
        {
            // send multicast signal announcing leaving

            // stop listening for multicast connections
        }
    }
}
