namespace Merchello.Core.Services
{
    using System;

    using Merchello.Core.Persistence;
    using Merchello.Core.Persistence.UnitOfWork;

    using Umbraco.Core.Events;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The merchello repository service.
    /// </summary>
    public abstract class MerchelloRepositoryService
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MerchelloRepositoryService"/> class.
        /// </summary>
        /// <param name="provider">
        /// The provider.
        /// </param>
        /// <param name="repositoryFactory">
        /// The repository factory.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        /// <param name="eventMessagesFactory">
        /// The event messages factory.
        /// </param>
        protected MerchelloRepositoryService(
            IDatabaseUnitOfWorkProvider provider,
            RepositoryFactory repositoryFactory,
            ILogger logger,
            IEventMessagesFactory eventMessagesFactory)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            if (repositoryFactory == null) throw new ArgumentNullException("repositoryFactory");
            if (logger == null) throw new ArgumentNullException("logger");
            if (eventMessagesFactory == null) throw new ArgumentNullException("eventMessagesFactory");
            Logger = logger;
            EventMessagesFactory = eventMessagesFactory;
            RepositoryFactory = repositoryFactory;
            UowProvider = provider;
        }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Gets the event messages factory.
        /// </summary>
        protected IEventMessagesFactory EventMessagesFactory { get; private set; }

        /// <summary>
        /// Gets the repository factory.
        /// </summary>
        protected RepositoryFactory RepositoryFactory { get; private set; }

        /// <summary>
        /// Gets the UOW provider.
        /// </summary>
        protected IDatabaseUnitOfWorkProvider UowProvider { get; private set; }
    }
}