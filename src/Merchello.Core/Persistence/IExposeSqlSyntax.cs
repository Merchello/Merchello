namespace Merchello.Core.Persistence
{
    using Merchello.Core.Persistence.SqlSyntax;

    /// <summary>
    /// Represents a class that exposes a SqlSyntaxProvider.
    /// </summary>
    public interface IExposeSqlSyntax
    {
        /// <summary>
        /// Gets the provider responsible for ensuring SQL is generated in the correct syntax.
        /// </summary>
        ISqlSyntaxProviderAdapter SqlSyntax { get; } 
    }
}