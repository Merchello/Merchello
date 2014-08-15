namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using Models;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the Invoice Status Repository
    /// </summary>
    internal interface IInvoiceStatusRepository : IRepositoryQueryable<Guid, IInvoiceStatus>
    {        
    }
}