namespace Merchello.Web.Search
{
    using System;

    using global::Examine.Providers;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Services;

    using Umbraco.Core.Logging;

    /// <summary>
    /// The cached collection query base.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of <see cref="IEntity"/>
    /// </typeparam>
    /// <typeparam name="TDisplay">
    /// The type of display object
    /// </typeparam>
    public abstract class CachedCollectionQueryBase<TEntity, TDisplay> : CachedQueryBase<TEntity, TDisplay>
        where TEntity : class, IEntity
        where TDisplay : class, new()
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedCollectionQueryBase{TEntity,TDisplay}"/> class.
        /// </summary>
        /// <param name="service">
        /// The service.
        /// </param>
        /// <param name="indexProvider">
        /// The index provider.
        /// </param>
        /// <param name="searchProvider">
        /// The search provider.
        /// </param>
        /// <param name="enableDataModifiers">
        /// The enable data modifiers.
        /// </param>
        protected CachedCollectionQueryBase(IPageCachedService<TEntity> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, bool enableDataModifiers)
            : base(service, indexProvider, searchProvider, enableDataModifiers)
        {
        }


        /// <summary>
        /// The get entity collection provider.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection Key.
        /// </param>
        /// <returns>
        /// The <see cref="CachedEntityCollectionProviderBase{T}"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws exception if the provider is not found
        /// </exception>
        /// <exception cref="InvalidCastException">
        /// Throws an invalid cast exception if the provider returned does not match the entity type of the collection
        /// </exception>
        internal CachedEntityCollectionProviderBase<TEntity> GetEntityCollectionProvider(Guid collectionKey)
        {
            var attempt = EntityCollectionProviderResolver.Current.GetProviderForCollection<CachedEntityCollectionProviderBase<TEntity>>(collectionKey);

            if (attempt.Success) return attempt.Result;
            
            LogHelper.Error<CachedProductQuery>("EntityCollectionProvider was not resolved", attempt.Exception);
            throw attempt.Exception;                        
        } 
    }
}