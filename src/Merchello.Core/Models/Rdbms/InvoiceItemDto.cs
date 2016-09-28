namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchInvoiceItem" table..
    /// </summary>
    [TableName("merchInvoiceItem")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class InvoiceItemDto : LineItemDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Column("pk")]
        [Constraint(Default = "newid()")]
        public override Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the invoice key which represents the container for the line item.
        /// </summary>
        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchInvoiceItem_merchInvoice", Column = "pk")]
        public override Guid ContainerKey { get; set; }
        

        /// <inheritdoc/>
        [Column("sku")]
        [Index(IndexTypes.NonClustered, Name = "IX_merchInvoiceItemSku")]
        public override string Sku { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="InvoiceDto"/>.
        /// </summary>
        [ResultColumn]
        public InvoiceDto InvoiceDto { get; set; }
    }
}