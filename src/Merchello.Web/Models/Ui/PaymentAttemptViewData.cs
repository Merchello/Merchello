namespace Merchello.Web.Models.Ui
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The view data for a payment attempt.
    /// </summary>
    public class PaymentAttemptViewData : IMerchelloViewData
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
        /// Gets or sets a value indicating whether success.
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        public Exception Exception { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public IEnumerable<string> Messages { get; set; }
    }
}