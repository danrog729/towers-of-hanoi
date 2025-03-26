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
using System.Xml.Linq;

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

        private static string greetingMessage = "TOH:ESTABLISH_CONNECTION_";
        private static string leaveMessage = "TOH:TERMINATE_CONNECTION";
        private static string readyMessage = "TOH:READY";
        private static string moveMessage = "TOH:MOVE_";

        public static event EventHandler GreetingReceived = delegate { };
        public static event EventHandler LeaveMessageReceived = delegate { };
        public static event EventHandler ReadyMessageReceived = delegate { };
        public static event EventHandler MoveMessageReceived = delegate { };

        static TCP()
        {
            IPAddress? localIP = GetLocalIPAddress();

            if (localIP == null)
            {
                _canConnect = false;
            }
            else
            {
                receivingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                receivingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
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
            receivingSocket.Blocking = false;
            while (!worker.CancellationPending)
            {
                if (receivingSocket.Poll(100000, SelectMode.SelectRead))
                {
                    using (Socket clientSocket = receivingSocket.Accept())
                    {
                        bool shouldDisconnect = false;
                        while (!shouldDisconnect && !worker.CancellationPending)
                        {
                            if (clientSocket.Poll(100000, SelectMode.SelectRead))
                            {
                                if (clientSocket.Available == 0)
                                {
                                    shouldDisconnect = true;
                                }
                                else if (clientSocket.RemoteEndPoint != null)
                                {
                                    byte[] buffer = new byte[clientSocket.ReceiveBufferSize];
                                    int bytesReceived = clientSocket.Receive(buffer);
                                    string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                                    Debug.WriteLine("TCP: Received message: " + message);

                                    if (message.Contains(greetingMessage) && GreetingReceived != null)
                                    {
                                        string payload = message.Remove(0, greetingMessage.Length);
                                        string[] data = payload.Split("_");
                                        App.MainApp.Dispatcher.Invoke(() =>
                                        {
                                            GreetingReceived.Invoke((data[0], data[1]), new EventArgs());
                                        });
                                    }
                                    else if (message == leaveMessage && LeaveMessageReceived != null)
                                    {
                                        App.MainApp.Dispatcher.Invoke(() =>
                                        {
                                            LeaveMessageReceived.Invoke(null, new EventArgs());
                                        });
                                    }
                                    else if (message == readyMessage && ReadyMessageReceived != null)
                                    {
                                        App.MainApp.Dispatcher.Invoke(() =>
                                        {
                                            ReadyMessageReceived.Invoke(null, new EventArgs());
                                        });
                                    }
                                    else if (message.Contains(moveMessage) && MoveMessageReceived != null)
                                    {
                                        string payload = message.Remove(0, moveMessage.Length);
                                        string[] data = payload.Split("_");
                                        if (Int32.TryParse(data[0], out int moveFrom) && Int32.TryParse(data[1], out int moveTo) && data.Length == 3)
                                        {
                                            App.MainApp.Dispatcher.Invoke(() =>
                                            {
                                                MoveMessageReceived.Invoke((moveFrom, moveTo, data[2]), new EventArgs());
                                            });
                                        }
                                    }
                                }
                            }
                            else
                            {
                                Thread.Sleep(10);
                            }
                        }
                        clientSocket.Shutdown(SocketShutdown.Both);
                        clientSocket.Close();
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

        public static void Connect(string ip, string name)
        {
            if (CanConnect)
            {
                sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                string[] data = ip.Split(':');
                if (IPAddress.TryParse(data[0], out IPAddress? address) && address != null &&
                    Int32.TryParse(data[1], out int port))
                {
                    remoteEndPoint = new IPEndPoint(address, port);
                    sendingSocket.Connect(remoteEndPoint);
                    SendGreeting(name);
                }
            }
        }

        public static void Disconnect()
        {
            if (sendingSocket != null)
            {
                SendLeaveMessage();
                sendingSocket.Shutdown(SocketShutdown.Both);
                sendingSocket.Close();
            }
        }

        public static void SendGreeting(string name)
        {
            if (CanConnect && sendingSocket != null && remoteEndPoint != null && localEndPoint != null)
            {
                sendingSocket.Send(Encoding.ASCII.GetBytes(greetingMessage + localEndPoint.ToString() + "_" + name));
                Debug.WriteLine("TCP: Sent message: " + greetingMessage + name);
            }
        }

        public static void SendLeaveMessage()
        {
            if (CanConnect && sendingSocket != null && remoteEndPoint != null)
            {
                sendingSocket.Send(Encoding.ASCII.GetBytes(leaveMessage));
                Debug.WriteLine("TCP: Sent message: " + leaveMessage);
            }
        }

        public static void SendReadyMessage()
        {
            if (CanConnect && sendingSocket != null && remoteEndPoint != null)
            {
                sendingSocket.Send(Encoding.ASCII.GetBytes(readyMessage));
                Debug.WriteLine("TCP: Sent message: " + readyMessage);
            }
        }

        public static void SendMove(int moveFrom, int moveTo, string time)
        {
            if (CanConnect && sendingSocket != null && remoteEndPoint != null)
            {
                sendingSocket.Send(Encoding.ASCII.GetBytes(moveMessage + moveFrom.ToString() + "_" + moveTo.ToString() + "_" + time));
                Debug.WriteLine("TCP: Sent message: " + moveMessage + moveFrom.ToString() + "_" + moveTo.ToString() + "_" + time);
            }
        }
    }
}
