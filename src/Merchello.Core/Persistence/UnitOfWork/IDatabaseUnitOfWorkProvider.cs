namespace Merchello.Core.Persistence.UnitOfWork
{
	/// <summary>
	/// Defines a Unit of Work Provider for working with an IDatabaseUnitOfWork
	/// </summary>
	/// <remarks>
    /// This is required due to Umbraco's IDatabaseUnitOfWork dependency on IUnitOfWork which references Umbraco
    /// specific entities.  Umbraco.Core.Models.EntityBase.IEntity
	/// </remarks>
	public interface IDatabaseUnitOfWorkProvider
	{
	    /// <summary>
	    /// Gets the database unit of work.
	    /// </summary>
	    /// <returns>
	    /// The <see cref="IDatabaseUnitOfWork"/>.
	    /// </returns>
	    IDatabaseUnitOfWork GetUnitOfWork();
	}
}