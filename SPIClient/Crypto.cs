using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace SPIClient
{
    public static class Crypto
    {

        /// <summary>
        /// Decrypt a block using a <see cref="CipherMode"/> of CBC and a <see cref="PaddingMode"/> of PKCS7.
        /// </summary>
        /// <param name="key">The key value</param>
        /// <param name="encMessage">the message to decrypt</param>
        /// <returns>Returns the resulting plaintext data.</returns>
        public static string AesDecrypt(byte[] key, string encMessage)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var inputBuffer = HexStringToByteArray(encMessage);
                
            using (var acp = new AesCryptoServiceProvider())
            {
                acp.Key = key;
                acp.IV = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                acp.Mode = CipherMode.CBC;
                acp.Padding = PaddingMode.PKCS7;

                var decryptor = acp.CreateDecryptor();
                var plainTextBytes = decryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

                var retStr = Encoding.UTF8.GetString(plainTextBytes);
                return retStr;
            }
        }

        /// <summary>
        /// Encrypt a block using a <see cref="CipherMode"/> of CBC and a <see cref="PaddingMode"/> of PKCS7.
        /// </summary>
        /// <param name="key">The key value</param>
        /// <param name="message">The message to encrypt</param>
        /// <returns>Returns the resulting ciphertext data.</returns>
        public static string AesEncrypt(byte[] key, string message)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }

            var inputBuffer = Encoding.UTF8.GetBytes(message);
            using (var acp = new AesCryptoServiceProvider())
            {
                acp.Key = key;
                acp.IV = new byte[] { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
                acp.Mode = CipherMode.CBC;
                acp.Padding = PaddingMode.PKCS7;

                var encryptor = acp.CreateEncryptor();
                var cipherTextBytes = encryptor.TransformFinalBlock(inputBuffer, 0, inputBuffer.Length);

                return ByteArrayToHexString(cipherTextBytes);
            }
        }
        
        /// <summary>
        /// Calculates the HMACSHA256 signature of a message.
        /// </summary>
        /// <param name="key">The Hmac Key as Bytes</param>
        /// <param name="messageToSign">The message to sign</param>
        /// <returns>The HMACSHA256 signature as a hex string</returns>
        public static string HmacSignature(byte[] key, string messageToSign)
        {
            var msgBytes = Encoding.UTF8.GetBytes(messageToSign);
            var hash = new HMACSHA256(key);
            return ByteArrayToHexString(hash.ComputeHash(msgBytes));
        }

        public static string ByteArrayToHexString(byte[] ba)
        {
            var hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }

        public static byte[] HexStringToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                .ToArray();
        }

    }
    
}