namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchPayment" table.
    /// </summary>
    internal class PaymentDto : IEntityDto, IExtendedDataDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        [CanBeNull]
        public Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method key.
        /// </summary>
        [CanBeNull]
        public Guid? PaymentMethodKey { get; set; }

        /// <summary>
        /// Gets or sets the payment type field key.
        /// </summary>
        public Guid PaymentTfKey { get; set; }

        /// <summary>
        /// Gets or sets the payment method name.
        /// </summary>
        [CanBeNull]
        public string PaymentMethodName { get; set; }

        /// <summary>
        /// Gets or sets the reference number.
        /// </summary>
        [CanBeNull]
        public string ReferenceNumber { get; set; }

        /// <summary>
        /// Gets or sets the amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether authorized.
        /// </summary>
        public bool Authorized { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether collected.
        /// </summary>
        public bool Collected { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the payment has been voided.
        /// </summary>
        public bool Voided { get; set; }

        /// <summary>
        /// Gets or sets the extended data serialization.
        /// </summary>
        [CanBeNull]
        public string ExtendedData { get; set; }

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