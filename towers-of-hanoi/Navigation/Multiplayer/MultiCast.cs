using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.NetworkInformation;

namespace towers_of_hanoi.Navigation.Multiplayer
{
    static class MultiCast
    {
        private static Socket sendingSocket;
        private static Socket receivingSocket;
        private static IPAddress multiCastIP;
        private static IPEndPoint sendingEndpoint;
        private static EndPoint receivingEndpoint;

        private static string requestMessage = "TOH:REQUEST_SERVERS";
        private static string responseMessage = "TOH:SERVER_OPEN_AT_";
        private static string resignmentMessage = "TOH:SERVER_CLOSED_AT_";

        private static BackgroundWorker multiCastListener;

        public static event EventHandler ServerRequestMessageReceived = delegate { };
        public static event EventHandler ServerResponseMessageReceived = delegate { };
        public static event EventHandler ServerResignmentMessageReceived = delegate { };

        static MultiCast()
        {
            multiCastIP = IPAddress.Parse("224.0.2.0");
            IPAddress localIP = GetLocalIPAddress();

            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multiCastIP));
            sendingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastInterface, localIP.GetAddressBytes());
            sendingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            sendingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sendingEndpoint = new IPEndPoint(multiCastIP, 12345);

            receivingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            receivingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multiCastIP, localIP));
            receivingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receivingEndpoint = (EndPoint)new IPEndPoint(localIP, 12345);
            receivingSocket.Bind(receivingEndpoint);
            receivingSocket.ReceiveBufferSize = 0;

            multiCastListener = new BackgroundWorker();
            multiCastListener.DoWork += StartListening;
            multiCastListener.WorkerSupportsCancellation = true;
        }

        private static IPAddress GetLocalIPAddress()
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
            throw new Exception("No active Ethernet connection found.");
        }

        public static void Connect()
        {
            if (!multiCastListener.IsBusy)
            {
                multiCastListener.RunWorkerAsync();
            }
        }

        public static void Disconnect()
        {
            if (multiCastListener.IsBusy)
            {
                multiCastListener.CancelAsync();
            }
        }

        private static void StartListening(object? sender, DoWorkEventArgs e)
        {
            bool shouldFinish = false;
            receivingSocket.ReceiveBufferSize = 8192;
            while (!shouldFinish)
            {

                byte[] buffer = new byte[receivingSocket.ReceiveBufferSize];
                if (receivingSocket.Available != 0)
                {
                    int bytesReceived = receivingSocket.Receive(buffer);
                    string message = Encoding.ASCII.GetString(buffer, 0, bytesReceived);
                    Debug.WriteLine("Received message: " + message);
                    
                    if (message == requestMessage && ServerRequestMessageReceived != null)
                    {
                        App.MainApp.Dispatcher.Invoke(() =>
                        {
                            ServerRequestMessageReceived.Invoke(null, new EventArgs());
                        });
                    }
                    else if (message.Contains(responseMessage) && ServerResponseMessageReceived != null)
                    {
                        string payload = message.Remove(0, responseMessage.Length);
                        string[] split = payload.Split("_");
                        if (split.Length == 5)
                        {
                            string ip = split[0];
                            string name = split[1];
                            int discs = 0;
                            int poles = 0;
                            int bestOf = 0;
                            if (Int32.TryParse(split[2], out discs) && 
                                Int32.TryParse(split[3], out poles) && 
                                Int32.TryParse(split[4], out bestOf))
                            {
                                App.MainApp.Dispatcher.Invoke(() =>
                                {
                                    ServerResponseMessageReceived.Invoke((ip, name, discs, poles, bestOf), new EventArgs());
                                });
                            }
                        }
                    }
                    else if (message.Contains(resignmentMessage) && ServerResignmentMessageReceived != null)
                    {
                        string ip = message.Remove(0, resignmentMessage.Length);
                        App.MainApp.Dispatcher.Invoke(() =>
                        {
                            ServerResignmentMessageReceived.Invoke(ip, new EventArgs());
                        });
                    }
                }

                if (multiCastListener.CancellationPending)
                {
                    shouldFinish = true;
                }
            }
            receivingSocket.ReceiveBufferSize = 0;
        }

        public static void SendServerRequest()
        {
            // send message
            byte[] messageArray = Encoding.ASCII.GetBytes(requestMessage);
            sendingSocket.SendTo(messageArray, sendingEndpoint);
            Debug.WriteLine("Sent message: " + requestMessage);
        }

        public static void SendServerResponse(string name, int discs, int poles, int bestOf)
        {
            // send message
            string message = responseMessage + "[PUT TCP ENDPOINT HERE]" + "_" + name + "_" + discs.ToString() + "_" + poles.ToString() + "_" + bestOf.ToString();
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            sendingSocket.SendTo(messageArray, sendingEndpoint);
            Debug.WriteLine("Sent message: " + message);
        }

        public static void SendServerResignment()
        {
            // send message
            string message = resignmentMessage + "[PUT TCP ENDPOINT HERE]";
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            sendingSocket.SendTo(messageArray, sendingEndpoint);
            Debug.WriteLine("Sent message: " + message);
        }
    }
}
