namespace Merchello.Core.Models
{
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    /// <summary>
    /// The entity collection extensions.
    /// </summary>
    public static class EntityCollectionExtensions
    {
        /// <summary>
        /// Resolves the provider for the collection.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        public static EntityCollectionProviderBase ResolveProvider(this IEntityCollection collection)
        {
            return !EntityCollectionProviderResolver.HasCurrent ? null : 
                EntityCollectionProviderResolver.Current.GetProviderForCollection(collection.Key);
        }

        /// <summary>
        /// The resolve provider.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <typeparam name="T">
        /// The type of entity
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        public static EntityCollectionProviderBase<T> ResolveProvider<T>(this IEntityCollection collection)
            where T : IEntity
        {
            var provider = collection.ResolveProvider();
            return provider != null ? provider as EntityCollectionProviderBase<T> : null;
        }

        #region Examine


        #endregion
    }
}