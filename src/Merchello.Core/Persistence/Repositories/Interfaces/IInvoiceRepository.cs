namespace Merchello.Core.Persistence.Repositories
{
    using Models;
    using Models.Rdbms;

    /// <summary>
    /// Marker interface for the invoice repository
    /// </summary>
    internal interface IInvoiceRepository : IPagedRepository<IInvoice, InvoiceDto>, IAssertsMaxDocumentNumber
    {
    }
}
