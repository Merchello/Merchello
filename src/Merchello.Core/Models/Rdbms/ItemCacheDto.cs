namespace Merchello.Core.Models.Rdbms
{
    using System;

    using Merchello.Core.Acquired.Persistence.DatabaseAnnotations;

    using NPoco;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchItemCache" table.
    /// </summary>
    [TableName("merchItemCache")]
    [PrimaryKey("pk", AutoIncrement = false)]
    [ExplicitColumns]
    internal class ItemCacheDto : EntityDto
    {
        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        [Column("entityKey")]
        [IndexAttribute(IndexTypes.NonClustered, Name = "IX_merchItemCacheEntityKey")]
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the item cache type field key.
        /// </summary>
        [Column("itemCacheTfKey")]
        public Guid ItemCacheTfKey { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        [Column("versionKey")]
        [Constraint(Default = "newid()")]
        public Guid VersionKey { get; set; }
    }
}
