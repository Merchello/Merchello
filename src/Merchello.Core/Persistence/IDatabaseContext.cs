namespace Merchello.Core.Persistence
{
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <summary>
    /// Represents the database.
    /// </summary>
    /// <remarks>One per AppDomain. Ensures that the database is available.</remarks>
    internal interface IDatabaseContext
    {
        /// <summary>
        /// Gets a value indicating whether a connection can be made to the database.
        /// </summary>
        bool CanConnect { get; }

        /// <summary>
        /// Gets the database.
        /// </summary>
        Database Database { get; }

        /// <summary>
        /// Gets a value indicating whether the database is configured.
        /// </summary>
        bool IsDatabaseConfigured { get; }

        /// <summary>
        /// Gets the factory responsible for translating entity queries.
        /// </summary>
        IQueryFactory QueryFactory { get; }

        /// <summary>
        /// Gets the the sql syntax provider.
        /// </summary>
        ISqlSyntaxProvider SqlSyntax { get; }
    }
}