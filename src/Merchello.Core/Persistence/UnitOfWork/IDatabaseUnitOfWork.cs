using System;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence.UnitOfWork
{
    /// <summary>
    /// Defines a unit of work when working with a database object
    /// </summary>
    /// <remarks>
    /// This is required due to Umbraco's IUnitOfWork dependency on Umbraco.Core.Models.EntityBase.IEntity
    /// </remarks>
    public interface IDatabaseUnitOfWork : IUnitOfWork, IDisposable
    {
        UmbracoDatabase Database { get; }
    }
}