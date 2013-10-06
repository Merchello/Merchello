using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    
    [TableName("merchProduct2ProductOption")]
    [PrimaryKey("productKey", autoIncrement = false)]
    [ExplicitColumns]
    internal class Product2ProductOptionDto
    {
        [Column("productKey")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProduct2Option", OnColumns = "productKey, optionId")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchProduct2Option_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }

        [Column("optionId")]
        [ForeignKey(typeof(ProductOptionDto), Name = "FK_merchProduct2Option_merchOption", Column = "id")]
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
        public ProductOptionDto ProductOptionDto { get; set; }
    }
}
