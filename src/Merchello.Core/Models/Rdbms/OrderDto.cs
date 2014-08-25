namespace Merchello.Core.Models.Rdbms
{
    using System;
    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.DatabaseAnnotations;

    /// <summary>
    /// The order dto.
    /// </summary>
    [TableName("merchOrder")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class OrderDto : IPageableDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the invoice key.
        /// </summary>
        [Column("invoiceKey")]
        [ForeignKey(typeof(InvoiceDto), Name = "FK_merchOrder_merchInvoice",Column = "pk")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public Guid InvoiceKey { get; set; }

        /// <summary>
        /// Gets or sets the order number prefix.
        /// </summary>
        [Column("orderNumberPrefix")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public string OrderNumberPrefix { get; set; }

        /// <summary>
        /// Gets or sets the order number.
        /// </summary>
        [Column("orderNumber")]
        [IndexAttribute(IndexTypes.UniqueNonClustered, Name = "IX_merchOrderNumber")]
        public int OrderNumber { get; set; }

        /// <summary>
        /// Gets or sets the order date.
        /// </summary>
        [Column("orderDate")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchOrderDate")]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// Gets or sets the order status key.
        /// </summary>
        [Column("orderStatusKey")]
        [ForeignKey(typeof(OrderStatusDto), Name = "FK_merchOrder_merchOrderStatus", Column = "pk")]
        public Guid OrderStatusKey { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        [Column("versionKey")]
        [Constraint(Default = "newid()")]
        public Guid VersionKey { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether exported.
        /// </summary>
        [Column("exported")]
        public bool Exported { get; set; }

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

        /// <summary>
        /// Gets or sets the order index dto.
        /// </summary>
        [ResultColumn]
        public OrderIndexDto OrderIndexDto { get; set; }

        /// <summary>
        /// Gets or sets the order status dto.
        /// </summary>
        [ResultColumn]
        public OrderStatusDto OrderStatusDto { get; set; }

    }
}