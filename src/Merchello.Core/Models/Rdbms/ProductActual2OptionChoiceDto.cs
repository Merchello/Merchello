using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductActual2OptionChoice")]
    [PrimaryKey("productActualKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductActual2OptionChoiceDto
    {
        [Column("productActualKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductActual2OptionChoice", OnColumns = "productActualKey, optionChoiceId")]
        [ForeignKey(typeof(ProductActualDto), Name = "FK_merchProductBase2OptionChoice_merchProductBase", Column = "pk")]
        public Guid ProductActualKey { get; set; }

        [Column("optionChoiceId")]
        [ForeignKey(typeof(OptionChoiceDto), Name = "FK_merchProductBase2OptionChoice_merchOptionChoice", Column = "id")]
        public int OptionChoiceId { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
