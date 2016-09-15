namespace Merchello.Core.Persistence.Querying
{
    using Merchello.Core.Persistence.Mappers;
    using Merchello.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents a query factory.
    /// </summary>
    internal class QueryFactory : IQueryFactory
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QueryFactory"/> class.
        /// </summary>
        /// <param name="sqlSyntax">
        /// The sql syntax.
        /// </param>
        /// <param name="mappingResolver">
        /// The mapping resolver.
        /// </param>
        public QueryFactory(ISqlSyntaxProvider sqlSyntax, IMappingResolver mappingResolver)
        {
            SqlSyntax = sqlSyntax;
            MappingResolver = mappingResolver;
        }

        /// <summary>
        /// Gets the resolver for mapping properties between entities and DTO (POCO) classes.
        /// </summary>
        public IMappingResolver MappingResolver { get; }

        /// <summary>
        /// Gets the sql syntax provider.
        /// </summary>
        public ISqlSyntaxProvider SqlSyntax { get; }

        /// <summary>
        /// Creates a lambda expression query.
        /// </summary>
        /// <typeparam name="T">
        /// The type of entity
        /// </typeparam>
        /// <returns>
        /// The <see cref="IQuery{T}"/>.
        /// </returns>
        public IQuery<T> Create<T>()
        {
            return new Query<T>(SqlSyntax, MappingResolver);
        }
    }
}

