using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    
    [TableName("merchProduct2Option")]
    [PrimaryKey("productId", autoIncrement = false)]
    [ExplicitColumns]
    internal class Product2OptionDto
    {
        [Column("productId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProduct2Option", OnColumns = "productId, optionId")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProduct2Option_merchProduct", Column = "id")]
        public int ProductId { get; set; }

        [Column("optionId")]
        [ForeignKey(typeof(OptionDto), Name = "FK_merchProduct2Option_merchOption", Column = "id")]
        public int OptionId { get; set; }

        [Column("sortOrder")]
        public int SortOrder { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
