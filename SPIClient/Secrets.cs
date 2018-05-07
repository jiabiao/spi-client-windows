using System;
using System.Runtime.InteropServices;

namespace SPIClient
{
    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("4576C7BE-CE32-49C9-B86F-78C3D6A84F2F")]
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