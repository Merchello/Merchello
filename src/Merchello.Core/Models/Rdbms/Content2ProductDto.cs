using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchContent2Product")]
    [PrimaryKey("contentId", autoIncrement = false)]
    [ExplicitColumns]
    internal class Content2ProductDto
    {
        [Column("contentId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchContent2Product", OnColumns = "contentId, productKey")]
        public int ContentId { get; set; }


        [Column("productKey")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchContent2Product_merchProduct", Column = "pk")]
        public Guid ProductKey { get; set; }


        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public ProductDto ProductDto { get; set; }
    }
}
