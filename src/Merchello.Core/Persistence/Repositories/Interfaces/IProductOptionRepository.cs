namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

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
        /// Get the collection of <see cref="IProductOption"/> for a given product.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        /// <remarks>
        /// The method manages the sort order of the options with respect to the product
        /// This query is never cached and is intended to generate objects that will be cached in 
        /// individual product collections
        /// </remarks>
        IEnumerable<IProductOption> GetByProductKey(Guid productKey);

        /// <summary>
        /// Safely saves product options for a given product.
        /// </summary>
        /// <param name="options">
        /// The options.
        /// </param>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <remarks>
        /// This method ensures the product association of choices, deleted only NON shared options and manages the sort order
        /// </remarks>
        /// <returns>
        /// The saved collection <see cref="IEnumerable{IProductOption}"/>.
        /// </returns>
        IEnumerable<IProductOption> SaveForProduct(IEnumerable<IProductOption> options, Guid productKey);


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