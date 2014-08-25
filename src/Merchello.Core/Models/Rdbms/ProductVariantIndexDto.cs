using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductVariantIndex")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    public class ProductVariantIndexDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("productVariantKey")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariantIndex_merchProductVariant", Column = "pk")]
        [Index(IndexTypes.UniqueNonClustered, Name = "IX_merchProductVariantIndex")]
        public Guid ProductVariantKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}