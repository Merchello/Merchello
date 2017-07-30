namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchInvoiceItem" table..
    /// </summary>
    internal class InvoiceItemDto : LineItemDto, IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the invoice key which represents the container for the line item.
        /// </summary>
        public Guid ContainerKey { get; set; }
        

        /// <inheritdoc/>
        public override string Sku { get; set; }

        ///// <summary>
        ///// Gets or sets the <see cref="InvoiceDto"/>.
        ///// </summary>
        //[ResultColumn]
        //public InvoiceDto InvoiceDto { get; set; }
    }
}