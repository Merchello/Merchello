using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchBasket")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class BasketDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("consumerKey")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchBasketConsumerKey")]
        public Guid ConsumerKey { get; set; }

        [Column("basketTypeFieldKey")]
        public Guid BasketTypeFieldKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
