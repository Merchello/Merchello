namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchProduct2EntityCollection" table.
    /// </summary>
    internal class Product2EntityCollectionDto : IDto
    {
        /// <summary>
        /// Gets or sets the product key.
        /// </summary>
        public Guid ProductKey { get; set; }

        /// <summary>
        /// Gets or sets the product collection key.
        /// </summary>
        public Guid EntityCollectionKey { get; set; }

        /// <summary>
        /// Gets or sets the update date.
        /// </summary>
        public DateTime UpdateDate { get; set; }

        /// <summary>
        /// Gets or sets the create date.
        /// </summary>
        public DateTime CreateDate { get; set; }
    }
}