using Networking.Cryptography;
using Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking.Client
{
    public class TalosClient
    {
        public delegate void ConnectedEvent();
        public event ConnectedEvent Connected;

        public delegate void ConnectionSecuredEvent();
        public event ConnectionSecuredEvent ConnectionSecured;

        private Encryption encryption;
        private Socket serverSocket;
        private Thread socketListenThread;

        public TalosClient(int keys)
        {
            encryption = new Encryption(keys);
            serverSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            socketListenThread = new Thread(() =>
            {
                while (true)
                    AcceptPacket();
            });
        }

        public bool Connect(string ip, int port = 1337)
        {
            int retries = 0;
            while (retries < 10)
                try
                {
                    serverSocket.Connect(new IPEndPoint(IPAddress.Parse(ip), port));
                    break;
                }
                catch (Exception e)
                {
                    retries += 1;
                    Log.Error($"Error connecting to Talos server re-tries {retries}/10");
                    Log.Error(e.Message);

                    if (retries == 10)
                    {
                        Log.Warning("Failed to connect to Talos server!");
                        return false;
                    }
                }

            socketListenThread.Start();
            new RSARequest(encryption.Keys.Length).Send(serverSocket);
            return true;
        }

        public void Drop()
        {
            serverSocket.Disconnect(false);
            socketListenThread.Abort();
        }

        public void AcceptPacket()
        {
            byte[] packetBuffer = new byte[serverSocket.Available];
            serverSocket.Receive(packetBuffer);

            if (encryption.isSecured)
                packetBuffer = encryption.DecryptPacket(packetBuffer);

            Packet packet = null;

            try
            {
                packet = Packet.Deserialize(packetBuffer);
            }
            catch (Exception e)
            {
                Log.Error("Error Deserializing packet");
                Log.Error(e.Message);
                return;
            }

            switch(packet.Type)
            {
                case 2:
                    RSAKeyExchange keyExchange = null;
                    if (encryption.GenerateKey((packet as RSAPublic).RSAPublicKey, out keyExchange))
                    {
                        keyExchange.Send(serverSocket);
                        new RSARequest(encryption.Keys.Length).Send(serverSocket);
                    }
                    break;

                case 4:
                    if (encryption.Verified(packet as SymmetricKeyVerification))
                    {
                        encryption.isSecured = true;
                        byte[] verificationData = Security.GenerateSecureKey(25);
                    }
                    else
                        Drop();
                    break;
            }
        }
    }
}