namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the customer registry repository
    /// </summary>
    internal interface IItemCacheRepository : IRepositoryQueryable<Guid, IItemCache>
    {
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
