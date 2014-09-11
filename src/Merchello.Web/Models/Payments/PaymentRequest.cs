namespace Merchello.Web.Models.Payments
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The payment request.
    /// </summary>
    public class PaymentRequest
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the payment key.
        /// </summary>
        public Guid? PaymentKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        public Guid PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets the processor args.
        /// </summary>
        public IEnumerable<KeyValuePair<string, string>> ProcessorArgs { get; set; }
    }
}