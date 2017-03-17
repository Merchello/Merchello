namespace Merchello.Core.Services.Interfaces
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Marker interface product service queries that need to be ported forward to V3 version.
    /// </summary>
    public interface IProductServicePortForward
    {
        /// <summary>
        /// Gets a list of currently listed Manufacturers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{String}"/> (manufacturer names).
        /// </returns>
        IEnumerable<string> GetAllManufacturers();

        /// <summary>
        /// Gets recently updated products.
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <returns>
        /// The <see cref="PagedCollection"/>.
        /// </returns>
        /// <remarks>
        /// Return value breaks the pattern in this repository for easier port forward to Merchello Version 3.0
        /// where all repositories return <see cref="PagedCollection"/> rather than the ORM <see cref="Page{T}"/>.
        /// </remarks>
        PagedCollection<IProduct> GetRecentlyUpdatedProducts(long page, long itemsPerPage = 10);

        ///// <summary>
        ///// Special case advanced search.
        ///// </summary>
        ///// <param name="collectionKey">
        ///// The collection key.
        ///// </param>
        ///// <param name="includeFields">
        ///// The include fields.
        ///// </param>
        ///// <param name="term">
        ///// The term.
        ///// </param>
        ///// <param name="manufacturer">
        ///// The manufacturer.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="orderExpression">
        ///// The order expression.
        ///// </param>
        ///// <param name="direction">
        ///// The direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="PagedCollection"/>.
        ///// </returns>
        //PagedCollection<IProduct> GetByAdvancedSearch(
        //    Guid collectionKey,
        //    string[] includeFields,
        //    string term,
        //    string manufacturer,
        //    long page,
        //    long itemsPerPage,
        //    string orderExpression,
        //    SortDirection direction = SortDirection.Ascending);
    }
}