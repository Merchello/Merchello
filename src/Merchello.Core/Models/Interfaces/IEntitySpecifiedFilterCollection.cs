namespace Merchello.Core.Models.Interfaces
{
    using System;
    using System.Collections.ObjectModel;

    /// <summary>
    /// Defines the entity specification collection.  Used for filtering entities.
    /// </summary>
    public interface IEntitySpecifiedFilterCollection : IEntityCollection
    {
        /// <summary>
        /// Gets the attribute collections.
        /// </summary>
        SpecifiedFilterAttributeCollection AttributeCollections { get; } 
    }
}