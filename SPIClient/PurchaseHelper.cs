namespace SPIClient
{
    public static class PurchaseHelper
    {
        public static PurchaseRequest CreatePurchaseRequest(int amountCents, string purchaseId)
        {
            return new PurchaseRequest(amountCents, purchaseId);
        }

        public static PurchaseRequest CreatePurchaseRequestV2(string posRefId, int purchaseAmount, int tipAmount, int cashoutAmount, bool promptForCashout)
        {
            var pr = new PurchaseRequest(purchaseAmount, posRefId)
            {
                CashoutAmount = cashoutAmount,
                TipAmount = tipAmount,
                PromptForCashout = promptForCashout
            };
            return pr;
        }

        
        public static RefundRequest CreateRefundRequest(int amountCents, string purchaseId)
        {
            return new RefundRequest(amountCents, purchaseId);
        }

    }
}