using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductVariant")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductVariantDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

        [Column("productKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProductVariant_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("barcode")]
        public string Barcode { get; set; }

        [Column("available")]
        public bool Available { get; set; }

        [Column("trackInventory")]
        [Constraint(Default = "1")]
        public bool TrackInventory { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
