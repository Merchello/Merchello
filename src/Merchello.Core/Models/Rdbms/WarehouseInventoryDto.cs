using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchWarehouseInventory")]
    [PrimaryKey("warehouseId", autoIncrement = false)]
    [ExplicitColumns]
    internal class WarehouseInventoryDto
    {
        [Column("warehouseId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchWarehouseInventory", OnColumns = "warehouseId, productVariantId")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchWarehouseInventory_merchWarehouse", Column = "id")]
        public int WarehouseId { get; set; }

        [Column("productVariantId")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchWarehouseInventory_merchProductVariant", Column = "id")]
        public int ProductVariantId { get; set; }

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