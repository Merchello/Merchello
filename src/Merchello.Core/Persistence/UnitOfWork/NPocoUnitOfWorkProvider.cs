namespace Merchello.Core.Persistence.UnitOfWork
{
    using Merchello.Core.Persistence;

    /// <summary>
    /// Represents a <see cref="IDatabaseUnitOfWork"/> provider that creates <see cref="NPocoUnitOfWork"/> instances.
    /// </summary>
    internal class NPocoUnitOfWorkProvider : IDatabaseUnitOfWorkProvider
    {
        /// <summary>
        /// The <see cref="IDatabaseFactory"/>.
        /// </summary>
        private readonly IDatabaseFactory _databaseFactory;

        /// <summary>
        /// The <see cref="RepositoryFactory"/>.
        /// </summary>
        private readonly RepositoryFactory _repositoryFactory;

        /// <summary>
        /// Initializes a new instance of the <see cref="NPocoUnitOfWorkProvider"/> class with a database factory and a repository factory.
        /// </summary>
        /// <param name="databaseFactory">A database factory.</param>
        /// <param name="repositoryFactory">A repository factory.</param>
        public NPocoUnitOfWorkProvider(IDatabaseFactory databaseFactory, RepositoryFactory repositoryFactory)
        {
            Ensure.ParameterNotNull(databaseFactory, nameof(databaseFactory));
            Ensure.ParameterNotNull(repositoryFactory, nameof(repositoryFactory));
            this._databaseFactory = databaseFactory;
            this._repositoryFactory = repositoryFactory;
        }

        #region Implement IUnitOfWorkProvider

        /// <summary>
        /// Creates a unit of work around a database obtained from the database factory.
        /// </summary>
        /// <returns>A unit of work.</returns>
        /// <remarks>The unit of work will execute on the database returned by the database factory.</remarks>
        public IDatabaseUnitOfWork CreateUnitOfWork()
        {
            // get a database from the factory - might be the "ambient" database eg
            // the one that's enlisted with the HttpContext - so it's not always a
            // "new" database.
            var database = this._databaseFactory.GetDatabase();
            return new NPocoUnitOfWork(database, this._repositoryFactory);
        }

        #endregion
    }
}