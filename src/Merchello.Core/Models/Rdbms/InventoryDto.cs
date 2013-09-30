using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInventory")]
    [PrimaryKey("warehouseId", autoIncrement = false)]
    [ExplicitColumns]
    internal class InventoryDto
    {
        [Column("warehouseId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchInventory", OnColumns = "warehouseId, productActualKey")]
        [ForeignKey(typeof(WarehouseDto), Name = "FK_merchInventory_merchWarehouse", Column = "id")]
        public int WarehouseId { get; set; }

        [Column("productActualKey")]
        [ForeignKey(typeof(ProductActualDto), Name = "FK_merchInventory_merchProductActual", Column = "pk")]
        public Guid ProductActualKey { get; set; }

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

    }
}