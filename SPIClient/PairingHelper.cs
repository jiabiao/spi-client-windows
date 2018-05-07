using System.Globalization;
using System.Numerics;
using System.Security.Cryptography;

namespace SPIClient
{

    /// <summary>
    /// This static class helps you with the pairing process as documented here:
    /// http://www.simplepaymentapi.com/#/api/pairing-process
    /// </summary>
    public static class PairingHelper
    {

        /// <summary>
        /// Generates a pairing Request.
        /// </summary>
        /// <returns>New PairRequest</returns>
        public static PairRequest NewPairequest()
        {
            return new PairRequest();
        }
        
        /// <summary>
        /// Calculates/Generates Secrets and KeyResponse given an incoming KeyRequest.
        /// </summary>
        /// <param name="keyRequest"></param>
        /// <returns>Secrets and KeyResponse to send back.</returns>
        public static SecretsAndKeyResponse GenerateSecretsAndKeyResponse(KeyRequest keyRequest)
        {
            var encPubAndSec = _calculateMyPublicKeyAndSecret(keyRequest.Aenc);
            var Benc = encPubAndSec.MyPublicKey;
            var Senc = encPubAndSec.SharedSecretKey;

            var hmacPubAndSec = _calculateMyPublicKeyAndSecret(keyRequest.Ahmac);
            var Bhmac = hmacPubAndSec.MyPublicKey;
            var Shmac = hmacPubAndSec.SharedSecretKey;
            
            var secrets = new Secrets(Senc, Shmac);
            var keyResponse = new KeyResponse(keyRequest.RequestId, Benc, Bhmac);

            return new SecretsAndKeyResponse(secrets, keyResponse);
        }
        
        /// <summary>
        /// Turns an incoming "A" value from the PinPad into the outgoing "B" value 
        /// and the secret value using DiffieHelmman helper.
        /// </summary>
        /// <param name="theirPublicKey">The incoming A value</param>
        /// <returns>Your B value and the Secret</returns>
        private static PublicKeyAndSecret _calculateMyPublicKeyAndSecret(string theirPublicKey)
        {
            // SPI uses the 2048-bit MODP Group as the shared constants for the DH algorithm
            // https://tools.ietf.org/html/rfc3526#section-3
            var modp2048P = BigInteger.Parse("32317006071311007300338913926423828248817941241140239112842009751400741706634354222619689417363569347117901737909704191754605873209195028853758986185622153212175412514901774520270235796078236248884246189477587641105928646099411723245426622522193230540919037680524235519125679715870117001058055877651038861847280257976054903569732561526167081339361799541336476559160368317896729073178384589680639671900977202194168647225871031411336429319536193471636533209717077448227988588565369208645296636077250268955505928362751121174096972998068410554359584866583291642136218231078990999448652468262416972035911852507045361090559");
            var modp2048G = 2;

            var theirPublicBI = SpiAHexStringToBigInteger(theirPublicKey);
            var myPrivateBI = DiffieHellman.RandomPrivateKey(modp2048P);
            var myPublicBI = DiffieHellman.PublicKey(modp2048P, modp2048G, myPrivateBI);
            var secretBI = DiffieHellman.Secret(modp2048P, theirPublicBI, myPrivateBI);

            var myPublic = myPublicBI.ToString("X");

            var secret = DHSecretToSPISecret(secretBI);
            return new PublicKeyAndSecret(myPublic, secret);
        }

        /// <summary>
        /// Converts an incoming A value into a BigInteger.
        /// There are some "gotchyas" here which is why this piece of work is abstracted so it can be tested separately.
        /// </summary>
        /// <param name="hexStringA"></param>
        /// <returns>A value as a BigInteger</returns>
        public static BigInteger SpiAHexStringToBigInteger(string hexStringA)
        {
            // We add "00" to bust signed little-endian that Numerics.BigInterger expects. 
            // Because we received an assumed unsiged hex-number string.
            return BigInteger.Parse("00" + hexStringA, NumberStyles.HexNumber);
        }
        
        /// <summary>
        /// Converts the DH secret BigInteger into the hex-string to be used as the secret.
        /// There are some "gotchyas" here which is why this piece of work is abstracted so it can be tested separately.
        /// See: http://www.simplepaymentapi.com/#/api/pairing-process
        /// </summary>
        /// <param name="secretBI">Secret as BigInteger</param>
        /// <returns>Secret as Hex-String</returns>
        public static string DHSecretToSPISecret(BigInteger secretBI)
        {
            // First we convert the big integer to a Hex String
            var secretBIAsHexStr = secretBI.ToString("X");
            
            // Now we apply padding as per the documentation
            if (secretBIAsHexStr.Length == 513)
            { // Sometimes we end up with an extra odd leading "0" which is strange, but we need to remove it.
                secretBIAsHexStr = secretBIAsHexStr.Substring(1);
            }
            else if (secretBIAsHexStr.Length < 512)
            { // in case we ended up wth a small secret, we need to pad it up. because padding=true.
                secretBIAsHexStr = secretBIAsHexStr.PadLeft(512, '0');
            }

            // Now we need to calculate sha256
            // First we turn the padded hex back into byte array
            var secretBIAsByteArray = Crypto.HexStringToByteArray(secretBIAsHexStr);
            // We sha256 that byte array
            var secretSha256AsByteArray = new SHA256Managed().ComputeHash(secretBIAsByteArray);
            // and finally we get that byte array back as a hex string
            var secretAsHexString = Crypto.ByteArrayToHexString(secretSha256AsByteArray);
            return secretAsHexString;
        }
        
        /// <summary>
        /// Internal Holder class for Public and Secret, so that we can use them together in method signatures. 
        /// </summary>
        private class PublicKeyAndSecret
        {
            public PublicKeyAndSecret(string myPublicKey, string sharedSecretKey)
            {
                MyPublicKey = myPublicKey;
                SharedSecretKey = sharedSecretKey;
            }
            public string MyPublicKey { get; }
            public string SharedSecretKey { get; }   
        }
    }
}