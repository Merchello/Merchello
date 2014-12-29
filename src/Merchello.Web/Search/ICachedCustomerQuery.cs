namespace Merchello.Web.Search
{
    using System;
    using Core.Persistence.Querying;
    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Defines a CachedCustomerQuery
    /// </summary>
    public interface ICachedCustomerQuery
    {
        /// <summary>
        /// Gets an <see cref="CustomerDisplay"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="CustomerDisplay"/>.
        /// </returns>
        CustomerDisplay GetByKey(Guid key);

        /// <summary>
        /// Searches all customers
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
        QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "loginName", SortDirection sortDirection = SortDirection.Ascending);

        /// <summary>
        /// Searches all customers by a term
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
        QueryResultDisplay Search(string term, long page, long itemsPerPage, string sortBy = "loginName", SortDirection sortDirection = SortDirection.Ascending);

        /// <summary>
        /// Searches customers that have customer dates within a specified date range
        /// </summary>
        /// <param name="lastActivityDateStart">
        /// The customer date start.
        /// </param>
        /// <param name="lastActivityDateEnd">
        /// The customer date end.
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
        QueryResultDisplay Search(DateTime lastActivityDateStart, DateTime lastActivityDateEnd, long page, long itemsPerPage, string sortBy = "loginName", SortDirection sortDirection = SortDirection.Ascending);
    }
}