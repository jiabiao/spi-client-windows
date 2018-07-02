using Newtonsoft.Json.Linq;
using System.Runtime.InteropServices;

namespace SPIClient
{
    public static class PreauthEvents
    {
        public const string AccountVerifyRequest = "account_verify";
        public const string AccountVerifyResponse = "account_verify_response";

        public const string PreauthOpenRequest = "preauth";
        public const string PreauthOpenResponse = "preauth_response";

        public const string PreauthTopupRequest = "preauth_topup";
        public const string PreauthTopupResponse = "preauth_topup_response";

        public const string PreauthExtendRequest = "preauth_extend";
        public const string PreauthExtendResponse = "preauth_extend_response";

        public const string PreauthPartialCancellationRequest = "preauth_partial_cancellation";
        public const string PreauthPartialCancellationResponse = "preauth_partial_cancellation_response";

        public const string PreauthCancellationRequest = "preauth_cancellation";
        public const string PreauthCancellationResponse = "preauth_cancellation_response";

        public const string PreauthCompleteRequest = "completion";
        public const string PreauthCompleteResponse = "completion_response";

    }

    public class AccountVerifyRequest
    {
        public string PosRefId { get; }

        public AccountVerifyRequest(string posRefId)
        {
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId)
            );

            return new Message(RequestIdHelper.Id("prav"), PreauthEvents.AccountVerifyRequest, data, true);
        }
    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("0279484F-E77C-40F5-B098-9EB30314C524")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class AccountVerifyResponse
    {
        public string PosRefId { get; }
        public PurchaseResponse Details { get; }

        private Message _m;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public AccountVerifyResponse() { }

        public AccountVerifyResponse(Message m)
        {
            Details = new PurchaseResponse(m);
            PosRefId = Details.PosRefId;
            _m = m;
        }
    }

    public class PreauthOpenRequest
    {
        public string PosRefId { get; }
        public int PreauthAmount { get; }

        public PreauthOpenRequest(int amountCents, string posRefId)
        {
            PosRefId = posRefId;
            PreauthAmount = amountCents;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("preauth_amount", PreauthAmount)
            );

            return new Message(RequestIdHelper.Id("prac"), PreauthEvents.PreauthOpenRequest, data, true);
        }
    }

    public class PreauthTopupRequest
    {
        public string PreauthId { get; }
        public int TopupAmount { get; }
        public string PosRefId { get; }

        public PreauthTopupRequest(string preauthId, int topupAmountCents, string posRefId)
        {
            PreauthId = preauthId;
            TopupAmount = topupAmountCents;
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("preauth_id", PreauthId),
                new JProperty("topup_amount", TopupAmount)
            );

            return new Message(RequestIdHelper.Id("prtu"), PreauthEvents.PreauthTopupRequest, data, true);
        }
    }

    public class PreauthPartialCancellationRequest
    {
        public string PreauthId { get; }
        public int PartialCancellationAmount { get; }
        public string PosRefId { get; }

        public PreauthPartialCancellationRequest(string preauthId, int partialCancellationAmountCents, string posRefId)
        {
            PreauthId = preauthId;
            PartialCancellationAmount = partialCancellationAmountCents;
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("preauth_id", PreauthId),
                new JProperty("preauth_cancel_amount", PartialCancellationAmount)
            );

            return new Message(RequestIdHelper.Id("prpc"), PreauthEvents.PreauthPartialCancellationRequest, data, true);
        }
    }

    public class PreauthExtendRequest
    {
        public string PreauthId { get; }
        public string PosRefId { get; }

        public PreauthExtendRequest(string preauthId, string posRefId)
        {
            PreauthId = preauthId;
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("preauth_id", PreauthId)
            );

            return new Message(RequestIdHelper.Id("prext"), PreauthEvents.PreauthExtendRequest, data, true);
        }
    }

    public class PreauthCancelRequest
    {
        public string PreauthId { get; }
        public string PosRefId { get; }

        public PreauthCancelRequest(string preauthId, string posRefId)
        {
            PreauthId = preauthId;
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("preauth_id", PreauthId)
            );

            return new Message(RequestIdHelper.Id("prac"), PreauthEvents.PreauthCancellationRequest, data, true);
        }
    }

    public class PreauthCompletionRequest
    {
        public string PreauthId { get; }
        public int CompletionAmount { get; }
        public string PosRefId { get; }

        public PreauthCompletionRequest(string preauthId, int completionAmountCents, string posRefId)
        {
            PreauthId = preauthId;
            CompletionAmount = completionAmountCents;
            PosRefId = posRefId;
        }

        public Message ToMessage()
        {
            var data = new JObject(
                new JProperty("pos_ref_id", PosRefId),
                new JProperty("preauth_id", PreauthId),
                new JProperty("completion_amount", CompletionAmount)
            );

            return new Message(RequestIdHelper.Id("prac"), PreauthEvents.PreauthCompleteRequest, data, true);
        }
    }

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("AFAA4A7E-2BBE-4B28-A00C-C3DB4E786CCE")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class PreauthResponse
    {
        public string PosRefId { get; }
        public string PreauthId { get; }

        public PurchaseResponse Details { get; }

        private Message _m;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public PreauthResponse() { }

        public PreauthResponse(Message m)
        {
            PreauthId = m.GetDataStringValue("preauth_id");
            Details = new PurchaseResponse(m);
            PosRefId = Details.PosRefId;
            _m = m;
        }

        public int GetBalanceAmount()
        {
            var txType = _m.GetDataStringValue("transaction_type");
            switch (txType)
            {
                case "PRE-AUTH":
                    return _m.GetDataIntValue("preauth_amount");
                case "TOPUP":
                    return _m.GetDataIntValue("balance_amount");
                case "CANCEL": // PARTIAL CANCELLATION
                    return _m.GetDataIntValue("balance_amount");
                case "PRE-AUTH EXT":
                    return _m.GetDataIntValue("balance_amount");
                case "PCOMP":
                    return 0; // Balance is 0 after completion
                case "PRE-AUTH CANCEL":
                    return 0; // Balance is 0 after cancellation
                default:
                    return 0;
            }
        }

        public int GetPreviousBalanceAmount()
        {
            var txType = _m.GetDataStringValue("transaction_type");
            switch (txType)
            {
                case "PRE-AUTH":
                    return 0;
                case "TOPUP":
                    return _m.GetDataIntValue("existing_preauth_amount");
                case "CANCEL": // PARTIAL CANCELLATION
                    return _m.GetDataIntValue("existing_preauth_amount");
                case "PRE-AUTH EXT":
                    return _m.GetDataIntValue("existing_preauth_amount");
                case "PCOMP":
                    // THIS IS TECHNICALLY NOT CORRECT WHEN COMPLETION HAPPENS FOR A PARTIAL AMOUNT.
                    // BUT UNFORTUNATELY, THIS RESPONSE DOES NOT CONTAIN "existing_preauth_amount".
                    // SO "completion_amount" IS THE CLOSEST WE HAVE.
                    return _m.GetDataIntValue("completion_amount");
                case "PRE-AUTH CANCEL":
                    return _m.GetDataIntValue("preauth_amount");
                default:
                    return 0;
            }
        }

        public int GetCompletionAmount()
        {
            var txType = _m.GetDataStringValue("transaction_type");
            switch (txType)
            {
                case "PCOMP":
                    return _m.GetDataIntValue("completion_amount");
                    break;
                default:
                    return 0;
            }

        }
    }
}