namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Persistence.Querying;

    using Models;
    using Models.Rdbms;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Marker interface for the invoice repository
    /// </summary>
    internal interface IInvoiceRepository : IPagedRepository<IInvoice, InvoiceDto>, IAssertsMaxDocumentNumber
    {
        /// <summary>
        /// Returns a value indicating whether or not the invoice exists in a collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        bool ExistsInCollection(Guid invoiceKey, Guid collectionKey);

        /// <summary>
        /// Adds a invoice to a static invoice collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        void AddInvoiceToCollection(Guid invoiceKey, Guid collectionKey);

        /// <summary>
        /// The remove invoice from collection.
        /// </summary>
        /// <param name="invoiceKey">
        /// The invoice key.
        /// </param>
        /// <param name="collectionKey">
        /// The collection key.
        /// </param>
        void RemoveProductFromCollection(Guid invoiceKey, Guid collectionKey);

        /// <summary>
        /// The get invoice keys from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="Page{T}"/>.
        /// </returns>
        Page<Guid> GetInvoiceKeysFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// Gets invoices from collection.
        /// </summary>
        /// <param name="collectionKey">
        /// The collection key.
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
        /// The <see cref="Page{IInvoice}"/>.
        /// </returns>
        Page<IInvoice> GetInvoicesFromCollection(
            Guid collectionKey,
            long page,
            long itemsPerPage,
            string orderExpression,
            SortDirection sortDirection = SortDirection.Descending);
    }
}
