namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;
    using Core.Persistence.Querying;
    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Defines the CachedInvoiceQuery.
    /// </summary>
    public interface ICachedOrderQuery
    {
        /// <summary>
        /// Gets an <see cref="OrderDisplay"/> by it's key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="OrderDisplay"/>.
        /// </returns>
        OrderDisplay GetByKey(Guid key);

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
        QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "orderNumber", SortDirection sortDirection = SortDirection.Descending);
        
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
        QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, long page, long itemsPerPage, string sortBy = "orderDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, Guid orderStatusKey, long page, long itemsPerPage, string sortBy = "ordereDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, bool exported, long page, long itemsPerPage, string sortBy = "orderDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(DateTime orderDateStart, DateTime orderDateEnd, Guid orderStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "orderDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay SearchByOrderStatus(Guid orderStatusKey, long page, long itemsPerPage, string sortBy = "orderNumber", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay SearchByOrderStatus(Guid orderStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "orderNumber", SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets a collection of orders by the invoice key.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <returns>
        /// The collection of orders for an invoice.
        /// </returns>
        IEnumerable<OrderDisplay> GetByInvoiceKey(Guid invoiceKey);
    }
}