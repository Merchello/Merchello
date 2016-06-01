namespace Merchello.Web.Models.Ui.Async
{
    using System;

    /// <summary>
    /// A model intended to be used to for JSON payment result responses.
    /// </summary>
    public class PaymentResultAsyncResponse : AsyncResponse, IEmitsBasketItemCount
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        /// <remarks>
        /// Used for payment retry attempts
        /// </remarks>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the payment key.
        /// </summary>
        /// <remarks>
        /// Used for payment retry attempts
        /// </remarks>
        public Guid PaymentKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method name.
        /// </summary>
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Gets or sets the basket item count.
        /// </summary>
        public int ItemCount { get; set; }
    }
}