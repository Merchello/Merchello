namespace Merchello.Core.Boot
{
    using Merchello.Core.Acquired.Persistence.SqlSyntax;
    using Merchello.Core.Cache;
    using Merchello.Core.Logging;

    /// <inheritdoc/>
    internal class CoreBootSettings : ICoreBootSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CoreBootSettings"/> class.
        /// </summary>
        public CoreBootSettings()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CoreBootSettings"/> class with a logger.
        /// </summary>
        /// <param name="isForTesting">
        /// A value indicating that this startup is being used by tests
        /// </param>
        public CoreBootSettings(bool isForTesting = false)
        {
            this.IsForTesting = isForTesting;
        }

        /// <inheritdoc/>
        public ILogger Logger { get; set; }

        /// <inheritdoc/>
        public ISqlSyntaxProvider SqlSyntaxProvider { get; set; }

        /// <inheritdoc/>
        public bool IsForTesting { get; set; }
    }
}