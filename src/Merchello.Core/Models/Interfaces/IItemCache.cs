namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Represents an item cache.
    /// </summary>
    public interface IItemCache : IStoreSpecificEntity, ILineItemContainer
    {
        /// <summary>
        /// Gets or sets the key of the entity associated with the item cache
        /// </summary>
        
        Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the item cache type field key
        /// </summary>
        
        Guid ItemCacheTfKey { get; set; }

        /// <summary>
        /// Gets or sets the item cache <see cref="ItemCacheType"/>
        /// </summary>
        
        ItemCacheType ItemCacheType { get; set; }
    }
}
