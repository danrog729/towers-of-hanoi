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
        private static IPEndPoint receivingEndpoint;

        private static BackgroundWorker multiCastListener;

        public static event EventHandler ServerExistsMessageReceived = delegate { };

        static MultiCast()
        {
            sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            receivingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            multiCastIP = IPAddress.Parse("239.1.1.1");
            sendingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multiCastIP));
            sendingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.MulticastTimeToLive, 2);
            receivingSocket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.AddMembership, new MulticastOption(multiCastIP));
            sendingEndpoint = new IPEndPoint(multiCastIP, 12345);
            receivingEndpoint = new IPEndPoint(multiCastIP, 12345);

            multiCastListener = new BackgroundWorker();
            multiCastListener.DoWork += StartListening;
            multiCastListener.WorkerSupportsCancellation = true;
        }

        public static void Connect()
        {
            multiCastListener.RunWorkerAsync();
        }

        public static void Disconnect()
        {
            multiCastListener.CancelAsync();
        }

        private static void StartListening(object? sender, DoWorkEventArgs e)
        {
            bool shouldFinish = false;
            while (!shouldFinish)
            {

                byte[] buffer = new byte[receivingSocket.ReceiveBufferSize];
                if (receivingSocket.Available != 0)
                {
                    int bytesReceived = receivingSocket.SendTo(buffer, sendingEndpoint);
                    Debug.WriteLine("Received message: " + buffer.ToString());
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
            string message = "TOH:REQUEST_SERVERS";
            byte[] messageArray = Encoding.ASCII.GetBytes(message);
            sendingSocket.SendTo(messageArray, sendingEndpoint);
            Debug.WriteLine("Sent message: TOH_REQUEST_SERVERS");
        }
    }
}
