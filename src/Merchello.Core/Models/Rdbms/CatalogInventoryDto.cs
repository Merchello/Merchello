using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCatalogInventory")]
    [PrimaryKey("catalogKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class CatalogInventoryDto
    {
        [Column("catalogKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchCatalogInventory", OnColumns = "catalogKey, productVariantKey")]
        [ForeignKey(typeof(WarehouseCatalogDto), Name = "FK_merchCatalogInventory_merchWarehouseCatalog", Column = "pk")]
        public Guid CatalogKey { get; set; }

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
        public WarehouseCatalogDto WarehouseCatalogDto { get; set; }

        [ResultColumn]
        public ProductVariantDto ProductVariantDto { get; set; } 
    }
}