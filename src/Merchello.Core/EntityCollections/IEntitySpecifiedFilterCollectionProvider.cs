namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Marker interface for EntitySpecificationCollectionProviders.
    /// </summary>
    public interface IEntitySpecifiedFilterCollectionProvider : IEntityCollectionProvider
    {
        /// <summary>
        /// Gets the type of provider that should be used when creating attribute collections
        /// </summary>
        Type AttributeCollectionProviderType { get; }
    }
}