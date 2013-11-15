using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchWarehouseInventory")]
    [PrimaryKey("warehouseKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class WarehouseInventoryDto
    {
        [Column("warehouseKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchWarehouseInventory", OnColumns = "warehouseKey, productVariantKey")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchWarehouseInventory_merchWarehouse", Column = "pk")]
        public Guid WarehouseKey { get; set; }

        [Column("productVariantKey")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchWarehouseInventory_merchProductVariant", Column = "pk")]
        public Guid ProductVariantKey { get; set; }

        [Column("count")]
        public int Count { get; set; }

        [Column("lowCount")] 
        public int LowCount { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public WarehouseDto WarehouseDto { get; set; }

        [ResultColumn]
        public ProductVariantDto ProductVariantDto { get; set; }

    }
}