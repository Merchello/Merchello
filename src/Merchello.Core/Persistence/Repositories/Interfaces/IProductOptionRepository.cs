namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models;
    using Merchello.Core.Persistence.Querying;

    using Umbraco.Core.Persistence;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Defines a product option repository.
    /// </summary>
    internal interface IProductOptionRepository : IRepositoryQueryable<Guid, IProductOption>
    {
        /// <summary>
        /// Gets a page of <see cref="IProductOption"/>.
        /// </summary>
        /// <param name="term">
        /// A search term to filter by
        /// </param>
        /// <param name="page">
        /// The page requested.
        /// </param>
        /// <param name="itemsPerPage">
        /// The number of items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by field.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <param name="sharedOnly">
        /// Indicates whether or not to only include shared option.
        /// </param>
        /// <returns>
        /// The <see cref="Page{IProductOption}"/>.
        /// </returns>
        Page<IProductOption> GetPage(string term, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending, bool sharedOnly = true);
    }
}