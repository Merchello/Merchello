using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchProductBase2Content")]
    [PrimaryKey("contentId", autoIncrement = false)]
    [ExplicitColumns]
    internal class ProductBase2ContentDto
    {
        [Column("productBaseKey")]
        [ForeignKey(typeof(ProductBaseDto), Name = "FK_merchProductBase2Content_merchProductBase", Column = "pk")]
        public Guid ProductBaseKey { get; set; }

        [Column("contentId")]
        [PrimaryKeyColumn(AutoIncrement = false, Name = "PK_merchProductBase2Content", OnColumns = "contentId, productBaseKey")]
        public int ContentId { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

        [ResultColumn]
        public ProductBaseDto ProductBaseDto { get; set; }
    }
}
