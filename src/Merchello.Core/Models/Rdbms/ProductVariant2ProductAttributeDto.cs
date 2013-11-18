using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductVariant2ProductAttribute")]
    [PrimaryKey("productVariantKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductVariant2ProductAttributeDto
    {
        [Column("productVariantKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductVariant2ProductAttribute", OnColumns = "productVariantKey, optionKey")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductVariant", Column = "pk")]
        public Guid ProductVariantKey { get; set; }

        [Column("optionKey")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductOption", Column = "pk")]
        public Guid OptionKey { get; set; }

        [Column("productAttributeKey")]
        [ForeignKey(typeof(ProductAttributeDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductAttribute", Column = "pk")]
        public Guid ProductAttributeKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public ProductAttributeDto ProductAttributeDto { get; set; }
    }
}
