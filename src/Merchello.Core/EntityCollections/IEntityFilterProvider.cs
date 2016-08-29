namespace Merchello.Core.EntityCollections
{
    using System;

    /// <summary>
    /// Defines an EntityFilterProviders.
    /// </summary>
    public interface IEntityFilterGroupProvider : IEntityCollectionProvider
    {
        /// <summary>
        /// Gets the type of provider that should be used when creating filter collections
        /// </summary>
        Type FilterProviderType { get; }
    }
}