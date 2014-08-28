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
    /// Represents a CachedOrderQuery.
    /// </summary>
    internal class CachedOrderQuery : CachedQueryBase<IOrder, OrderDisplay>, ICachedOrderQuery
    {
        /// <summary>
        /// The order service.
        /// </summary>
        private readonly OrderService _orderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedOrderQuery"/> class.
        /// </summary>
        public CachedOrderQuery()
            : this(MerchelloContext.Current.Services.OrderService)
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedOrderQuery"/> class.
        /// </summary>
        /// <param name="orderService">
        /// The order service.
        /// </param>
        internal CachedOrderQuery(IOrderService orderService)
            : this(
            orderService,
            ExamineManager.Instance.IndexProviderCollection["MerchelloOrderIndexer"],
            ExamineManager.Instance.SearchProviderCollection["MerchelloOrderSearcher"])
        {            
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedOrderQuery"/> class.
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
        internal CachedOrderQuery(
            IPageCachedService<IOrder> service, 
            BaseIndexProvider indexProvider, 
            BaseSearchProvider searchProvider)
            : base(service, indexProvider, searchProvider)
        {
            _orderService = (OrderService)service;
        }

        /// <summary>
        /// Gets the key field in index.
        /// </summary>
        protected override string KeyFieldInIndex
        {
            get { return "orderKey"; }
        }

        /// <summary>
        /// Gets an <see cref="OrderDisplay"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="OrderDisplay"/>.
        /// </returns>
        public override OrderDisplay GetByKey(Guid key)
        {
            return GetDisplayObject(key);
        }

        /// <summary>
        /// Searches all orders
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
        public QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "orderNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            return GetQueryResultDisplay(_orderService.GetPagedKeys(page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches orders that have order dates within a specified date range
        /// </summary>
        /// <param name="orderDateStart">
        /// The order date start.
        /// </param>
        /// <param name="orderDateEnd">
        /// The order date end.
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
        public QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, long page, long itemsPerPage, string sortBy = "orderDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IOrder>.Builder.Where(x => x.OrderDate >= orderDateStart && x.OrderDate <= orderDateEnd);

            return GetQueryResultDisplay(_orderService.GetPage(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches order that have order dates within a specified date range with a particular order status
        /// </summary>
        /// <param name="orderDateStart">
        /// The order date start.
        /// </param>
        /// <param name="orderDateEnd">
        /// The order date end.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, Guid orderStatusKey, long page, long itemsPerPage, string sortBy = "ordereDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IOrder>.Builder.Where(x => x.OrderDate >= orderDateStart && x.OrderDate <= orderDateEnd && x.OrderStatusKey == orderStatusKey);

            return GetQueryResultDisplay(_orderService.GetPage(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches orders that have order dates within a specified date range with an export value
        /// </summary>
        /// <param name="orderDateStart">
        /// The order date start.
        /// </param>
        /// <param name="orderDateEnd">
        /// The order date end.
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
        public QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, bool exported, long page, long itemsPerPage, string sortBy = "orderDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IOrder>.Builder.Where(x => x.OrderDate >= orderDateStart && x.OrderDate <= orderDateEnd && x.Exported == exported);

            return GetQueryResultDisplay(_orderService.GetPage(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches orders that have order dates within a specified date range with a particular order status and export value
        /// </summary>
        /// <param name="orderDateStart">
        /// The order date start.
        /// </param>
        /// <param name="orderDateEnd">
        /// The order date end.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, Guid orderStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "orderDate", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IOrder>.Builder.Where(x => x.OrderDate >= orderDateStart && x.OrderDate <= orderDateEnd && x.OrderStatusKey == orderStatusKey && x.Exported == exported);

            return GetQueryResultDisplay(_orderService.GetPage(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches for orders by order status.
        /// </summary>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public QueryResultDisplay SearchByOrderStatus(Guid orderStatusKey, long page, long itemsPerPage, string sortBy = "orderNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IOrder>.Builder.Where(x => x.OrderStatusKey == orderStatusKey);

            return GetQueryResultDisplay(_orderService.GetPage(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Searches for orders by order status and exported value
        /// </summary>
        /// <param name="orderStatusKey">
        /// The order status key.
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
        public QueryResultDisplay SearchByOrderStatus(Guid orderStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "orderNumber", SortDirection sortDirection = SortDirection.Descending)
        {
            var query = Query<IOrder>.Builder.Where(x => x.OrderStatusKey == orderStatusKey && x.Exported == exported);

            return GetQueryResultDisplay(_orderService.GetPage(query, page, itemsPerPage, sortBy, sortDirection));
        }

        /// <summary>
        /// Gets a collection of orders by the invoice key.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The collection of orders for an invoice.
        /// </returns>
        public IEnumerable<OrderDisplay> GetByInvoiceKey(Guid invoiceKey)
        {
            var query = Query<IOrder>.Builder.Where(x => x.InvoiceKey == invoiceKey);

            return GetQueryResultDisplay(_orderService.GetPage(query, 1, int.MaxValue, "orderNumber")).Items.Select(x => (OrderDisplay)x);
        }

        /// <summary>
        /// The re-index entity.
        /// </summary>
        /// <param name="entity">
        /// The entity.
        /// </param>
        internal override void ReindexEntity(IOrder entity)
        {
            IndexProvider.ReIndexNode(entity.SerializeToXml().Root, IndexTypes.Order);
        }

        /// <summary>
        /// The perform map search result to display object.
        /// </summary>
        /// <param name="result">
        /// The result.
        /// </param>
        /// <returns>
        /// The <see cref="OrderDisplay"/>.
        /// </returns>
        protected override OrderDisplay PerformMapSearchResultToDisplayObject(SearchResult result)
        {
            return result.ToOrderDisplay();
        }

    }
}