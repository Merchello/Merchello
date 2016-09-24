namespace Merchello.Core.Persistence.UnitOfWork
{
    using NPoco;

    /// <summary>
	/// Represents a persistence unit of work for working with a database.
	/// </summary>
	internal interface IDatabaseUnitOfWork : IUnitOfWork
	{
        /// <summary>
        /// Gets the database.
        /// </summary>
        Database Database { get; }

        /// <summary>
        /// Read lock to prevent dirty read.
        /// </summary>
        /// <param name="lockIds">
        /// The lock ids.
        /// </param>
        void ReadLock(params int[] lockIds);

        /// <summary>
        /// Database write lock.
        /// </summary>
        /// <param name="lockIds">
        /// The lock ids.
        /// </param>
        void WriteLock(params int[] lockIds);
    }
}