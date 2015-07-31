namespace Merchello.Core.Models
{
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Models.Interfaces;

    using Umbraco.Core.Logging;

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
            if (!EntityCollectionProviderResolver.HasCurrent) return null;
 
            var attempt = EntityCollectionProviderResolver.Current.GetProviderForCollection(collection.Key);

            if (attempt.Success) return attempt.Result;
            
            LogHelper.Error(typeof(EntityCollectionExtensions), "Resolver failed to resolve collection provider", attempt.Exception);
            return null;
        }

        /// <summary>
        /// The resolve provider.
        /// </summary>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <typeparam name="T">
        /// The type of provider
        /// </typeparam>
        /// <returns>
        /// The <see cref="EntityCollectionProviderBase"/>.
        /// </returns>
        public static T ResolveProvider<T>(this IEntityCollection collection)
            where T : class
        {
            var provider = collection.ResolveProvider();
            
            if (provider == null) return null;
            
            if (provider is T) return provider as T;
            
            LogHelper.Debug(typeof(EntityCollectionExtensions), "Provider was resolved but was not of expected type.");

            return null;
        }

        #region Examine


        #endregion
    }
}