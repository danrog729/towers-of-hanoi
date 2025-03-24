using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace towers_of_hanoi.Navigation.Multiplayer
{
    static class TCP
    {
        private static Socket? sendingSocket;
        private static Socket? receivingSocket;
        private static IPEndPoint? localEndPoint;
        private static IPEndPoint? remoteEndPoint;

        private static bool _canConnect = true;
        public static bool CanConnect
        {
            get => _canConnect;
            set
            {
                return;
            }
        }

        private static BackgroundWorker? worker;

        private static string greetingMessage = "TOH:ESTABLISH_CONNECTION";

        public static event EventHandler GreetingReceived = delegate { };

        static TCP()
        {
            IPAddress? localIP = GetLocalIPAddress();

            if (localIP == null)
            {
                _canConnect = false;
            }
            else
            {
                sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                receivingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                Random random = new Random();
                localEndPoint = new IPEndPoint(localIP, random.Next(3690,4105));
                receivingSocket.Bind(localEndPoint);

                worker = new BackgroundWorker();
                worker.DoWork += StartListening;
                worker.WorkerSupportsCancellation = true;
            }
        }

        private static IPAddress? GetLocalIPAddress()
        {
            foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                // Ignore virtual, loopback, and non-operational interfaces
                if (ni.OperationalStatus == OperationalStatus.Up &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    ni.NetworkInterfaceType != NetworkInterfaceType.Tunnel &&
                    !ni.Description.ToLower().Contains("virtual") &&  // Ignore vEthernet adapters
                    !ni.Description.ToLower().Contains("vpn"))       // Ignore VPNs
                {
                    foreach (UnicastIPAddressInformation ip in ni.GetIPProperties().UnicastAddresses)
                    {
                        if (ip.Address.AddressFamily == AddressFamily.InterNetwork) // IPv4 only
                        {
                            return ip.Address; // Found a valid local IP
                        }
                    }
                }
            }
            return null;
        }

        public static string GetIP()
        {
            if (localEndPoint != null)
            {
                return localEndPoint.ToString();
            }
            else
            {
                return "NULL_IP";
            }
        }

        private static void StartListening(object? sender, DoWorkEventArgs e)
        {
            if (!CanConnect || receivingSocket == null || localEndPoint == null || worker == null)
            {
                return;
            }
            receivingSocket.Listen(100);
            while (!worker.CancellationPending)
            {
                if (receivingSocket.Poll(100000, SelectMode.SelectRead))
                {
                    using (Socket clientSocket = receivingSocket.Accept())
                    {
                        if (clientSocket.Available > 0 && clientSocket.RemoteEndPoint != null)
                        {
                            byte[] buffer = new byte[clientSocket.ReceiveBufferSize];
                            int bytesReceived = clientSocket.Receive(buffer);
                            string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                            Debug.WriteLine("TCP: Received message: " + message);

                            if (message == greetingMessage && GreetingReceived != null)
                            {
                                App.MainApp.Dispatcher.Invoke(() =>
                                {
                                    GreetingReceived.Invoke(clientSocket.RemoteEndPoint.ToString(), new EventArgs());
                                });
                            }
                        }
                    }
                }
            }
        }

        public static void StartServer()
        {
            if (CanConnect && worker != null)
            {
                worker.RunWorkerAsync();
            }
        }

        public static void CloseServer()
        {
            if (worker != null)
            {
                worker.CancelAsync();
            }
        }

        public static void Connect(string ip)
        {
            if (CanConnect && sendingSocket != null)
            {
                string[] data = ip.Split(':');
                if (IPAddress.TryParse(data[0], out IPAddress? address) && address != null &&
                    Int32.TryParse(data[1], out int port))
                {
                    remoteEndPoint = new IPEndPoint(address, port);
                    sendingSocket.Connect(remoteEndPoint);
                    SendGreeting();
                }
            }
        }

        private static void SendGreeting()
        {
            if (CanConnect && sendingSocket != null && remoteEndPoint != null)
            {
                sendingSocket.Send(Encoding.ASCII.GetBytes(greetingMessage));
                Debug.WriteLine("TCP: Sent message: " + greetingMessage);
            }
        }
    }
}
