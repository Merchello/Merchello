namespace Merchello.Core.EntityCollections
{
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Defines an entity specification provider.
    /// </summary>
    public interface IEntitySpecificationCollectionProvider : IEntityCollectionProvider
    {
        /// <summary>
        /// Gets the attribute collections.
        /// </summary>
        IEnumerable<IEntityCollection> AttributeCollections { get; }
    }
}