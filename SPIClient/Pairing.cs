using Newtonsoft.Json.Linq;

namespace SPIClient
{
    /// <summary>
    /// Pairing Interaction 1: Outgoing
    /// </summary>
    public class PairRequest
    {

        public Message ToMessage()
        {
            var data = new JObject(new JProperty("padding", true));
            return new Message(RequestIdHelper.Id("pr"), Events.PairRequest, data, false);
        }
    }

    /// <summary>
    /// Pairing Interaction 2: Incoming
    /// </summary>
    public class KeyRequest
    {
        public string RequestId { get; }
        public string Aenc { get; }
        public string Ahmac { get; }

        public KeyRequest(Message m)
        {
            RequestId = m.Id;
            Aenc = (string)m.Data["enc"]["A"];
            Ahmac = (string)m.Data["hmac"]["A"];
        }
    }

    /// <summary>
    /// Pairing Interaction 3: Outgoing
    /// </summary>
    public class KeyResponse
    {
        public string RequestId { get;}
        public string Benc { get;}
        public string Bhmac { get;}

        public KeyResponse(string requestId, string Benc, string Bhmac)
        {
            RequestId = requestId;
            this.Benc = Benc;
            this.Bhmac = Bhmac;
        }

        public Message ToMessage()
        {
            var data =
                new JObject(
                    new JProperty("enc", new JObject(new JProperty("B", Benc))),
                        new JProperty("hmac", new JObject(new JProperty("B", Bhmac))));

            return new Message(RequestId, Events.KeyResponse, data, false);
        }
    }

    /// <summary>
    /// Pairing Interaction 4: Incoming
    /// </summary>
    public class KeyCheck
    {
        public string ConfirmationCode { get; }

        public KeyCheck(Message m)
        {
            ConfirmationCode = m.IncomingHmac.Substring(0,6);
        }
    }
    
    /// <summary>
    /// Pairing Interaction 5: Incoming
    /// </summary>
    public class PairResponse
    {
        public bool Success { get; }

        public PairResponse(Message m)
        {
            Success = (bool)m.Data["success"];
        }
    }

    /// <summary>
    /// Holder class for Secrets and KeyResponse, so that we can use them together in method signatures.
    /// </summary>
    public class SecretsAndKeyResponse
    {
        public Secrets Secrets { get; }
        public KeyResponse KeyResponse { get; }

        public SecretsAndKeyResponse(Secrets secrets, KeyResponse keyResponse)
        {
            Secrets = secrets;
            KeyResponse = keyResponse;
        }
    }
    
    public class DropKeysRequest
    {
        public Message ToMessage()
        {
            return new Message(RequestIdHelper.Id("drpkys"), Events.DropKeysAdvice, null, true);
        }
    }

    
}