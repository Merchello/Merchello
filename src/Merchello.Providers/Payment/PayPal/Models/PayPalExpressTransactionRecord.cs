namespace Merchello.Providers.Payment.PayPal.Models
{
    using Merchello.Core.Models;

    public class PayPalExpressTransactionRecord
    {
        public PayPalExpressTransactionRecord()
        {
            this.Success = true;
            this.Data = new PayPalExpressTransaction() { Authorized = false };
        }

        public bool Success { get; set; }

        public PayPalExpressTransaction Data { get; set; }

        public ExpressCheckoutResponse SetExpressCheckout { get; set; }

        public ExpressCheckoutResponse GetExpressCheckoutDetails { get; set; }

        public ExpressCheckoutResponse DoExpressCheckoutPayment { get; set; }

        public ExpressCheckoutResponse DoAuthorization { get; set; }

        public ExpressCheckoutResponse DoCapture { get; set; }
    }

    internal static class PayPalExpressTransactionRecordExtensions
    {
        public static PayPalExpressTransactionRecord GetPayPalTransactionRecord(this IPayment payment)
        {
            return payment.ExtendedData.GetValue<PayPalExpressTransactionRecord>(Providers.Constants.PayPal.ExtendedDataKeys.PayPalExpressTransaction);
        }

        public static void SavePayPalTransactionRecord(this IPayment payment, PayPalExpressTransactionRecord record)
        {
            payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.PayPalExpressTransaction, record);
        }
    }
}