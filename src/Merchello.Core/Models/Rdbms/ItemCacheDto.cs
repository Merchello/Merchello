namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchItemCache" table.
    /// </summary>
    internal class ItemCacheDto : IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the store key.
        /// </summary>
        public Guid StoreKey { get; set; }


        /// <summary>
        /// Gets or sets the entity key.
        /// </summary>
        public Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the item cache type field key.
        /// </summary>
        public Guid ItemCacheTfKey { get; set; }

        /// <summary>
        /// Gets or sets the version key.
        /// </summary>
        public Guid VersionKey { get; set; }
        
        /// <inheritdoc/>
        public DateTime UpdateDate { get; set; }

        /// <inheritdoc/>
        public DateTime CreateDate { get; set; }
    }
}
