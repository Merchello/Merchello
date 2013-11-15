using System;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Merchello.Core.Models.Rdbms
{
    [TableName("merchItemCache")]
    [PrimaryKey("pk", autoIncrement = false)]
    [ExplicitColumns]
    internal class ItemCacheDto
    {
        [Column("pk")]
        [PrimaryKeyColumn(AutoIncrement = false)]
        [Constraint(Default = "newid()")]
        public Guid Key { get; set; }

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
