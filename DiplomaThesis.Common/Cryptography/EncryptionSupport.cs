using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace DiplomaThesis.Common.Cryptography
{
    public class EncryptionSupport : IEncryptionSupport
    {
        private static readonly Lazy<IEncryptionSupport> instance = new Lazy<IEncryptionSupport>(() => new EncryptionSupport()); // thread-safe singleton
        private static readonly byte[] KEY = new byte[]{ 94, 2, 70, 75, 18, 135, 130, 155, 24, 190, 33, 17, 18, 243, 11, 26, 81, 11, 236, 168, 10, 220, 30, 206, 118, 64, 94, 78, 147, 246, 132, 2 };
        private static readonly byte[] IV = new byte[] { 226, 203, 152, 117, 5, 23, 189, 246, 126, 61, 78, 204, 99, 44, 181, 51 };

        public static IEncryptionSupport Instance { get { return instance.Value; } }
        private EncryptionSupport()
        {

        }

        public string Encrypt(string password)
        {
            using (var aes = new AesManaged())
            {
                aes.Key = KEY;
                aes.IV = IV;
                using (ICryptoTransform encryptor = aes.CreateEncryptor())
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                        {
                            using (StreamWriter writer = new StreamWriter(cryptoStream))
                            {
                                writer.Write(password);
                                writer.Flush();
                            }
                            return Convert.ToBase64String(memoryStream.ToArray());
                        }
                    } 
                }
            }
        }

        public string Decrypt(string cipher)
        {
            var data = Convert.FromBase64String(cipher);
            using (var aes = new AesManaged())
            {
                aes.Key = KEY;
                aes.IV = IV;
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                {
                    using (var memoryStream = new MemoryStream(data))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cryptoStream))
                            {
                                return reader.ReadToEnd();
                            }
                        }
                    }
                }
            }
        }
    }
}
