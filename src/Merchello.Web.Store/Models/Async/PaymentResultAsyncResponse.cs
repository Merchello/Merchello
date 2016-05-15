namespace Merchello.Web.Store.Models.Async
{
    using System;

    /// <summary>
    /// A model intended to be used to for JSON payment result responses.
    /// </summary>
    public class PaymentResultAsyncResponse : AsyncResponse
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
    }
}