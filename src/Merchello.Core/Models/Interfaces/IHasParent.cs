namespace Merchello.Core.Models
{
    using System;
    

    using Merchello.Core.Models.EntityBase;

    /// <summary>
    /// Marker interface for entities that have a parent key.
    /// </summary>
    public interface IHasParent : IHasKeyId
    {
        /// <summary>
        /// Gets the parent key.
        /// </summary>
        
        Guid? ParentKey { get; }
    }
}