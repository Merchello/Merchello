namespace Merchello.Web.Search
{
    using System;
    using System.Collections.Generic;

    using Core.Persistence.Querying;
    using Models.ContentEditing;
    using Models.Querying;

    /// <summary>
    /// Defines a CachedProductQuery.
    /// </summary>
    public interface ICachedProductQuery
    {
        /// <summary>
        /// Gets a <see cref="ProductDisplay"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        ProductDisplay GetByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="ProductDisplay"/> by it's SKU
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="ProductDisplay"/>.
        /// </returns>
        ProductDisplay GetBySku(string sku);

        /// <summary>
        /// Gets a <see cref="ProductVariantDisplay"/> by it's key
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        ProductVariantDisplay GetProductVariantByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="ProductVariantDisplay"/> by it's SKU
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="ProductVariantDisplay"/>.
        /// </returns>
        ProductVariantDisplay GetProductVariantBySku(string sku);

        /// <summary>
        /// Searches all products
        /// </summary>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay Search(long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending);


        /// <summary>
        /// Searches all products for a given term
        /// </summary>
        /// <param name="term">
        /// The term.
        /// </param>
        /// <param name="page">
        /// The page.
        /// </param>
        /// <param name="itemsPerPage">
        /// The items per page.
        /// </param>
        /// <param name="sortBy">
        /// The sort by.
        /// </param>
        /// <param name="sortDirection">
        /// The sort direction.
        /// </param>
        /// <returns>
        /// The <see cref="QueryResultDisplay"/>.
        /// </returns>
        QueryResultDisplay Search(string term, long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Ascending);

        QueryResultDisplay GetProductsWithOption(string optionName, IEnumerable<string> choiceNames, long page, long itemsPerPage, string sortBy = "name", SortDirection sortDirection = SortDirection.Descending);
    }
}