using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Packets
{
    [Serializable]
    [PacketType("RSARequest", 1)]
    public class RSARequest : Packet
    {
        public int Keys { get; }
        public RSARequest(int keys)
        {
            Keys = keys;
            Type = 1;
        }
    }

    [Serializable]
    [PacketType("RSAPublic", 2)]
    public class RSAPublic : Packet
    {
        public RSAParameters RSAPublicKey { get; }

        public RSAPublic(RSAParameters publicKey)
        {
            Type = 2;
            RSAPublicKey = publicKey;
        }
    }

    [Serializable]
    [PacketType("RSAKeyExchange", 3)]
    public class RSAKeyExchange : Packet
    {
        public int KeyIndex { get; }
        public byte[] SymmetricKey { get; }
        public RSAKeyExchange(byte[] key)
        {
            Type = 3;
            SymmetricKey = key;
        }
    }

    [Serializable]
    [PacketType("SymmetricKeyVerification", 4)]
    public class SymmetricKeyVerification : Packet
    {
        public byte[] TestData { get; }
        public string Checksum { get; }
        public SymmetricKeyVerification(byte[] data, string checksum)
        {
            Type = 4;
            TestData = data;
            Checksum = checksum;
        }
    }
}