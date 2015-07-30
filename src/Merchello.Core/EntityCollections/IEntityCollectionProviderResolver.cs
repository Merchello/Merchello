namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// The EntityCollectionProviderResolver interface.
    /// </summary>
    internal interface IEntityCollectionProviderResolver
    {
        /// <summary>
        /// The get all.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<EntityCollectionProviderBase> GetAllProviders();

        /// <summary>
        /// The get all providers.
        /// </summary>
        /// <typeparam name="T">
        /// The type of <see cref="EntityCollectionProviderBase"/>
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{EntityCollectionProviderBase}"/>.
        /// </returns>
        IEnumerable<T> GetAllProviders<T>() where T : EntityCollectionProviderBase;

        /// <summary>
        /// The get provider by providerKey.
        /// </summary>
        /// <param name="providerKey">
        /// The providerKey.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity collection provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> GetProviderByKey<T>(Guid providerKey) where T : EntityCollectionProviderBase;

        /// <summary>
        /// The get provider for collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        EntityCollectionProviderBase GetProviderForCollection(Guid collectionKey);

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
        T GetProviderForCollection<T>(Guid collectionKey) where T : EntityCollectionProviderBase;

        /// <summary>
        /// The get provider by providerKey.
        /// </summary>
        /// <param name="providerKey">
        /// The providerKey.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        EntityCollectionProviderBase GetProviderByKey(Guid providerKey);
    }
}