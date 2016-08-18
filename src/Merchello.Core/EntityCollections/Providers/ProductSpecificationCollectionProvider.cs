namespace Merchello.Core.EntityCollections.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Represents the product specification collection.
    /// </summary>
    /// <remarks>
    /// EntitySpecificationCollectionProviders need to implement <see cref="IEntitySpecificationCollectionProvider"/> (marker interface)
    /// </remarks>
    [EntityCollectionProvider("5316C16C-E967-460B-916B-78985BB7CED2", "9F923716-A022-4089-A110-1E9B4E1F2AD1", "Product Specification Collection", "A collection of product specification that could be used for product filters and custom product groupings", false)]
    public class ProductSpecificationCollectionProvider : CachedQueryableEntityCollectionProviderBase<IProduct>, IEntitySpecificationCollectionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductSpecificationCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public ProductSpecificationCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// Gets the <see cref="IEntitySpecificationCollection"/>.
        /// </summary>
        public new IEntitySpecificationCollection EntityCollection
        {
            get
            {
                return (IEntitySpecificationCollection)base.EntityCollection;
            }
        }



        /// <summary>
        /// Returns true if the entity exists in the collection.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool PerformExists(IProduct entity)
        {
            if (!EntityCollection.AttributeCollections.Any()) return false;

            var keys = EntityCollection.AttributeCollections.Select(x => x.Key);

            return MerchelloContext.Services.ProductService.ExistsInCollection(entity.Key, keys);
        }

        /// <summary>
        /// The perform get paged entities.
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
        /// The <see cref="Page{IProductContent}"/>.
        /// </returns>
        protected override Page<IProduct> PerformGetPagedEntities(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = GetAttributeCollectionKeys();

            return keys != null ?
                MerchelloContext.Services.ProductService.GetFromCollections(keys, page, itemsPerPage, sortBy, sortDirection) :
                null;
        }

        /// <summary>
        /// The perform get paged keys.
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
        protected override Page<Guid> PerformGetPagedEntityKeys(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            var keys = GetAttributeCollectionKeys();

            return keys != null ?
                ((ProductService)MerchelloContext.Services.ProductService)
                    .GetKeysFromCollection(keys, page, itemsPerPage, sortBy, sortDirection) :
                    null;
        }

        /// <summary>
        /// Gets a distinct page of product keys not in multiple collections.
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

            var keys = GetAttributeCollectionKeys();

            return keys != null
                       ? ((ProductService)MerchelloContext.Services.ProductService).GetKeysFromCollection(
                           keys,
                           args["searchTerm"].ToString(),
                           page,
                           itemsPerPage,
                           sortBy,
                           sortDirection) :
                        null;
        }

        /// <summary>
        /// Gets a distinct page of product keys from multiple collections.
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
            var keys = GetAttributeCollectionKeys();

            return keys != null
                       ? ((ProductService)MerchelloContext.Services.ProductService).GetKeysNotInCollection(
                           keys,
                           page,
                           itemsPerPage,
                           sortBy,
                           sortDirection) :
                           null;
        }

        /// <summary>
        /// Gets a distinct page of product keys that don't exist in multiple collections.
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
            var keys = GetAttributeCollectionKeys();

            return keys != null
                       ? ((ProductService)MerchelloContext.Services.ProductService).GetKeysNotInCollection(
                           keys,
                           args["searchTerm"].ToString(),
                           page,
                           itemsPerPage,
                           sortBy,
                           sortDirection) :
                           null;
        }

        /// <summary>
        /// Overrides the default GetInstance method.
        /// </summary>
        /// <returns>
        /// The <see cref="IEntityCollection"/>.
        /// </returns>
        protected override IEntityCollection GetInstance()
        {
            var cacheKey = string.Format("merch.entitycollection.{0}", CollectionKey);
            var provider = Cache.GetCacheItem(cacheKey);
            if (provider != null) return (IEntitySpecificationCollection)provider;

            var specCollection =
                ((EntityCollectionService)MerchelloContext.Services.EntityCollectionService)
                    .GetEntitySpecificationCollection(CollectionKey);

            return
                (IEntitySpecificationCollection)
                Cache.GetCacheItem(
                    cacheKey,
                    () => specCollection);
        }

        /// <summary>
        /// Gets the collection of child collection keys.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Guid}"/>.
        /// </returns>
        private IEnumerable<Guid> GetAttributeCollectionKeys()
        {
            if (!EntityCollection.AttributeCollections.Any())
            {
                MultiLogHelper.Info<ProductSpecificationCollectionProvider>("ProductSpecificationCollection does not have any child collections. Returning null.");
                return null;
            }

            return EntityCollection.AttributeCollections.Select(x => x.Key);
        }
    }
}