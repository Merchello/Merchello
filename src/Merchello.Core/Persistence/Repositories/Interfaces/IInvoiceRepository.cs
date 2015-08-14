namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Web.UI;

    using Merchello.Core.Persistence.Querying;

    using Models;
    using Models.Rdbms;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Marker interface for the invoice repository
    /// </summary>
    internal interface IInvoiceRepository : IPagedRepository<IInvoice, InvoiceDto>, IStaticEntityCollectionRepository<IInvoice>, IAssertsMaxDocumentNumber
    {
        #region Filter Queries

        /// <summary>
        /// Gets invoices matching the search term and the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        Page<IInvoice> GetInvoicesMatching(string searchTerm, Guid invoiceStatusKey, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets invoice keys matching the search term and the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        Page<Guid> GetInvoiceKeysMatching(string searchTerm, Guid invoiceStatusKey, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets invoices matching the search term but not the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        Page<IInvoice> GetInvoicesMatchingTermNotInvoiceStatus(string searchTerm, Guid invoiceStatusKey, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets invoice keys matching the search term but not the invoice status key.
        /// </summary>
        /// <param name="searchTerm">
        /// The search term.
        /// </param>
        /// <param name="invoiceStatusKey">
        /// The invoice status key.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="orderExpression">
        /// The order expression.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="Page"/>.
        /// </returns>
        Page<Guid> GetInvoiceKeysMatchingTermNotInvoiceStatus(string searchTerm, Guid invoiceStatusKey, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        #endregion
    }
}
