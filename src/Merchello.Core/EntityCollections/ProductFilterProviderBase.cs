namespace Merchello.Core.EntityCollections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.EntityCollections.Providers;
    using Merchello.Core.Logging;
    using Merchello.Core.Models;
    using Merchello.Core.Models.Interfaces;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// A base class for Product based Specified Filter Collections Providers.
    /// </summary>
    public abstract class ProductFilterGroupProviderBase : CachedQueryableEntityCollectionProviderBase<IProduct>, IProductFilterGroupProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProductFilterGroupProviderBase"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The <see cref="MerchelloContext"/>.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        protected ProductFilterGroupProviderBase(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// Gets the attribute provider type.
        /// </summary>
        public virtual Type FilterProviderType
        {
            get
            {
                return typeof(StaticProductCollectionProvider);
            }
        }

        /// <summary>
        /// Gets the <see cref="IEntityFilterGroup"/>.
        /// </summary>
        public new virtual IEntityFilterGroup EntityGroup
        {
            get
            {
                return (IEntityFilterGroup)base.EntityCollection;
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
            if (!this.EntityGroup.Filters.Any()) return false;

            var keys = this.EntityGroup.Filters.Select(x => x.Key);

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
        /// The <see cref="Page{T}"/>.
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
        /// Gets the collection of child collection keys.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        protected virtual IEnumerable<Guid> GetAttributeCollectionKeys()
        {
            if (!this.EntityGroup.Filters.Any())
            {
                MultiLogHelper.Info<ProductFilterGroupProvider>("ProductSpecificationCollection does not have any child collections. Returning null.");
                return null;
            }

            return this.EntityGroup.Filters.Select(x => x.Key);
        }
    }
}