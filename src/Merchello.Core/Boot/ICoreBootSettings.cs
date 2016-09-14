namespace Merchello.Core.Boot
{
    using Merchello.Core.Acquired.Persistence.SqlSyntax;
    using Merchello.Core.Logging;

    /// <summary>
    /// Defines settings required for the <see cref="CoreBootManager"/>.
    /// </summary>
    internal interface ICoreBootSettings
    {
        /// <summary>
        /// Gets or sets the <see cref="ILogger"/>.
        /// </summary>
        ILogger Logger { get; set; }
        
        /// <summary>
        /// Gets or sets the SQL syntax provider.
        /// </summary>
        ISqlSyntaxProvider SqlSyntaxProvider { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the boot manager is being used in tests.
        /// </summary>
        bool IsForTesting { get; set; }
    }
}