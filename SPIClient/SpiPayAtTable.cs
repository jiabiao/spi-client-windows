﻿using System.Collections.Generic;
using System.Runtime.InteropServices;
using Newtonsoft.Json.Linq;

namespace SPIClient
{

    public delegate BillStatusResponse PayAtTableGetBillStatus(string billId, string tableId, string operatorId);

    public delegate BillStatusResponse PayAtTableBillPaymentReceived(BillPayment billPayment, string updatedBillData);

    /// <summary>
    /// These attributes work for COM interop.
    /// </summary>
    [ComVisible(true)]
    [Guid("9C77D435-D69D-4403-9DEA-9DCEAC66E450")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    public class SpiPayAtTable
    {
        private readonly Spi _spi;

        public PayAtTableConfig Config { get; }

        /// <summary>
        /// This delegate will be called when the Eftpos needs to know the current state of a bill for a table. 
        /// <para />
        /// Parameters:<para />
        /// billId - The unique identifier of the bill. If empty, it means that the PayAtTable flow on the Eftpos is just starting, and the lookup is by tableId.<para />
        /// tableId - The identifier of the table that the bill is for. <para />
        /// operatorId - The id of the operator entered on the eftpos. <para />
        /// <para />
        /// Return:<para />
        /// You need to return the current state of the bill.
        /// </summary>
        public PayAtTableGetBillStatus GetBillStatus;

        public PayAtTableBillPaymentReceived BillPaymentReceived;

        /// <summary>
        /// This default stucture works for COM interop.
        /// </summary>
        public SpiPayAtTable() { }

        internal SpiPayAtTable(Spi spi)
        {
            _spi = spi;

            Config = new PayAtTableConfig
            {
                OperatorIdEnabled = true,
                AllowedOperatorIds = new List<string>(),
                EqualSplitEnabled = true,
                SplitByAmountEnabled = true,
                SummaryReportEnabled = true,
                TippingEnabled = true,
                LabelOperatorId = "Operator ID",
                LabelPayButton = "Pay at Table",
                LabelTableId = "Table Number"
            };
        }

        public void PushPayAtTableConfig()
        {
            _spi._send(Config.ToMessage(RequestIdHelper.Id("patconf")));
        }

        internal void _handleGetBillDetailsRequest(Message m)
        {
            var operatorId = m.GetDataStringValue("operator_id");
            var tableId = m.GetDataStringValue("table_id");

            // Ask POS for Bill Details for this tableId, inluding encoded PaymentData
            var billStatus = GetBillStatus("", tableId, operatorId);
            billStatus.TableId = tableId;
            if (billStatus.TotalAmount <= 0)
            {
                _log.Info("Table has 0 total amount. not sending it to eftpos.");
                billStatus.Result = BillRetrievalResult.INVALID_TABLE_ID;
            }

            _spi._send(billStatus.ToMessage(m.Id));
        }

        internal void _handleBillPaymentAdvice(Message m)
        {
            var billPayment = new BillPayment(m);

            // Ask POS for Bill Details, inluding encoded PaymentData
            var existingBillStatus = GetBillStatus(billPayment.BillId, billPayment.TableId, billPayment.OperatorId);
            if (existingBillStatus.Result != BillRetrievalResult.SUCCESS)
            {
                _log.Warn("Could not retrieve Bill Status for Payment Advice. Sending Error to Eftpos.");
                _spi._send(existingBillStatus.ToMessage(m.Id));
            }

            var existingPaymentHistory = existingBillStatus.getBillPaymentHistory();

            var foundExistingEntry = existingPaymentHistory.Find(phe => phe.GetTerminalRefId() == billPayment.PurchaseResponse.GetTerminalReferenceId());
            if (foundExistingEntry != null)
            {
                // We have already processed this payment.
                // perhaps Eftpos did get our acknowledgement.
                // Let's update Eftpos.
                _log.Warn("Had already received this bill_paymemnt advice from eftpos. Ignoring.");
                _spi._send(existingBillStatus.ToMessage(m.Id));
                return;
            }

            // Let's add the new entry to the history
            var updatedHistoryEntries = new List<PaymentHistoryEntry>(existingPaymentHistory)
            {
                new PaymentHistoryEntry
                {
                    PaymentType = billPayment.PaymentType.ToString().ToLower(),
                    PaymentSummary = billPayment.PurchaseResponse.ToPaymentSummary()
                }
            };
            var updatedBillData = BillStatusResponse.ToBillData(updatedHistoryEntries);

            // Advise POS of new payment against this bill, and the updated BillData to Save.
            var updatedBillStatus = BillPaymentReceived(billPayment, updatedBillData);

            // Just in case client forgot to set these:
            updatedBillStatus.BillId = billPayment.BillId;
            updatedBillStatus.TableId = billPayment.TableId;

            if (updatedBillStatus.Result != BillRetrievalResult.SUCCESS)
            {
                _log.Warn("POS Errored when being Advised of Payment. Letting EFTPOS know, and sending existing bill data.");
                updatedBillStatus.BillData = existingBillStatus.BillData;
            }
            else
            {
                updatedBillStatus.BillData = updatedBillData;
            }

            _spi._send(updatedBillStatus.ToMessage(m.Id));
        }

        internal void _handleGetTableConfig(Message m)
        {
            _spi._send(Config.ToMessage(m.Id));
        }

        private static readonly log4net.ILog _log = log4net.LogManager.GetLogger("SPIClient", "spipat");

    }

}