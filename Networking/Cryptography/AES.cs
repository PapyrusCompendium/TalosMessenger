using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Networking.Cryptography
{
    public static class AES
    {
        public static void GenerateKeys(out byte[] key, out byte[] iv, int keySize = 256)
        {
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            aesProvider.KeySize = keySize;
            aesProvider.GenerateKey();
            aesProvider.GenerateIV();
            key = aesProvider.Key;
            iv = aesProvider.IV;
        }

        public static byte[] Encrypt(byte[] plainBytes, string password, int keySize = 256)
        {
            //128 bit block sizes
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            aesProvider.Mode = CipherMode.CBC;
            aesProvider.Padding = PaddingMode.PKCS7;
            aesProvider.KeySize = keySize;
            aesProvider.IV = Security.GenerateSecureKey(aesProvider.BlockSize / 8);

            //We are not saving this hash so we do not need a salt.
            aesProvider.Key = new PasswordDeriveBytes(password, System.Text.Encoding.UTF8.GetBytes("")).GetBytes(aesProvider.KeySize / 8);

            ICryptoTransform encrypter = aesProvider.CreateEncryptor();

            using (MemoryStream output = new MemoryStream())
            using (CryptoStream writer = new CryptoStream(output, encrypter, CryptoStreamMode.Write))
            {
                writer.Write(plainBytes, 0, plainBytes.Length);
                writer.FlushFinalBlock();
                byte[] Encrypted = new byte[output.Length + aesProvider.BlockSize / 8];
                Buffer.BlockCopy(aesProvider.IV, 0, Encrypted, 0, aesProvider.IV.Length);
                Buffer.BlockCopy(output.ToArray(), 0, Encrypted, aesProvider.IV.Length, output.ToArray().Length);
                return Encrypted;
            }
        }

        public static byte[] Decrypt(byte[] data, string password, int keySize = 256)
        {
            AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider();
            aesProvider.Mode = CipherMode.CBC;
            aesProvider.Padding = PaddingMode.PKCS7;
            aesProvider.KeySize = keySize;
            aesProvider.IV = data.Take(aesProvider.BlockSize / 8).ToArray();

            aesProvider.Key = new PasswordDeriveBytes(password, System.Text.Encoding.UTF8.GetBytes("")).GetBytes(aesProvider.KeySize / 8);

            byte[] decrypted = new byte[data.Length - aesProvider.BlockSize / 8];
            ICryptoTransform decrypter = aesProvider.CreateDecryptor();

            using (MemoryStream input = new MemoryStream(data.Skip(aesProvider.BlockSize / 8).ToArray()))
            using (CryptoStream reader = new CryptoStream(input, decrypter, CryptoStreamMode.Read))
            {
                reader.Read(decrypted, 0, decrypted.Length);
                return decrypted;
            }
        }
    }
}
