using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Cryptography
{
    public static class Security
    {
        public static string Sha512(byte[] data)
        {
            SHA512Managed sha512Provider = new SHA512Managed();
            return BitConverter.ToString(sha512Provider.ComputeHash(data)).Replace("-", "");
        }

        public static byte[] GenerateSecureKey(int length)
        {
            RNGCryptoServiceProvider secureRandom = new RNGCryptoServiceProvider();
            byte[] secureBytes = new byte[length];
            secureRandom.GetNonZeroBytes(secureBytes);
            return secureBytes;
        }

        public static ulong GenerateSnowFlakeID(ulong a = 1, ulong b = 0, ulong c = 1)
        {
            ulong Bits = (ulong)(DateTime.Now - new DateTime(1970, 1, 1)).TotalMilliseconds;
            ulong ID = 0;
            ID |= (Bits) << 22;
            ID |= (a) << 17;
            ID |= (b) << 12;
            ID |= (c);

            return ID;
        }
    }
}
