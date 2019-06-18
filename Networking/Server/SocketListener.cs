using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking.Server
{
    public class SocketListener
    {
        public List<ChatClient> chatClients = new List<ChatClient>();
        private Socket serverSocket;
        private Thread SocketListenThread;

        public SocketListener()
        {
            serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            SocketListenThread = new Thread(() =>
            {
                while (true)
                    AcceptSocket();
            });
        }

        public void Start(string ip, int port, int maxBackLog = 100)
        {
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(maxBackLog);

            SocketListenThread.Start();
        }

        public void Stop()
        {
            serverSocket.Shutdown(SocketShutdown.Both);
            SocketListenThread.Abort();
        }

        private void AcceptSocket()
        {
            Socket socket = serverSocket.Accept();

            Task.Run(() =>
            {
                chatClients.Add(new ChatClient(socket));
            });
        }
    }
}