namespace Merchello.Core.Models.Rdbms
{
    using System;

    using JetBrains.Annotations;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchInvoice" table.
    /// </summary>
    internal class InvoiceDto : IEntityDto, IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the store key.
        /// </summary>
        public Guid StoreKey { get; set; }

        /// <summary>
        /// Gets or sets the customer key.
        /// </summary>
        [CanBeNull]
        public Guid? CustomerKey { get; set; }

        /// <summary>
        /// Gets or sets the invoice number prefix.
        /// </summary>
        [CanBeNull]
        public string InvoiceNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the invoice number.
        /// </summary>
        public int InvoiceNumber { get; set; }

        /// <summary>
        /// Gets or sets the po number.
        /// </summary>
        [CanBeNull]
        public string PoNumber { get; set; }

        /// <summary>
        /// Gets or sets the invoice date.
        /// </summary>
        public DateTime InvoiceDate { get; set; }

        /// <summary>
        /// Gets or sets the invoice status key.
        /// </summary>
        public Guid InvoiceStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets the bill to name.
        /// </summary>
        [CanBeNull]
        public string BillToName { get; set; }

        /// <summary>
        /// Gets or sets the bill to address 1.
        /// </summary>
        [CanBeNull]
        public string BillToAddress1 { get; set; }

        /// <summary>
        /// Gets or sets the bill to address 2.
        /// </summary>
        [CanBeNull]
        public string BillToAddress2 { get; set; }

        /// <summary>
        /// Gets or sets the bill to locality.
        /// </summary>
        [CanBeNull]
        public string BillToLocality { get; set; }

        /// <summary>
        /// Gets or sets the bill to region.
        /// </summary>
        [CanBeNull]
        public string BillToRegion { get; set; }

        /// <summary>
        /// Gets or sets the bill to postal code.
        /// </summary>
        [CanBeNull]
        public string BillToPostalCode { get; set; }

        /// <summary>
        /// Gets or sets the bill to country code.
        /// </summary>
        [CanBeNull]
        public string BillToCountryCode { get; set; }

        /// <summary>
        /// Gets or sets the bill to email.
        /// </summary>
        [CanBeNull]
        public string BillToEmail { get; set; }

        /// <summary>
        /// Gets or sets the bill to phone.
        /// </summary>
        [CanBeNull]
        public string BillToPhone { get; set; }

        /// <summary>
        /// Gets or sets the bill to company.
        /// </summary>
        [CanBeNull]
        public string BillToCompany { get; set; }

        /// <summary>
        /// Gets or sets the currency code.
        /// </summary>
        public string CurrencyCode { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        public bool Exported { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether archived.
        /// </summary>
        public bool Archived { get; set; }

        /// <summary>
        /// Gets or sets the total.
        /// </summary>
        public decimal Total { get; set; }

        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}