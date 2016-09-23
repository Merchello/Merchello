namespace Merchello.Core.Persistence.UnitOfWork
{
    using NPoco;

    /// <summary>
	/// Represents a persistence unit of work for working with a database.
	/// </summary>
	internal interface IDatabaseUnitOfWork : IUnitOfWork
	{
		Database Database { get; }

	    //void ReadLock(params int[] lockIds);
	    //void WriteLock(params int[] lockIds);
    }
}