using Networking.Packets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Cryptography
{
    public class Encryption
    {
        public string[] Keys { get; set; }
        public bool isSecured { get; set; }

        public int lastGeneratedKey;
        public Encryption(int keys)
        {
            Keys = new string[keys];
            lastGeneratedKey = 0;
        }

        public bool Verified(Packets.SymmetricKeyVerification packet) => Security.Sha512(DecryptPacket(packet.TestData)) == packet.Checksum;
        private RSAKeyExchange EncryptKey(string key, RSAParameters publicKey) => new RSAKeyExchange(RSA.Encrypt(Encoding.UTF8.GetBytes(key), publicKey));

        public bool GenerateKey(RSAParameters publicKey, out RSAKeyExchange Exchange)
        {
            Exchange = null;

            if (lastGeneratedKey >= Keys.Length)
                return false;

            Keys[lastGeneratedKey] = Encoding.UTF8.GetString(Security.GenerateSecureKey(32));
            Exchange = EncryptKey(Keys[lastGeneratedKey], publicKey);

            lastGeneratedKey++;
            return true;
        }

        public byte[] DecryptPacket(byte[] buffer)
        {
            for (int x = 0; x < Keys.Length; x++)
                buffer = AES.Decrypt(buffer, Keys[x]);

            return buffer;
        }

        public byte[] EncryptPacket(byte[] buffer)
        {
            for (int x = 0; x < Keys.Length; x++)
                buffer = AES.Encrypt(buffer, Keys[x]);

            return buffer;
        }
    }
}
