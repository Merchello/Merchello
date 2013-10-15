using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchItemCache")]
    [PrimaryKey("id")]
    [ExplicitColumns]
    internal class ItemCacheDto
    {
        [Column("id")]
        [PrimaryKeyColumn]
        public int Id { get; set; }

        [Column("entityKey")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchItemCacheEntityKey")]
        public Guid EntityKey { get; set; }

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
