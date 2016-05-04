namespace Merchello.Providers.Payment.PayPal.Models
{
    using System.Diagnostics.CodeAnalysis;

    using Merchello.Core.Models;

    /// <summary>
    /// A model to store serialized transaction data for PayPal Express Checkout transactions.
    /// </summary>
    public class PayPalExpressTransactionRecord
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PayPalExpressTransactionRecord"/> class.
        /// </summary>
        public PayPalExpressTransactionRecord()
        {
            this.Success = true;
            this.Data = new PayPalExpressTransaction() { Authorized = false };
        }

        /// <summary>
        /// Gets or sets a value indicating whether the currenct transcation was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the general or common data used by multiple API calls.
        /// </summary>
        public PayPalExpressTransaction Data { get; set; }

        /// <summary>
        /// Gets or sets the SetExpressCheckout transaction response.
        /// </summary>
        public ExpressCheckoutResponse SetExpressCheckout { get; set; }

        /// <summary>
        /// Gets or sets the GetExpressCheckoutDetails transaction response.
        /// </summary>
        public ExpressCheckoutResponse GetExpressCheckoutDetails { get; set; }

        /// <summary>
        /// Gets or sets the DoExpressCheckoutPayment transaction response.
        /// </summary>
        public ExpressCheckoutResponse DoExpressCheckoutPayment { get; set; }

        /// <summary>
        /// Gets or sets the DoAuthorization transaction response.
        /// </summary>
        public ExpressCheckoutResponse DoAuthorization { get; set; }

        /// <summary>
        /// Gets or sets the DoCapture transaction response.
        /// </summary>
        public ExpressCheckoutResponse DoCapture { get; set; }

        /// <summary>
        /// Gets or sets the RefundTransaction response.
        /// </summary>
        public ExpressCheckoutResponse RefundTransaction { get; set; }
    }

    /// <summary>
    /// Utility extensions for the <see cref="PayPalExpressTransactionRecord"/>.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    internal static class PayPalExpressTransactionRecordExtensions
    {
        /// <summary>
        /// Gets the <see cref="PayPalExpressTransactionRecord"/> stored in the <see cref="IPayment"/>.
        /// </summary>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <returns>
        /// The <see cref="PayPalExpressTransactionRecord"/>.
        /// </returns>
        public static PayPalExpressTransactionRecord GetPayPalTransactionRecord(this IPayment payment)
        {
            return payment.ExtendedData.GetValue<PayPalExpressTransactionRecord>(Constants.PayPal.ExtendedDataKeys.PayPalExpressTransaction);
        }

        /// <summary>
        /// Stores a <see cref="PayPalExpressTransactionRecord"/> into the <see cref="IPayment"/>'s <see cref="ExtendedDataCollection"/>.
        /// </summary>
        /// <param name="payment">
        /// The payment.
        /// </param>
        /// <param name="record">
        /// The record.
        /// </param>
        public static void SavePayPalTransactionRecord(this IPayment payment, PayPalExpressTransactionRecord record)
        {
            payment.ExtendedData.SetValue(Constants.PayPal.ExtendedDataKeys.PayPalExpressTransaction, record);
        }
    }
}