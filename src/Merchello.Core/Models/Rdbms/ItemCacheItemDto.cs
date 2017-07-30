namespace Merchello.Core.Models.Rdbms
{
    using System;

    /// <summary>
    /// The item cache item dto.
    /// </summary>
    internal class ItemCacheItemDto : LineItemDto, IEntityDto
    {
        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public Guid Key { get; set; }

        /// <summary>
        /// Gets or sets the item cache key which represents the container for the line item.
        /// </summary>
        public Guid ContainerKey { get; set; }
    }
}
