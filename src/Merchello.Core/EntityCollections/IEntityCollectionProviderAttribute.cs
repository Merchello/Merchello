namespace Merchello.Core.EntityCollections
{
    using System;

    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// Represents meta information about an <see cref="IEntityCollectionProvider"/>.
    /// </summary>
    public interface IEntityCollectionProviderMeta : IHasEntityTypeField
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Gets the name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the description.
        /// </summary>
        string Description { get; }

        /// <summary>
        /// Gets or sets the localization name key.
        /// </summary>
        /// <remarks>
        /// e.g. "merchelloProviders/providerNameKey"
        /// </remarks>
        string LocalizedNameKey { get; set; }

        /// <summary>
        /// Gets a value indicating whether manages unique collection.
        /// </summary>
        bool ManagesUniqueCollection { get; }

        /// <summary>
        /// Gets the entity type.
        /// </summary>
        EntityType EntityType { get; }

        /// <summary>
        /// Gets the relative path to the editor view html
        /// </summary>
        string EditorView { get; }
    }
}