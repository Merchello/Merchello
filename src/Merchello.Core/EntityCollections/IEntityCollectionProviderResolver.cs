namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Umbraco.Core;

    /// <summary>
    /// Defines an EntityCollectionProviderResolver.
    /// </summary>
    internal interface IEntityCollectionProviderResolver
    {
        /// <summary>
        /// The get provider key.
        /// </summary>
        /// <typeparam name="T">
        /// Type of the provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        Guid GetProviderKey<T>();

        /// <summary>
        /// The get provider key.
        /// </summary>
        /// <param name="type">
        /// The type of provider
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        Guid GetProviderKey(Type type);

        /// <summary>
        /// Gets the <see cref="EntityCollectionProviderAttribute"/> from the provider of type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityCollectionProviderAttribute"/>.
        /// </returns>
        EntityCollectionProviderAttribute GetProviderAttribute<T>();

        /// <summary>
        /// Gets the provider attributes for all resolved types.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionProviderAttribute}"/>.
        /// </returns>
        IEnumerable<EntityCollectionProviderAttribute> GetProviderAttributes(); 
            
        /// <summary>
        /// Gets a collection provider types for a specific entity type.
        /// </summary>
        /// <param name="entityType">
        /// The entity type.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<Type> GetProviderTypesForEntityType(EntityType entityType);

        /// <summary>
        /// The get provider for collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        Attempt<EntityCollectionProviderBase> GetProviderForCollection(Guid collectionKey);


        /// <summary>
        /// The get provider for collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <typeparam name="T">
        /// The type of provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="T"/>.
        /// </returns>
        Attempt<T> GetProviderForCollection<T>(Guid collectionKey) where T : EntityCollectionProviderBase;
    }
}