namespace Merchello.Core.EntityCollections.Providers
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using Umbraco.Core.Persistence;

    using Constants = Merchello.Core.Constants;

    /// <summary>
    /// The unpaid invoice collection provider.
    /// </summary>
    [EntityCollectionProvider("A8120A01-E9BF-4204-ADDD-D9553F6F24FE", "454539B9-D753-4C16-8ED5-5EB659E56665", 
        "Unpaid Invoice Collection", "A dynamic collection queries for unpaid invoices", true, 
        "merchelloProviders/unpaidInvoiceCollection")]
    internal class DynamicUnpaidInvoiceCollectionProvider : CachedQueryableEntityCollectionProviderBase<IInvoice>
    {
        /// <summary>
        /// The <see cref="InvoiceService"/>.
        /// </summary>
        private readonly InvoiceService _invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicUnpaidInvoiceCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public DynamicUnpaidInvoiceCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
            _invoiceService = (InvoiceService)merchelloContext.Services.InvoiceService;
        }

        /// <summary>
        /// Checks if the invoice exists in the collection
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        protected override bool PerformExists(IInvoice entity)
        {
            return entity.InvoiceStatusKey.Equals(Constants.InvoiceStatus.Unpaid);
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
        protected override Page<IInvoice> PerformGetPagedEntities(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Ascending)
        {
            var keyPage = this.PerformGetPagedEntityKeys(page, itemsPerPage, sortBy, sortDirection);
            return _invoiceService.GetPageFromKeyPage(keyPage, () => _invoiceService.GetByKeys(keyPage.Items));
        }

        /// <summary>
        /// Gets a page of keys associated with invoices with an unpaid status
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
            var query =
                Query<IInvoice>.Builder.Where(x => x.InvoiceStatusKey == Constants.InvoiceStatus.Unpaid);

            return _invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection);
        }

        /// <summary>
        /// The perform get paged entity keys.
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
            
            return
                    this._invoiceService.GetInvoiceKeysMatchingInvoiceStatus(
                        args["searchTerm"].ToString(),
                        Constants.InvoiceStatus.Unpaid,
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection);
        }

        /// <summary>
        /// The perform get paged entity keys not in collection.
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
            var query =
               Query<IInvoice>.Builder.Where(x => x.InvoiceStatusKey != Constants.InvoiceStatus.Unpaid);

            return _invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection);
        }

        /// <summary>
        /// The perform get paged entity keys not in collection.
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


            return
                this._invoiceService.GetInvoiceKeysMatchingTermNotInvoiceStatus(
                    args["searchTerm"].ToString(),
                    Constants.InvoiceStatus.Unpaid,
                    page,
                    itemsPerPage,
                    sortBy,
                    sortDirection);
        }
    }
}