using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductVariant2ProductAttribute")]
    [PrimaryKey("productVariantId", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductVariant2ProductAttributeDto
    {
        [Column("productVariantId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductVariant2ProductAttribute", OnColumns = "productVariantId, optionId")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductVariant", Column = "id")]
        public int ProductVariantId { get; set; }

        [Column("optionId")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductOption", Column = "id")]
        public int OptionId { get; set; }

        [Column("productAttributeId")]
        [ForeignKey(typeof(ProductAttributeDto), Name = "FK_merchProductVariant2ProductAttribute_merchProductAttribute", Column = "id")]
        public int ProductAttributeId { get; set; }

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
