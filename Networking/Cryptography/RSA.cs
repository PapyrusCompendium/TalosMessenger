using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Cryptography
{
    public static class RSA
    {
        public static RSAParameters GeneratePublic(out RSAParameters privateKey)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(4096);

            privateKey = rsaProvider.ExportParameters(true);
            return rsaProvider.ExportParameters(false);
        }

        public static byte[] Decrypt(byte[] encrypted, RSAParameters privateKey)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(4096);
            rsaProvider.ImportParameters(privateKey);
            return rsaProvider.Decrypt(encrypted, false);
        }

        public static byte[] Encrypt(byte[] data, RSAParameters publicKey)
        {
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(4096);
            rsaProvider.ImportParameters(publicKey);
            return rsaProvider.Encrypt(data, false);
        }
    }
}
