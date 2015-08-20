namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The invoice 2 entity collection dto.
    /// </summary>
    [TableName("merchInvoice2EntityCollection")]
    [PrimaryKey("invoiceKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class Invoice2EntityCollectionDto
    {
        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        [Column("invoiceKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchInvoice2EntityCollection", OnColumns = "invoiceKey, entityCollectionKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchInvoice2EntityCollection_merchInvoice", Column = "pk")]
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the product collection key.
        /// </summary>
        [Column("entityCollectionKey")]
        [ForeignKey(typeof(EntityCollectionDto), Name = "FK_merchInvoice2EntityCollection_merchEntityCollection", Column = "pk")]
        public Guid EntityCollectionKey { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }     
}