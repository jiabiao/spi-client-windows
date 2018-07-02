using System;
using System.Runtime.InteropServices;

namespace SPIClient
{
    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("10EF6BEC-9248-4530-A510-24D37AF53CB2")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class Secrets : EventArgs
    {
        public string EncKey { get; }
        public string HmacKey { get; }

        public byte[] EncKeyBytes { get; }
        public byte[] HmacKeyBytes { get; }

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public Secrets() { }

        public Secrets(string encKey, string hmacKey)
        {
            EncKey = encKey;
            HmacKey = hmacKey;

            EncKeyBytes = Crypto.HexStringToByteArray(encKey);
            HmacKeyBytes = Crypto.HexStringToByteArray(hmacKey);
        }
        
    }

    
}