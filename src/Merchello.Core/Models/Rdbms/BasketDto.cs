using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchBasket")]
    [PrimaryKey("id", autoIncrement = false)]
    [ExplicitColumns]
    public class BasketDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("identityKey")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchBasketIdentitKey")]
        public Guid IdentityKey { get; set; }

        [Column("typeKey")]
        public Guid TypeKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
