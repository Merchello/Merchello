namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the invoice repository
    /// </summary>
    internal interface IInvoiceRepository : IPagedEntityKeyFetchRepository<Guid, IInvoice>
    {
    }
}
