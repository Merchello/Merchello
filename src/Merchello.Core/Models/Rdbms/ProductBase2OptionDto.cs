using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    
    [TableName("merchProductBase2Option")]
    [PrimaryKey("productBaseKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductBase2OptionDto
    {
        [Column("productBaseKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductBase2Option", OnColumns = "productBaseKey, optionId")]
        [ForeignKey(typeof(ProductBaseDto), Name = "FK_merchProductBase2Option_merchProductBase", Column = "pk")]
        public Guid ProductBaseKey { get; set; }

        [Column("optionId")]
        [ForeignKey(typeof(OptionDto), Name = "FK_merchProductBase2Option_merchOption", Column = "id")]
        public int OptionId { get; set; }

        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public OptionDto OptionDto { get; set; }
    }
}
