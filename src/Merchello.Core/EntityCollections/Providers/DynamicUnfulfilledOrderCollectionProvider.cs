namespace Merchello.Core.EntityCollections.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Services;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The dynamic unfulfilled order collection provider.
    /// </summary>
    [EntityCollectionProvider("5FD6E5EB-0B7C-41A4-B863-7AEC31BE84C0", "454539B9-D753-4C16-8ED5-5EB659E56665",
        "Invoices with unfulfilled orders collection", "A dynamic collection queries for unfufilled orders and returns the associated invoices", true,
        "merchelloProviders/notFulfilledOrderCollection")]
    internal class DynamicUnfulfilledOrderCollectionProvider : CachedQueryableEntityCollectionProviderBase<IInvoice>
    {
        /// <summary>
        /// The <see cref="InvoiceService"/>.
        /// </summary>
        private readonly InvoiceService _invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicUnfulfilledOrderCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public DynamicUnfulfilledOrderCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
            _invoiceService = (InvoiceService)merchelloContext.Services.InvoiceService;
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
            return !entity.Orders.Any()
                   || entity.Orders.All(x => x.OrderStatusKey == Constants.OrderStatus.NotFulfilled);
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
            return _invoiceService.GetInvoiceKeysMatchingOrderStatus(
                Constants.OrderStatus.NotFulfilled,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
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
                    this._invoiceService.GetInvoiceKeysMatchingOrderStatus(
                        args["searchTerm"].ToString(),
                        Constants.OrderStatus.NotFulfilled,
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
            return _invoiceService.GetInvoiceKeysMatchingTermNotOrderStatus(
                Constants.OrderStatus.NotFulfilled,
                page,
                itemsPerPage,
                sortBy,
                sortDirection);
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
            if (!args.ContainsKey("searchTerm")) return PerformGetPagedEntityKeys(page, itemsPerPage, sortBy, sortDirection);

            return
                    this._invoiceService.GetInvoiceKeysMatchingTermNotOrderStatus(
                        args["searchTerm"].ToString(),
                        Constants.OrderStatus.NotFulfilled,
                        page,
                        itemsPerPage,
                        sortBy,
                        sortDirection);
        }
    }
}