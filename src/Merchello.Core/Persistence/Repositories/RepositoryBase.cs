namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Cache;
    using Merchello.Core.Logging;
    using Merchello.Core.Persistence.Querying;
    using Merchello.Core.Persistence.UnitOfWork;

    /// <summary>
    /// Represents a repository.
    /// </summary>
    internal abstract class RepositoryBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryBase"/> class.
        /// </summary>
        /// <param name="work">
        /// The unit of work.
        /// </param>
        /// <param name="cache">
        /// The cache.
        /// </param>
        /// <param name="logger">
        /// The logger.
        /// </param>
        protected RepositoryBase(IUnitOfWork work, ICacheHelper cache, ILogger logger)
        {
            if (work == null) throw new ArgumentNullException(nameof(work));
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            Logger = logger;
            UnitOfWork = work;
            RepositoryCache = cache;
        }

        /// <summary>
        /// Gets the Unit of Work added to the repository
        /// </summary>
        protected internal IUnitOfWork UnitOfWork { get; }

        /// <summary>
        /// Gets the logger.
        /// </summary>
        protected ILogger Logger { get; private set; }

        /// <summary>
        /// Gets the repository cache.
        /// </summary>
        protected ICacheHelper RepositoryCache { get; }

        /// <summary>
        /// Gets the runtime cache used for this repo - by standard this is the runtime cache exposed by the CacheHelper but can be overridden
        /// </summary>
        protected virtual IRuntimeCacheProvider RuntimeCache => RepositoryCache.RuntimeCache;
    }
}