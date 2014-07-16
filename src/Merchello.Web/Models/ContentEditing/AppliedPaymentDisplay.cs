namespace Merchello.Web.Models.ContentEditing
{
    using System;

    /// <summary>
    /// The applied payment display.
    /// </summary>
    public class AppliedPaymentDisplay
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the payment key.
        /// </summary>
        public Guid PaymentKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the applied payment type field key.
        /// </summary>
        public Guid AppliedPaymentTfKey { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the record has been exported.
        /// </summary>
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}