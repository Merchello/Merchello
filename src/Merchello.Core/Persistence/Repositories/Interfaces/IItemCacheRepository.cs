namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the customer registry repository
    /// </summary>
    internal interface IItemCacheRepository : IRepositoryQueryable<Guid, IItemCache>
    {
            /// <summary>
        /// Gets a page of <see cref="IItemCache"/>
        /// </summary>
        /// <param name="itemCacheTfKey">
        /// The item cache type.
        /// </param>
        /// <param name="startDate">
        /// The start Date.
        /// </param>
        /// <param name="endDate">
        /// The end Date.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The sort by field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IItemCache}"/>.
        /// </returns>
        Page<IItemCache> GetCustomerItemCachePage(
            Guid itemCacheTfKey,
            DateTime startDate,
            DateTime endDate, 
            long page, 
            long itemsPerPage, 
            string orderExpression, 
            SortDirection sortDirection = SortDirection.Descending);


        /// <summary>
        /// Gets the count of of item caches for a customer type for a given date range.
        /// </summary>
        /// <param name="itemCacheTfKey">
        /// The item cache type field key.
        /// </param>
        /// <param name="customerType">
        /// The customer type.
        /// </param>
        /// <param name="startDate">
        /// The start Date.
        /// </param>
        /// <param name="endDate">
        /// The end Date.
        /// </param>
        /// <returns>
        /// The count of item caches.
        /// </returns>
        int Count(Guid itemCacheTfKey, CustomerType customerType, DateTime startDate, DateTime endDate);
    }
}
