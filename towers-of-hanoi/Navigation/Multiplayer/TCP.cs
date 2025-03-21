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
            using (Socket clientSocket = receivingSocket.Accept())
            {
                while (!worker.CancellationPending)
                {
                    if (clientSocket.Available > 0)
                    {
                        byte[] buffer = new byte[clientSocket.ReceiveBufferSize];
                        int bytesReceived = clientSocket.Receive(buffer);
                        string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                        Debug.WriteLine("Received message: " + message);
                    }

                    Thread.Sleep(10); // Prevent CPU overuse
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
            if (CanConnect && worker != null)
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
                string message = "Hello";
                sendingSocket.Send(Encoding.ASCII.GetBytes(message));
                Debug.WriteLine("Sent message: " + message);
            }
        }
    }
}
