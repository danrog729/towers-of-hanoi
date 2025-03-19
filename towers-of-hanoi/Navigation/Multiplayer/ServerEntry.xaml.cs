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
                IPAddressBox.Text = value;
            }
        }

        public ServerEntry()
        {
            InitializeComponent();
        }
    }
}
