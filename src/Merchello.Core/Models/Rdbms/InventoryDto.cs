using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchInventory")]
    [PrimaryKey("warehouseId", autoIncrement = false)]
    [ExplicitColumns]
    internal class InventoryDto
    {
        [Column("warehouseId")]
        public int WarehouseId { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("count")]
        public int Count { get; set; }

        [Column("lowCount")]
        public int LowCount { get; set; }

        [Column("updateDate")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        public DateTime CreateDate { get; set; }

    }
}