namespace Merchello.Core.Adapters
{
    using System;

    using Merchello.Core.Logging;
    using Merchello.Core.Persistence;

    /// <summary>
    /// Represents a base class used to adapt a database context provided by the CMS.
    /// </summary>
    internal abstract class DatabaseContextAdapterBase
    {
        /// <summary>
        /// The database factory.
        /// </summary>
        private readonly IDatabaseFactory _factory;

        /// <summary>
        /// The logger.
        /// </summary>
        private readonly ILogger _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseContextAdapterBase"/> class.
        /// </summary>
        /// <param name="factory">
        /// The database factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// Throws exceptions if either the factory or logger is null
        /// </exception>
        protected DatabaseContextAdapterBase(IDatabaseFactory factory, ILogger logger)
        {
            if (factory == null) throw new ArgumentNullException(nameof(factory));
            if (logger == null) throw new ArgumentNullException(nameof(logger));

            _factory = factory;
            _logger = logger;
        }
    }
}