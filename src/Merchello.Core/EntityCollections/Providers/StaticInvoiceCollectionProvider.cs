namespace Merchello.Core.EntityCollections.Providers
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The static invoice collection provider.
    /// </summary>
    [EntityCollectionProvider("1AEF5650-242D-4566-ADCA-AC0C90538B47", "454539B9-D753-4C16-8ED5-5EB659E56665", "Static Invoice Collection", "A static invoice collection that could be used for categorizing or grouping sales", false)]
    public class StaticInvoiceCollectionProvider : CachedEntityCollectionProviderBase<IInvoice>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StaticInvoiceCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>        
        public StaticInvoiceCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
        }

        /// <summary>
        /// The perform get entities.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{IInvoice}"/>.
        /// </returns>
        protected override IEnumerable<IInvoice> PerformGetEntities()
        {
            return this.PerformGetPagedEntities(1, long.MaxValue).Items;
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
        protected override bool PerformExists(IInvoice entity)
        {
            return MerchelloContext.Services.InvoiceService.ExistsInCollection(entity.Key, CollectionKey);
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        protected override Page<IInvoice> PerformGetPagedEntities(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            return MerchelloContext.Services.InvoiceService.GetFromCollection(
                CollectionKey,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
        }

        /// <summary>
        /// The perform get paged entity keys.
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
            return
                ((InvoiceService)MerchelloContext.Services.InvoiceService).GetInvoiceKeysFromStaticCollection(
                    CollectionKey,
                    page,
                    itemsPerPage,
                    sortBy,
                    sortDirection);
        }
    }
}