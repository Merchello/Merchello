namespace Merchello.Core.Persistence
{
    using NPoco;

    /// <summary>
    /// Represents the Merchello Database.
    /// </summary>
    internal interface IMerchelloDatabase : IExposeSqlSyntax
    {
        /// <summary>
        /// Gets the NPoco database.
        /// </summary>
        Database Database { get; }
    }
}