namespace Merchello.Core.EntityCollections.Providers
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The static product collection provider.
    /// </summary>
    [EntityCollectionProvider("4700456D-A872-4721-8455-1DDAC19F8C16", "9F923716-A022-4089-A110-1E9B4E1F2AD1", "Static Product Collection", "A static product collection that could be used for product categories and product groupings", false)]
    internal sealed class StaticProductCollectionProvider : CachedQueryableEntityCollectionProviderBase<IProduct>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticProductCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public StaticProductCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }


        /// <summary>
        /// The perform exists.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool PerformExists(IProduct entity)
        {
            return MerchelloContext.Services.ProductService.ExistsInCollection(entity.Key, CollectionKey);
        }

        /// <summary>
        /// The get entities.
        /// </summary>
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
        /// The <see cref="Page{IProduct}"/>.
        /// </returns>
        protected override Page<IProduct> PerformGetPagedEntities(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            return this.MerchelloContext.Services.ProductService.GetFromCollection(
                this.CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// The get paged entity keys.
        /// </summary>
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
        protected override Page<Guid> PerformGetPagedEntityKeys(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            return ((ProductService)this.MerchelloContext.Services.ProductService).GetKeysFromCollection(
                this.CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// Gets paged entity keys in the collection
        /// </summary>
        /// <param name="args">
        /// The args.
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
        protected override Page<Guid> PerformGetPagedEntityKeys(
            Dictionary<string, object> args,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            if (!args.ContainsKey("searchTerm")) return PerformGetPagedEntityKeys(page, itemsPerPage, sortBy, sortDirection);

            return ((ProductService)this.MerchelloContext.Services.ProductService).GetKeysFromCollection(
                this.CollectionKey,
                args["searchTerm"].ToString(),
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// Get paged entity keys not in collection.
        /// </summary>
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
        protected override Page<Guid> PerformGetPagedEntityKeysNotInCollection(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            return ((ProductService)this.MerchelloContext.Services.ProductService).GetKeysNotInCollection(
                this.CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// Get paged entity keys not in collection.
        /// </summary>
        /// <param name="args">
        /// The args.
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
        protected override Page<Guid> PerformGetPagedEntityKeysNotInCollection(
            Dictionary<string, object> args,
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            if (!args.ContainsKey("searchTerm")) return PerformGetPagedEntityKeysNotInCollection(page, itemsPerPage, sortBy, sortDirection);

            return ((ProductService)this.MerchelloContext.Services.ProductService).GetKeysNotInCollection(
                this.CollectionKey,
                args["searchTerm"].ToString(),
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }
    }
}