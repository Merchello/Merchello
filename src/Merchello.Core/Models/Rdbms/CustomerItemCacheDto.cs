using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchCustomerItemCache")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class CustomerItemCacheDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("customerKey")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchCustomerItemCacheConsumerKey")]
        public Guid CustomerKey { get; set; }

        [Column("itemCacheTfKey")]
        public Guid ItemCacheTfKey { get; set; }

        [Column("updateDate")]
        [Constraint(Default = "getdate()")]
        public DateTime UpdateDate { get; set; }

        [Column("createDate")]
        [Constraint(Default = "getdate()")]
        public DateTime CreateDate { get; set; }

    }
}
