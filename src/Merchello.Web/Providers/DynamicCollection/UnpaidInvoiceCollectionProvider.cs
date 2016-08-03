namespace Merchello.Web.Providers.DynamicCollection
{
    using System;

    using Merchello.Core;
    using Merchello.Core.EntityCollections;
    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// The unpaid invoice collection provider.
    /// </summary>
    internal class UnpaidInvoiceCollectionProvider : CachedEntityCollectionProviderBase<IInvoice>
    {
        /// <summary>
        /// The <see cref="MerchelloHelper"/>
        /// </summary>
        private readonly MerchelloHelper _merchello;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnpaidInvoiceCollectionProvider"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        public UnpaidInvoiceCollectionProvider(IMerchelloContext merchelloContext, Guid collectionKey)
            : base(merchelloContext, collectionKey)
        {
            _merchello = new MerchelloHelper(merchelloContext, false);
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
            return entity.InvoiceStatusKey.Equals(Constants.DefaultKeys.InvoiceStatus.Unpaid);
        }

        /// <summary>
        /// Gets a page of unpaid invoices
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
            throw new NotImplementedException();
        }

        protected override Page<Guid> PerformGetPagedEntityKeys(
            long page,
            long itemsPerPage,
            string sortBy = "",
            SortDirection sortDirection = SortDirection.Ascending)
        {
            throw new NotImplementedException();
        }
    }
}