namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchWarehouseInventory" table.
    /// </summary>
    [TableName("merchWarehouseInventory")]
    [PrimaryKey("warehouseKey", AutoIncrement = false)]
    [ExplicitColumns]
    internal class WarehouseInventoryDto : IDto
    {
        /// <summary>
        /// Gets or sets the warehouse key.
        /// </summary>
        [Column("warehouseKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchWarehouseInventory", OnColumns = "warehouseKey, productVariantKey")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchWarehouseInventory_merchWarehouse", Column = "pk")]
        public Guid WarehouseKey { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        [Column("productVariantKey")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchWarehouseInventory_merchProductVariant", Column = "pk")]
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        [Column("count")]
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the low count.
        /// </summary>
        [Column("lowCount")] 
        public int LowCount { get; set; }

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
        /// Gets or sets the <see cref="WarehouseDto"/>.
        /// </summary>
        [ResultColumn]
        public WarehouseDto WarehouseDto { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="ProductVariantDto"/>.
        /// </summary>
        [ResultColumn]
        public ProductVariantDto ProductVariantDto { get; set; }
    }
}