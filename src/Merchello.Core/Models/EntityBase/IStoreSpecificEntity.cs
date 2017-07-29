namespace Merchello.Core.Models.EntityBase
{
    using System;

    /// <summary>
    /// Represents an entity that is store specific in multi-store setups.
    /// </summary>
    public interface IStoreSpecificEntity : IEntity
    {
        /// <summary>
        /// Gets the store key.
        /// </summary>
        Guid StoreKey { get; } 
    }
}