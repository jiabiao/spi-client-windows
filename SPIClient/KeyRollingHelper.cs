using System.Security.Cryptography;
using Newtonsoft.Json.Linq;

namespace SPIClient
{   
    public static class KeyRollingHelper
    {
        public static KeyRollingResult PerformKeyRolling(Message krRequest, Secrets currentSecrets)
        {
            var m = new Message(krRequest.Id, Events.KeyRollResponse, new JObject(new JProperty("status", "confirmed")), true);
            var newSecrets = new Secrets(
                Crypto.ByteArrayToHexString(new SHA256Managed().ComputeHash(Crypto.HexStringToByteArray(currentSecrets.EncKey)))
                ,
                Crypto.ByteArrayToHexString(new SHA256Managed().ComputeHash(Crypto.HexStringToByteArray(currentSecrets.HmacKey))));
            return new KeyRollingResult(m, newSecrets);
        }
    }

    public class KeyRollingResult
    {
        public Message KeyRollingConfirmation { get; }
        public Secrets NewSecrets { get; }

        public KeyRollingResult(Message keyRollingConfirmation, Secrets newSecrets)
        {
            KeyRollingConfirmation = keyRollingConfirmation;
            NewSecrets = newSecrets;
        }
    }
}