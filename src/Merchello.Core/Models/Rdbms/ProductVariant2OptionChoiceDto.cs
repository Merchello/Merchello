using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductVariant2OptionChoice")]
    [PrimaryKey("productVariantPk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductVariant2OptionChoiceDto
    {
        [Column("productVariantPk")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductVariant2OptionChoice", OnColumns = "productVariantPk, optionChoiceId")]
        [ForeignKey(typeof(ProductVariantDto), Name = "FK_merchProductVariant2OptionChoice_merchProductVariant", Column = "pk")]
        public Guid ProductVariantKey { get; set; }

        [Column("optionChoiceId")]
        [ForeignKey(typeof(OptionChoiceDto), Name = "FK_merchProductVariant2OptionChoice_merchOptionChoice", Column = "id")]
        public int OptionChoiceId { get; set; }


        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
