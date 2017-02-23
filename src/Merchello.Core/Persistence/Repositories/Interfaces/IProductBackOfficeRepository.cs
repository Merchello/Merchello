namespace Merchello.Core.Persistence.Repositories
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Represents a back office repository.
    /// </summary>
    /// <remarks>
    /// Port forward to V3
    /// </remarks>
    public interface IProductBackOfficeRepository
    {
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
    }
}