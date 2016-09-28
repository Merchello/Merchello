namespace Merchello.Core.Persistence.UnitOfWork
{
    using System;
    using System.Data;

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
        internal NPocoUnitOfWork(IMerchelloDatabase database, RepositoryFactory factory)
            : base(factory)
        {
            this.Database = database;
        }

        /// <summary>
        /// Gets the unit of work underlying database.
        /// </summary>
        public IMerchelloDatabase Database { get; }

        /// <inheritdoc/>
        public void ReadLock(params int[] lockIds)
        {
            Begin(); // we need a transaction

            if (Database.Database.Transaction.IsolationLevel < IsolationLevel.RepeatableRead)
                throw new InvalidOperationException("A transaction with minimum RepeatableRead isolation level is required.");

            // *not* using a unique 'WHERE IN' query here because the *order* of lockIds is important to avoid deadlocks
            foreach (var lockId in lockIds)
            {
                var i = Database.Database.ExecuteScalar<int?>(
                    "SELECT value FROM merchLock WHERE id=@id",
                    new { @id = lockId });
                if (i == null) // ensure we are actually locking!
                    throw new Exception($"LockObject with id={lockId} does not exist.");
            }
        }

        /// <inheritdoc/>
        public void WriteLock(params int[] lockIds)
        {
            Begin(); // we need a transaction

            if (Database.Database.Transaction.IsolationLevel < IsolationLevel.RepeatableRead)
                throw new InvalidOperationException("A transaction with minimum RepeatableRead isolation level is required.");

            // *not* using a unique 'WHERE IN' query here because the *order* of lockIds is important to avoid deadlocks
            foreach (var lockId in lockIds)
            {
                var i = Database.Database.Execute(
                    "UPDATE merchLock SET value = (CASE WHEN (value=1) THEN -1 ELSE 1 END) WHERE id=@id",
                    new { @id = lockId });
                if (i == 0) // ensure we are actually locking!
                    throw new Exception($"LockObject with id={lockId} does not exist.");
            }
        }

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
                this._transaction = this.Database.Database.GetTransaction();
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