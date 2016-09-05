namespace Merchello.Umbraco
{
    using Merchello.Core.Logging;
    using Merchello.Web;

    /// <summary>
    /// Starts the Merchello Umbraco CMS Package.
    /// </summary>
    internal class BootManager : WebBootManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BootManager"/> class.
        /// </summary>
        public BootManager()
            : this(Logger.CreateWithDefaultLog4NetConfiguration(), null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BootManager"/> class.
        /// </summary>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="sqlSyntax">
        /// The SQL syntax.
        /// </param>
        /// <param name="isForTesting">
        /// The is for testing.
        /// </param>
        public BootManager(ILogger logger, object sqlSyntax, bool isForTesting = false)
            : base(logger, sqlSyntax, isForTesting)
        {
        }

    }
}