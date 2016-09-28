namespace Merchello.Core.Persistence.Querying
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    /// Represents a query for building lambda expression translated SQL queries
    /// </summary>
    /// <typeparam name="T">The entity type</typeparam>
    public interface IQuery<T>
    {
        /// <summary>
        /// Adds a where clause to the query
        /// </summary>
        /// <param name="predicate">
        /// The predicate
        /// </param>
        /// <returns>This instance so calls to this method are chainable</returns>
        IQuery<T> Where(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Returns all translated where clauses and their sql parameters
        /// </summary>
        /// <returns>
        /// A tuple containing translated where clauses and their SQL parameters.
        /// </returns>
        IEnumerable<Tuple<string, object[]>> GetWhereClauses();
    }
}