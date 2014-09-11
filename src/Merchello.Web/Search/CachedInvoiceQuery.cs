namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core;
    using Core.Models;
    using Core.Persistence.Querying;
    using Core.Services;
    using Examine;
    using global::Examine;
    using global::Examine.Providers;
    using Models.ContentEditing;
    using Models.Querying;
    using Umbraco.Core.Persistence;

    /// <summary>
    /// Responsible for invoice related queries - caches via lucene
    /// </summary>
    internal class CachedInvoiceQuery : CachedQueryBase<IInvoice, InvoiceDisplay>, ICachedInvoiceQuery
    {
        /// <summary>
        /// The invoice service.
        /// </summary>
        private readonly InvoiceService _invoiceService;

        /// <summary>
        /// The method to retreive orders for an invoice
        /// </summary>
        private readonly Func<Guid, IEnumerable<OrderDisplay>> _getOrders;
 
        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInvoiceQuery"/> class.
        /// </summary>
        public CachedInvoiceQuery()
            : this(MerchelloContext.Current.Services.InvoiceService)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInvoiceQuery"/> class.
        /// </summary>
        /// <param name="invoiceService">
        /// The invoice service.
        /// </param>
        internal CachedInvoiceQuery(IInvoiceService invoiceService)
            : this(invoiceService, new CachedOrderQuery(MerchelloContext.Current.Services.OrderService).GetByInvoiceKey)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedInvoiceQuery"/> class.
        /// </summary>
        /// <param name="invoiceService">
        /// The invoice service.
        /// </param>
        /// <param name="getOrders">
        /// The get Orders.
        /// </param>
        internal CachedInvoiceQuery(IInvoiceService invoiceService, Func<Guid, IEnumerable<OrderDisplay>> getOrders)
            : this(
            invoiceService,
            getOrders,
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
        /// <param name="getOrders">
        /// The get Orders.
        /// </param>
        /// <param name="indexProvider">
        /// The index provider.
        /// </param>
        /// <param name="searchProvider">
        /// The search provider.
        /// </param>
        internal CachedInvoiceQuery(
            IPageCachedService<IInvoice> service, 
            Func<Guid, IEnumerable<OrderDisplay>> getOrders,
            BaseIndexProvider indexProvider, 
            BaseSearchProvider searchProvider) 
            : base(service, indexProvider, searchProvider)
        {
            _invoiceService = (InvoiceService)service;
            _getOrders = getOrders;
        }

        /// <summary>
        /// Gets the key field in index.
        /// </summary>
        protected override string KeyFieldInIndex
        {
            get { return "invoiceKey"; }
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
            return GetDisplayObject(key);
        }

        /// <summary>
        /// Searches all invoices
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by field
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        public QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches all invoices by a term
        /// </summary>
        /// <param name="term">
        /// The term.
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
        public QueryResultDisplay Search(string term, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(term, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches invoices that have invoice dates within a specified date range
        /// </summary>
        /// <param name="invoiceDateStart">
        /// The invoice date start.
        /// </param>
        /// <param name="invoiceDateEnd">
        /// The invoice date end.
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
        public QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.InvoiceDate >= invoiceDateStart && x.InvoiceDate <= invoiceDateEnd);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches invoices that have invoice dates within a specified date range with a particular invoice status
        /// </summary>
        /// <param name="invoiceDateStart">
        /// The invoice date start.
        /// </param>
        /// <param name="invoiceDateEnd">
        /// The invoice date end.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
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
        public QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, Guid invoiceStatusKey, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.InvoiceDate >= invoiceDateStart && x.InvoiceDate <= invoiceDateEnd && x.InvoiceStatusKey == invoiceStatusKey);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches invoices that have invoice dates within a specified date range with an export value
        /// </summary>
        /// <param name="invoiceDateStart">
        /// The invoice date start.
        /// </param>
        /// <param name="invoiceDateEnd">
        /// The invoice date end.
        /// </param>
        /// <param name="exported">
        /// The exported.
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
        public QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, bool exported, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.InvoiceDate >= invoiceDateStart && x.InvoiceDate <= invoiceDateEnd && x.Exported == exported);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches invoices that have invoice dates within a specified date range with a particular invoice status and export value
        /// </summary>
        /// <param name="invoiceDateStart">
        /// The invoice date start.
        /// </param>
        /// <param name="invoiceDateEnd">
        /// The invoice date end.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="exported">
        /// The exported.
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
        public QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, Guid invoiceStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.InvoiceDate >= invoiceDateStart && x.InvoiceDate <= invoiceDateEnd && x.Exported == exported && x.InvoiceStatusKey == invoiceStatusKey);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches for invoices by invoice status.
        /// </summary>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
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
        public QueryResultDisplay SearchByInvoiceStatus(Guid invoiceStatusKey, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.InvoiceStatusKey == invoiceStatusKey);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches for invoices by invoice status and exported value
        /// </summary>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="exported">
        /// The exported.
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
        public QueryResultDisplay SearchByInvoiceStatus(Guid invoiceStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.InvoiceStatusKey == invoiceStatusKey && x.Exported == exported);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches for invoices associated with a customer
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
        public QueryResultDisplay SearchByCustomer(Guid customerKey, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.CustomerKey == customerKey);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches for invoices associated with a customer and invoice status
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
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
        public QueryResultDisplay SearchByCustomer(Guid customerKey, Guid invoiceStatusKey, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.CustomerKey == customerKey && x.InvoiceStatusKey == invoiceStatusKey);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Gets the collection of all customer invoices
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The collection of customer invoices.
        /// </returns>
        public IEnumerable<InvoiceDisplay> GetByCustomerKey(Guid customerKey)
        {
            var query = Query<IInvoice>.Builder.Where(x => x.CustomerKey == customerKey);

            return GetQueryResultDisplay(_invoiceService.GetPagedKeys(query, 1, int.MaxValue, "invoiceNumber")).Items.Select(x => (InvoiceDisplay)x);
        }

        /// <summary>
        /// The re-index entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal override void ReindexEntity(IInvoice entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.Invoice);
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
            return result.ToInvoiceDisplay(_getOrders);
        }


    }
}