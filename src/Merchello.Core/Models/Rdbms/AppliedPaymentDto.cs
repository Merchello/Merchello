namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchAppliedPayment" table.
    /// </summary>
    internal class AppliedPaymentDto : IEntityDto
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
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        public bool Exported { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}