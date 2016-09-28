namespace Merchello.Core.Persistence.Repositories
{
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Querying;

    /// <summary>
    /// Represents a queryable repository.
    /// </summary>
    /// <typeparam name="TEntity">
    /// The type of entity.
    /// </typeparam>
    public interface IRepositoryQueryable<TEntity>
    {
        /// <summary>
        /// Gets the query instance
        /// </summary>
        IQuery<TEntity> Query { get; }

        /// <summary>
        /// Gets the query factory.
        /// </summary>
        IQueryFactory QueryFactory { get; }

        /// <summary>
        /// Gets all entities of the specified type and query
        /// </summary>
        /// <param name="query">
        /// The query
        /// </param>
        /// <returns>
        /// The collection of entities
        /// </returns>
        IEnumerable<TEntity> GetByQuery(IQuery<TEntity> query);

        /// <summary>
        /// Gets the count for the specified query
        /// </summary>
        /// <param name="query">The query</param>
        /// <returns>The count</returns>
        int Count(IQuery<TEntity> query);
    }
}