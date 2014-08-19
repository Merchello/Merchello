using System;
using Merchello.Core.Persistence.Querying;
using Umbraco.Core.Persistence;

namespace Merchello.Core.Persistence.Repositories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// Marker interface for the invoice repository
    /// </summary>
    internal interface IInvoiceRepository : IPagedRepository<IInvoice, InvoiceDto>
    {
    }
}
