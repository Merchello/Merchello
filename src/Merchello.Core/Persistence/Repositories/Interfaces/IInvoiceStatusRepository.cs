using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the Invoice Status Repository
    /// </summary>
    internal interface IInvoiceStatusRepository : IRepositoryQueryable<Guid, IInvoiceStatus>
    { }
}