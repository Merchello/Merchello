namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The table definition and "POCO" for database operations associated with the "merchWarehouseInventory" table.
    /// </summary>
    internal class WarehouseInventoryDto : IDto
    {
        /// <summary>
        /// Gets or sets the warehouse key.
        /// </summary>
        public Guid WarehouseKey { get; set; }

        /// <summary>
        /// Gets or sets the product variant key.
        /// </summary>
        public Guid ProductVariantKey { get; set; }

        /// <summary>
        /// Gets or sets the count.
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// Gets or sets the low count.
        /// </summary>
        public int LowCount { get; set; }

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