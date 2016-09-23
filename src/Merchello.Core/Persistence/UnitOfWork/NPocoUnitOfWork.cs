namespace Merchello.Core.Persistence.UnitOfWork
{
    using Merchello.Core.Persistence;

    using NPoco;

    /// <summary>
    /// Implements IDatabaseUnitOfWork for NPoco.
    /// </summary>
    internal class NPocoUnitOfWork : UnitOfWorkBase, IDatabaseUnitOfWork
    {
        /// <summary>
        /// The transaction.
        /// </summary>
        private ITransaction _transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="NPocoUnitOfWork"/> class with a database and a repository factory.
        /// </summary>
        /// <param name="database">A database.</param>
        /// <param name="factory">A repository factory.</param>
        /// <remarks>This should be used by the NPocoUnitOfWorkProvider exclusively.</remarks>
        internal NPocoUnitOfWork(Database database, RepositoryFactory factory)
            : base(factory)
        {
            this.Database = database;
        }

        /// <summary>
        /// Gets the unit of work underlying database.
        /// </summary>
        public Database Database { get; }

        /// <summary>
        /// Creates a repository.
        /// </summary>
        /// <typeparam name="TRepository">The type of the repository.</typeparam>
        /// <param name="name">The optional name of the repository.</param>
        /// <returns>The created repository for the unit of work.</returns>
        public override TRepository CreateRepository<TRepository>(string name = null)
        {
            return this.Factory.CreateRepository<TRepository>(this, name);
        }

        /// <summary>
        /// Ensures that we have a transaction.
        /// </summary>
        public override void Begin()
        {
            base.Begin();

            if (this._transaction == null)
                this._transaction = this.Database.GetTransaction();
        }

        /// <summary>
        /// Disposes the resources.
        /// </summary>
        protected override void DisposeResources()
        {
            base.DisposeResources();

            // no transaction, nothing to do
            if (this._transaction == null) return;

            // will either complete or abort NPoco transaction
            // which means going one level up in the transaction stack
            // and commit or rollback only if at top of stack
            if (this.Completed)
                this._transaction.Complete(); // complete the transaction
            else
                this._transaction.Dispose(); // abort the transaction

            this._transaction = null;
        }
    }
}