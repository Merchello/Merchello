namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using global::Examine.Providers;

    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models.EntityBase;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;
    using Merchello.Web.Models.Querying;

    using Umbraco.Core;
    using Umbraco.Core.Logging;
    using Umbraco.Core.Persistence;
    using Umbraco.Web.Media.EmbedProviders.Settings;

    /// <summary>
    /// The cached collection query base.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of <see cref="IEntity"/>
    /// </typeparam>
    /// <typeparam name="TDisplay">
    /// The type of display object
    /// </typeparam>
    public abstract class CachedQueryableCollectionQueryBase<TEntity, TDisplay> : CachedQueryBase<TEntity, TDisplay>, ICachedCollectionQuery
        where TEntity : class, IEntity
        where TDisplay : class, new()
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedQueryableCollectionQueryBase{TEntity,TDisplay}"/> class.
        /// </summary>
        /// <param name="cacheHelper">
        /// The <see cref="CacheHelper"/>.
        /// </param>
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
        protected CachedQueryableCollectionQueryBase(CacheHelper cacheHelper, IPageCachedService<TEntity> service, BaseIndexProvider indexProvider, BaseSearchProvider searchProvider, bool enableDataModifiers)
            : base(cacheHelper, service, indexProvider, searchProvider, enableDataModifiers)
        {
        }

        /// <summary>
        /// The get products from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public virtual QueryResultDisplay GetFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {

            return
                this.GetQueryResultDisplay(GetCollectionPagedKeys(collectionKey, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Gets entities from the collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public virtual QueryResultDisplay GetFromCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var provider = this.GetEntityCollectionProvider(collectionKey);

            if (!(provider is CachedQueryableEntityCollectionProviderBase<TEntity>))
            {
                LogHelper.Error(typeof(CachedQueryableEntityCollectionProviderBase<TEntity>), "Provider cannot be cast to Cached Queryable Provider", new InvalidOperationException("Provider cannot execute query.  Returning an empty result."));
                return new QueryResultDisplay();
            }


            var args = this.BuildSearchTermArgs(searchTerm);
            var cacheKey =
                PagedKeyCache.GetPagedQueryCacheKey<CachedQueryableCollectionQueryBase<TEntity, TDisplay>>(
                    "GetFromCollection",
                    page,
                    itemsPerPage,
                    sortBy,
                    sortDirection,
                    new Dictionary<string, string>
                    {
                        { "collectionKey", collectionKey.ToString() },
                        { "searchTerm", searchTerm }
                    });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);
            if (pagedKeys != null) return GetQueryResultDisplay(pagedKeys);

            return
                this.GetQueryResultDisplay(
                    PagedKeyCache
                    .CachePage(
                        cacheKey,
                        ((CachedQueryableEntityCollectionProviderBase<TEntity>)provider).GetPagedEntityKeys(args, page, itemsPerPage, sortBy, sortDirection)));
        }

        /// <summary>
        /// Gets entities not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public virtual QueryResultDisplay GetNotInCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var provider = this.GetEntityCollectionProvider(collectionKey);

            if (!(provider is CachedQueryableEntityCollectionProviderBase<TEntity>))
            {
                LogHelper.Error(typeof(CachedQueryableEntityCollectionProviderBase<TEntity>), "Provider cannot be cast to Cached Queryable Provider", new InvalidOperationException("Provider cannot execute query.  Returning an empty result."));
                return new QueryResultDisplay();
            }

            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<CachedQueryableCollectionQueryBase<TEntity, TDisplay>>(
                "GetNotInCollection",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string>
                {
                    { "collectionKey", collectionKey.ToString() }
                });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            if (pagedKeys != null) return GetQueryResultDisplay(pagedKeys);

            return this.GetQueryResultDisplay(
                    PagedKeyCache
                    .CachePage(
                        cacheKey, ((CachedQueryableEntityCollectionProviderBase<TEntity>)provider).GetPagedEntityKeysNotInCollection(page, itemsPerPage, sortBy, sortDirection)));
        }

        /// <summary>
        /// Gets entities not in collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public virtual QueryResultDisplay GetNotInCollection(
            Guid collectionKey,
            string searchTerm,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var provider = this.GetEntityCollectionProvider(collectionKey);

            if (!(provider is CachedQueryableEntityCollectionProviderBase<TEntity>))
            {
                LogHelper.Error(typeof(CachedQueryableEntityCollectionProviderBase<TEntity>), "Provider cannot be cast to Cached Queryable Provider", new InvalidOperationException("Provider cannot execute query.  Returning an empty result."));
                return new QueryResultDisplay();
            }

            var args = this.BuildSearchTermArgs(searchTerm);

            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<CachedQueryableCollectionQueryBase<TEntity, TDisplay>>(
                "GetNotInCollection",
                page,
                itemsPerPage,
                sortBy,
                sortDirection,
                new Dictionary<string, string>
                {
                    { "collectionKey", collectionKey.ToString() },
                    { "searchTerm", searchTerm }
                });

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);

            if (pagedKeys != null) return GetQueryResultDisplay(pagedKeys);

            return this.GetQueryResultDisplay(
                    PagedKeyCache
                    .CachePage(
                        cacheKey,
                        ((CachedQueryableEntityCollectionProviderBase<TEntity>)provider).GetPagedEntityKeysNotInCollection(args, page, itemsPerPage, sortBy, sortDirection)));
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

        /// <summary>
        /// Gets a paged collection of entity keys for a collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        protected Page<Guid> GetCollectionPagedKeys(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var args = new Dictionary<string, string>
                {
                    { "collectionKey", collectionKey.ToString() }
                };

            var cacheKey = PagedKeyCache.GetPagedQueryCacheKey<CachedQueryableCollectionQueryBase<TEntity, TDisplay>>(
               "GetFromCollection",
               page,
               itemsPerPage,
               sortBy,
               sortDirection,
               args);

            var pagedKeys = PagedKeyCache.GetPageByCacheKey(cacheKey);
            if (pagedKeys != null) return pagedKeys;

            var provider = this.GetEntityCollectionProvider(collectionKey);
            return PagedKeyCache.CachePage(
                       cacheKey,
                       provider.GetPagedEntityKeys(page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Builds the search term args.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <returns>
        /// The <see cref="Dictionary"/>.
        /// </returns>
        protected Dictionary<string, object> BuildSearchTermArgs(string searchTerm)
        {
            return new Dictionary<string, object>()
                       {
                           { "searchTerm", searchTerm }
                       };
        }
    }
}