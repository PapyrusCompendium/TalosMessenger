using Networking.Cryptography;
using Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Networking.Server
{
    public class ChatClient
    {
        public ulong ID { get; }
        public string Username { get; }
        public Socket ClientSocket { get; }
        private Encryption encryption;
        private Thread listenThread;
        private RSAParameters privateKey;

        public ChatClient(Socket socket)
        {
            //An ID ensures that a user can NOT effectivly spoof a user's username, as an ID that differs will also be shown
            //The ID is comprised of the date it was created in ulong format. It also Supports 3 variabls of 5(a), 5(b), 12(c) bits.
            //These variables can be used for any arbitrary data (that fits).
            ID = Security.GenerateSnowFlakeID();

            ClientSocket = socket;

            //Create a Thread for listening on, and start it after.
            listenThread = new Thread(() =>
            {
                while (ClientSocket.Connected)
                    AcceptPacket();
            });

            listenThread.Start();
        }

        public void Drop()
        {
            ClientSocket.Disconnect(false);
            listenThread.Abort();
        }

        private void AcceptPacket()
        {
            byte[] packetBuffer = new byte[ClientSocket.Available];
            ClientSocket.Receive(packetBuffer);

            if (packetBuffer.Length == 0)
                return;

            if (encryption.isSecured)
                packetBuffer = encryption.DecryptPacket(packetBuffer);

            Packet packet = null;

            try
            {
                packet = Packet.Deserialize(packetBuffer);
            }
            catch(Exception e)
            {
                Log.Error("Error Deserializing packet");
                Log.Error(e.Message);
                return;
            }

            //If our packet type is not an initial stage packet
            if (packet.Type > 4)
                PacketManager.ManagePacket(packet, this);
            else
            {
                switch(packet.Type)
                {
                    //RSARequest Packet, Send a public key if there are any unknown keys left.
                    case 1:
                        if (encryption == null)
                            encryption = new Encryption((packet as RSARequest).Keys);

                        if (encryption.Keys.Any(i => i == string.Empty || i == null))
                            new RSAPublic(Cryptography.RSA.GeneratePublic(out privateKey)).Send(ClientSocket);
                        break;

                    //RSAKeyExchange Packet
                    case 3:
                        encryption.Keys[(packet as RSAKeyExchange).KeyIndex] = Encoding.UTF8.GetString(Cryptography.RSA.Decrypt((packet as RSAKeyExchange).SymmetricKey, privateKey));
                        break;

                    //Verification Packet, If verified send one to the client | Otherwise Drop the connection.
                    case 4:
                        if (encryption.Verified(packet as SymmetricKeyVerification))
                        {
                            encryption.isSecured = true;
                            byte[] verificationData = Security.GenerateSecureKey(25);
                            new SymmetricKeyVerification(encryption.EncryptPacket(verificationData), Security.Sha512(verificationData));
                        }
                        else
                            Drop();
                        break;
                }
            }
        }
    }
}
