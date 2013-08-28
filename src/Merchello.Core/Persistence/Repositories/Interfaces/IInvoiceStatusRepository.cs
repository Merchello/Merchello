using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the invoice status repository
    /// </summary>
    public interface IInvoiceStatusRepository : IRepository<int, IInvoiceStatus>
    {
    }
}
