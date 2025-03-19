using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;

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

            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            sendingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multiCastIP));
            sendingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            sendingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            sendingEndpoint = new IPEndPoint(multiCastIP, 12345);

            receivingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            receivingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multiCastIP));
            receivingSocket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);
            receivingEndpoint = (EndPoint)new IPEndPoint(IPAddress.Any, 12345);
            receivingSocket.Bind(receivingEndpoint);

            multiCastListener = new BackgroundWorker();
            multiCastListener.DoWork += StartListening;
            multiCastListener.WorkerSupportsCancellation = true;
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
                        string ip = split[0];
                        int discs = 0;
                        int poles = 0;
                        if (!Int32.TryParse(split[1], out discs))
                        {
                            return;
                        }
                        if (!Int32.TryParse(split[2], out poles))
                        {
                            return;
                        }
                        App.MainApp.Dispatcher.Invoke(() =>
                        {
                            ServerResponseMessageReceived.Invoke((ip, discs, poles), new EventArgs());
                        });
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
        }

        public static void SendServerRequest()
        {
            // send message
            byte[] messageArray = Encoding.ASCII.GetBytes(requestMessage);
            sendingSocket.SendTo(messageArray, sendingEndpoint);
            Debug.WriteLine("Sent message: " + requestMessage);
        }

        public static void SendServerResponse(int discs, int poles)
        {
            // send message
            string message = responseMessage + "[PUT_TCP_ENDPOINT_HERE]" + "_" + discs.ToString() + "_" + poles.ToString();
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            sendingSocket.SendTo(messageArray, sendingEndpoint);
            Debug.WriteLine("Sent message: " + message);
        }

        public static void SendServerResignment()
        {
            // send message
            string message = resignmentMessage + "[PUT_TCP_ENDPOINT_HERE]";
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            sendingSocket.SendTo(messageArray, sendingEndpoint);
            Debug.WriteLine("Sent message: " + message);
        }
    }
}
