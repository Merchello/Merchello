using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the invoice item repository
    /// </summary>
    public interface IInvoiceItemRepository : IRepositoryQueryable<int, IInvoiceItem>
    {
    }
}
