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
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchContent2Product", OnColumns = "contentId, productId")]
        public int ContentId { get; set; }


        [Column("productId")]
        [ForeignKey(typeof(ProductDto), Name = "FK_merchContent2Product_merchProduct", Column = "id")]
        public int ProductId { get; set; }


        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }
    }
}
