namespace Merchello.Core.Persistence
{
    using Merchello.Core.Persistence.SqlSyntax;

    using NPoco;

    /// <summary>
    /// Represents a SQL context.
    /// </summary>
    internal interface ISqlContext
    {
        /// <summary>
        /// Gets the database type.
        /// </summary>
        DatabaseType DatabaseType { get; }

        /// <summary>
        /// Gets the poco data factory.
        /// </summary>
        IPocoDataFactory PocoDataFactory { get; }

        /// <summary>
        /// Gets the sql syntax.
        /// </summary>
        ISqlSyntaxProvider SqlSyntax { get; }
    }
}