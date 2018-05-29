using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace SPIClient
{    
    public delegate void CBTxFlowStateChanged(TransactionFlowState txState);
    public delegate void CBPairingFlowStateChanged(PairingFlowState pairingFlowState);
    public delegate void CBSecretsChanged(Secrets secrets);
    public delegate void CBSpiStatusChanged(SpiStatusEventArgs status);

    /// <summary>
    /// This class is wrapper for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("AA7C0854-4FF3-4EE7-A807-6C534B0B02DC")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class ComWrapper
    {
        private Spi _spi;

        IntPtr ptr;
        CBTxFlowStateChanged callBackTxState;
        CBPairingFlowStateChanged callBackPairingFlowState;
        CBSecretsChanged callBackSecrets;
        CBSpiStatusChanged callBackStatus;

        public Spi SpiInit(string posId, string eftposAddress, Secrets secrets)
        {
            return new Spi(posId, eftposAddress, secrets);
        }

        public PurchaseResponse PurchaseResponseInit(Message m)
        {
            return new PurchaseResponse(m);
        }

        public RefundResponse RefundResponseInit(Message m)
        {
            return new RefundResponse(m);
        }

        public Settlement SettlementInit(Message m)
        {
            return new Settlement(m);
        }

        public Secrets SecretsInit(string encKey, string hmacKey)
        {
            return new Secrets(encKey, hmacKey);
        }

        public GetLastTransactionResponse GetLastTransactionResponseInit(Message m)
        {
            return new GetLastTransactionResponse(m);
        }

        public CashoutOnlyResponse CashoutOnlyResponseInit(Message m)
        {
            return new CashoutOnlyResponse(m);
        }

        public MotoPurchaseResponse MotoPurchaseResponseInit(Message m)
        {
            return new MotoPurchaseResponse(m);
        }

        public SchemeSettlementEntry[] GetSchemeSettlementEntries(TransactionFlowState txState)
        {
            var settleResponse = new Settlement(txState.Response);
            var schemes = settleResponse.GetSchemeSettlementEntries();
            var schemeList = new List<SchemeSettlementEntry>();
            foreach (var s in schemes)
            {
                schemeList.Add(s);
            }

            return schemeList.ToArray();
        }

        public void Main(Spi spi, Int32 cBTxStatePtr, Int32 cBPairingFlowStatePtr, Int32 cBsecretsPtr, Int32 cBStatusPtr)
        {
            _spi = spi; // It is ok to not have the secrets yet to start with.
            ptr = new IntPtr(cBTxStatePtr);
            callBackTxState = (CBTxFlowStateChanged)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CBTxFlowStateChanged));
            ptr = new IntPtr(cBPairingFlowStatePtr);
            callBackPairingFlowState = (CBPairingFlowStateChanged)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CBPairingFlowStateChanged));
            ptr = new IntPtr(cBsecretsPtr);
            callBackSecrets = (CBSecretsChanged)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CBSecretsChanged));
            ptr = new IntPtr(cBStatusPtr);
            callBackStatus = (CBSpiStatusChanged)Marshal.GetDelegateForFunctionPointer(ptr, typeof(CBSpiStatusChanged));

            _spi.StatusChanged += OnSpiStatusChanged;
            _spi.PairingFlowStateChanged += OnPairingFlowStateChanged;
            _spi.SecretsChanged += OnSecretsChanged;
            _spi.TxFlowStateChanged += OnTxFlowStateChanged;
        }

        private void OnTxFlowStateChanged(object sender, TransactionFlowState txState)
        {
            callBackTxState(txState);
        }

        private void OnPairingFlowStateChanged(object sender, PairingFlowState pairingFlowState)
        {
            callBackPairingFlowState(pairingFlowState);
        }

        private void OnSecretsChanged(object sender, Secrets secrets)
        {
            callBackSecrets(secrets);
        }

        private void OnSpiStatusChanged(object sender, SpiStatusEventArgs status)
        {
            callBackStatus(status);
        }

        public string Get_Id(string prefix)
        {
            return RequestIdHelper.Id(prefix);
        }

        public String GetSpiStatusEnumName(int intSpiStatus)
        {
            SpiStatus spiStatus = (SpiStatus)intSpiStatus;
            return spiStatus.ToString();
        }

        public String GetSpiFlowEnumName(int intSpiFlow)
        {
            SpiFlow spiFlow = (SpiFlow)intSpiFlow;
            return spiFlow.ToString();
        }

        public String GetSuccessStateEnumName(int intSuccessState)
        {
            Message.SuccessState successState = (Message.SuccessState)intSuccessState;
            return successState.ToString();
        }

        public String GetTransactionTypeEnumName(int intTransactionType)
        {
            TransactionType transactionType = (TransactionType)intTransactionType;
            return transactionType.ToString();
        }

        private static readonly ILog log = LogManagerWrapper.GetLogger("spi");
    }

    public static class LogManagerWrapper
    {
        private static readonly string LOG_CONFIG_FILE = @"path\to\log4net.config";

        public static ILog GetLogger(string type)
        {
            // If no loggers have been created, load our own.
            if (LogManager.GetCurrentLoggers().Length == 0)
            {
                LoadConfig();
            }
            return LogManager.GetLogger(type);
        }

        private static void LoadConfig()
        {
            //// TODO: Do exception handling for File access issues and supply sane defaults if it's unavailable.   
            XmlConfigurator.ConfigureAndWatch(new FileInfo(LOG_CONFIG_FILE));
        }
    }
}