namespace Merchello.Examine.DataServices
{
    using Umbraco.Core.Logging;

    /// <summary>
    /// The data service base.
    /// </summary>
    internal abstract class DataServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataServiceBase"/> class.
        /// </summary>
        protected DataServiceBase()
        {
            DataServiceLogger = Logger.CreateWithDefaultLog4NetConfiguration();
        }

        /// <summary>
        /// Gets the data service logger.
        /// </summary>
        public ILogger DataServiceLogger { get; private set; } 
    }
}