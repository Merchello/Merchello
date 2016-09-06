namespace Merchello.Core.Models
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Defines a customer item cache
    /// </summary>
    public interface IItemCache : ILineItemContainer
    {
        /// <summary>
        /// Gets or sets the key of the entity associated with the item cache
        /// </summary>
        [DataMember]
        Guid EntityKey { get; set; }

        /// <summary>
        /// Gets or sets the item cache type field key
        /// </summary>
        [DataMember]
        Guid ItemCacheTfKey { get; set; }

        /// <summary>
        /// Gets or sets the item cache <see cref="ItemCacheType"/>
        /// </summary>
        [DataMember]
        ItemCacheType ItemCacheType { get; set; }
    }
}
