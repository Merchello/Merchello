namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchOrder" table.
    /// </summary>
    internal class OrderDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        [CanBeNull]
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the order number prefix.
        /// </summary>
        [CanBeNull]
        public string OrderNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        public int OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the order status key.
        /// </summary>
        public Guid OrderStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

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