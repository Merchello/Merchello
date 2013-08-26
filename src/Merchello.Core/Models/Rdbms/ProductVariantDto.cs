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

        [Column("productId")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProductVariant_merchProduct", Column = "id")]
        public int ProductId { get; set; }

        [Column("sku")]
        public string Sku { get; set; }

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
