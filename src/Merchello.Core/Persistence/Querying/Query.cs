namespace Merchello.Core.Persistence.Querying
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    using Merchello.Core.Acquired.Persistence.Mappers;
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents a query.
    /// </summary>
    /// <typeparam name="T">
    /// The type of entity to query.
    /// </typeparam>
    internal class Query<T> : IQuery<T>
    {
        /// <summary>
        /// The sql syntax provider.
        /// </summary>
        private readonly ISqlSyntaxProvider _sqlSyntax;

        /// <summary>
        /// The Merchello's mapping resolver.
        /// </summary>
        private readonly IMappingResolver _mappingResolver;

        /// <summary>
        /// The translated list of where clauses.
        /// </summary>
        private readonly List<Tuple<string, object[]>> _wheres = new List<Tuple<string, object[]>>();

        /// <summary>
        /// Initializes a new instance of the <see cref="Query{T}"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The sql syntax provider.
        /// </param>
        /// <param name="mappingResolver">
        /// The Merchello's mapping resolver.
        /// </param>
        public Query(ISqlSyntaxProvider sqlSyntax, IMappingResolver mappingResolver)
        {
            _sqlSyntax = sqlSyntax;
            _mappingResolver = mappingResolver;
        }

        /// <summary>
        /// Adds a where clause to the query
        /// </summary>
        /// <param name="predicate">A lambda expression</param>
        /// <returns>This instance so calls to this method are chainable</returns>
        public virtual IQuery<T> Where(Expression<Func<T, bool>> predicate)
        {
            if (predicate != null)
            {
                var expressionHelper = new ModelToSqlExpressionHelper<T>(_sqlSyntax, _mappingResolver);
                string whereExpression = expressionHelper.Visit(predicate);

                _wheres.Add(new Tuple<string, object[]>(whereExpression, expressionHelper.GetSqlParameters()));
            }
            return this;
        }

        /// <summary>
        /// Returns all translated where clauses and their sql parameters
        /// </summary>
        /// <returns>
        /// The translated where clauses.
        /// </returns>
        public IEnumerable<Tuple<string, object[]>> GetWhereClauses()
        {
            return _wheres;
        }
    }
}