namespace Merchello.Web.Search
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using Core.Persistence.Querying;
    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Defines the CachedInvoiceQuery.
    /// </summary>
    public interface ICachedInvoiceQuery
    {
        /// <summary>
        /// Gets an <see cref="InvoiceDisplay"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="InvoiceDisplay"/>.
        /// </returns>
        InvoiceDisplay GetByKey(Guid key);

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
        QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(string term, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, Guid invoiceStatusKey, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, bool exported, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay Search(DateTime invoiceDateStart, DateTime invoiceDateEnd, Guid invoiceStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "invoiceDate", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay SearchByInvoiceStatus(Guid invoiceStatusKey, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay SearchByInvoiceStatus(Guid invoiceStatusKey, bool exported, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay SearchByCustomer(Guid customerKey, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending);

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
        QueryResultDisplay SearchByCustomer(Guid customerKey, Guid invoiceStatusKey, long page, long itemsPerPage, string sortBy = "invoiceNumber", SortDirection sortDirection = SortDirection.Descending);


        /// <summary>
        /// Gets the collection of all customer invoices
        /// </summary>
        /// <param name="customerKey">
        /// The customer key.
        /// </param>
        /// <returns>
        /// The collection of customer invoices.
        /// </returns>
        IEnumerable<InvoiceDisplay> GetByCustomerKey(Guid customerKey);
    }
}