namespace Merchello.Web.Search
{
    using System;

    using Core.Models;
    using Core.Services;
    using global::Examine;
    using global::Examine.Providers;

    using Merchello.Core;
    using Merchello.Core.Cache;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Examine;
    using Merchello.Web.Models.Querying;

    using Models.ContentEditing;

    /// <summary>
    /// Responsible for invoice related queries - caches via lucene
    /// </summary>
    internal class CachedInvoiceQuery : CachedQueryBase<IInvoice, InvoiceDisplay>
    {
        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly InvoiceService _invoiceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInvoiceQuery"/> class.
        /// </summary>
        public CachedInvoiceQuery() : this(
            MerchelloContext.Current.Services.InvoiceService,
            ExamineManager.Instance.IndexProviderCollection["MerchelloInvoiceIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloInvoiceSearcher"])
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInvoiceQuery"/> class.
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
        internal CachedInvoiceQuery(
            IPageCachedService<IInvoice> service, 
            BaseIndexProvider indexProvider, 
            BaseSearchProvider searchProvider) 
            : base(service, indexProvider, searchProvider)
        {
            _invoiceService = (InvoiceService)service;
        }

        /// <summary>
        /// Gets an display class by it's unique by key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        public override InvoiceDisplay GetByKey(Guid key)
        {
            return this.GetDisplayObject("invoiceKey", key);
        }

        /// <summary>
        /// The get by customer key.
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
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
        public QueryResultDisplay GetByCustomerKey(Guid customerKey, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending)
        {
            var results = _invoiceService.GetPageByCustomerKey(customerKey, page, itemsPerPage, sortBy, sortDirection);
            return this.GetQueryResultDisplay(results, "invoiceKey");
        }

        /// <summary>
        /// Maps a <see cref="SearchResult"/> to <see cref="InvoiceDisplay"/>
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        protected override InvoiceDisplay PerformMapSearchResultToDisplayObject(SearchResult result)
        {
            return result.ToInvoiceDisplay();
        }

        /// <summary>
        /// The re-index entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        protected override void ReindexEntity(IInvoice entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.Invoice);
        }
    }
}