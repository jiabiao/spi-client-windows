using System;
using System.Runtime.InteropServices;

namespace SPIClient
{
    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("EAD2447D-CEFF-4DC8-970A-57F5B3F1D489")]
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