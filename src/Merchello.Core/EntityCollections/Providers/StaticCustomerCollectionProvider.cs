namespace Merchello.Core.EntityCollections.Providers
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The static customer collection provider.
    /// </summary>
    [EntityCollectionProvider("A389D41B-C8F1-4289-BD2E-5FFF01DBBDB1", "1607D643-E5E8-4A93-9393-651F83B5F1A9", "Static Customer Collection", "A static customer collection that could be used for categorizing or grouping sales", false)]
    internal sealed class StaticCustomerCollectionProvider : CachedQueryableEntityCollectionProviderBase<ICustomer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticCustomerCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public StaticCustomerCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
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
        protected override bool PerformExists(ICustomer entity)
        {
            return MerchelloContext.Services.CustomerService.ExistsInCollection(entity.Key, CollectionKey);
        }

        /// <summary>
        /// Gets a page of <see cref="ICustomer"/>s from the collection.
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
        /// The <see cref="Page{ICustomer}"/>.
        /// </returns>
        protected override Page<ICustomer> PerformGetPagedEntities(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            return this.MerchelloContext.Services.CustomerService.GetFromCollection(
                this.CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// Gets a <see cref="Page{Guid}"/> of customer keys.
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
            return ((CustomerService)this.MerchelloContext.Services.CustomerService).GetKeysFromCollection(
                    this.CollectionKey,
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
            return ((CustomerService)this.MerchelloContext.Services.CustomerService).GetKeysNotInCollection(
                    this.CollectionKey,
                    page,
                    itemsPerPage,
                    sortBy,
                    sortDirection);
        }

        /// <summary>
        /// Gets get paged entity keys included in the collection
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

            return ((CustomerService)this.MerchelloContext.Services.CustomerService).GetKeysFromCollection(
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

            return ((CustomerService)this.MerchelloContext.Services.CustomerService).GetKeysNotInCollection(
            this.CollectionKey,
            args["searchTerm"].ToString(),
            page,
            itemsPerPage,
            sortBy,
            sortDirection);
        }
    }
}