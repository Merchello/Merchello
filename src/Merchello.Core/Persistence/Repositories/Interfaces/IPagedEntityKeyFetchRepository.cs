namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Linq.Expressions;

    using Merchello.Core.Models.Rdbms;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Querying;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// The PagedEntityKeyFetchRepository interface.
    /// </summary>
    /// <typeparam name="TId">
    /// </typeparam>
    /// <typeparam name="TEntity">
    /// </typeparam>
    internal interface IPagedEntityKeyFetchRepository<TId, TEntity> : IRepositoryQueryable<TId, TEntity>
    {
        /// <summary>
        /// The get paged keys.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="query">
        /// The query.
        /// </param>
        /// <param name="orderExpression">
        /// The field to order by
        /// </param>
        /// <param name="sortDirection">
        /// The sort Direction.
        /// </param>
        /// <returns>
        /// The paged result.
        /// </returns>
        Page<TId> GetPagedKeys(long page, long itemsPerPage, IQuery<TEntity> query, string orderExpression, SortDirection sortDirection = SortDirection.Descending);
    }
}