namespace Merchello.Core.Persistence
{
    using System;
    using System.Threading;

    using Merchello.Core.Acquired.Threading;
    using Merchello.Core.Persistence.Querying;

    using NPoco;

    /// <summary>
    /// Represents a base database factory used to adapt CMS database factories.
    /// </summary>
    internal abstract class DatabaseFactoryBase : IDatabaseFactory
    {
        /// <summary>
        /// The locker.
        /// </summary>
        private readonly ReaderWriterLockSlim _lock = new ReaderWriterLockSlim();

        /// <summary>
        /// The Merchello's query factory.
        /// </summary>
        private readonly IQueryFactory _queryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="DatabaseFactoryBase"/> class.
        /// </summary>
        /// <param name="queryFactory">
        /// The query factory.
        /// </param>
        protected DatabaseFactoryBase(IQueryFactory queryFactory)
        {
            this._queryFactory = queryFactory;
        }

        /// <summary>
        /// Gets a value indicating whether the database is configured.
        /// </summary>
        public abstract bool Configured { get; }

        /// <summary>
        /// Gets a value indicating whether a connection can be made to the database.
        /// </summary>
        public abstract bool CanConnect { get; }

        /// <summary>
        /// Gets the factory responsible for translating entity queries.
        /// </summary>
        public IQueryFactory QueryFactory
        {
            get
            {
                EnsureConfigured();
                return _queryFactory;
            }
        }

        /// <summary>
        /// Gets the database.
        /// </summary>
        /// <returns>
        /// The <see cref="Database"/>.
        /// </returns>
        public abstract IMerchelloDatabase GetDatabase();

        /// <summary>
        /// Disposes resources.
        /// </summary>
        public abstract void Dispose();

        /// <summary>
        /// Ensures the database is configured.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// Throws exception if the database is not configured.
        /// </exception>
        private void EnsureConfigured()
        {
            using (new ReadLock(_lock))
            {
                if (Configured == false)
                    throw new InvalidOperationException("Not configured.");
            }
        }
    }
}