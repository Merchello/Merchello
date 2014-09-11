namespace Merchello.Web.Models.Payments
{
    using ContentEditing;

    /// <summary>
    /// The payment result display.
    /// </summary>
    public class PaymentResultDisplay 
    {
        /// <summary>
        /// Gets or sets a value indicating whether the payment request was successful.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the payment.
        /// </summary>
        public PaymentDisplay Payment { get; set; }

        /// <summary>
        /// Gets or sets the invoice that was paid
        /// </summary>
        public InvoiceDisplay Invoice { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether order creation was approved.
        /// </summary>
        public bool ApproveOrderCreation { get; set; }
    }
}