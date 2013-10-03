using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductActual2ProductAttribute")]
    [PrimaryKey("productActualKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductActual2ProductAttributeDto
    {
        [Column("productActualKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductActual2ProductAttribute", OnColumns = "productActualKey, optionId")]
        [ForeignKey(typeof(ProductActualDto), Name = "FK_merchProductActual2ProductAttribute_merchProductActual", Column = "pk")]
        public Guid ProductActualKey { get; set; }

        [Column("optionId")]
        [ForeignKey(typeof(OptionDto), Name = "FK_merchProductActual2OProductAttribute_merchOption", Column = "id")]
        public int OptionId { get; set; }

        [Column("productAttributeId")]
        [ForeignKey(typeof(ProductAttributeDto), Name = "FK_merchProductActual2ProductAttribute_merchProductAttribute", Column = "id")]
        public int ProductAttributeId { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
