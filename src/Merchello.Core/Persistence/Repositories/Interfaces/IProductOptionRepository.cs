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
        /// Saves the product options for a given product.
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        void SaveForProduct(IProduct product);

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
        ProductOptionCollection SaveForProduct(IEnumerable<IProductOption> options, Guid productKey);

        /// <summary>
        /// Gets the <see cref="ProductOptionCollection"/> for a given product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductOptionCollection"/>.
        /// </returns>
        /// <remarks>
        /// The method manages the sort order of the options with respect to the product
        /// This query is never cached and is intended to generate objects that will be cached in 
        /// individual product collections
        /// </remarks>
        ProductOptionCollection GetProductOptionCollection(Guid productKey);


        /// <summary>
        /// Gets a <see cref="ProductAttributeCollection"/> for a product variant.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="ProductAttributeCollection"/>.
        /// </returns>
        ProductAttributeCollection GetProductAttributeCollectionForVariant(Guid productVariantKey);

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


        void DeleteProductVariantAttributes(IProductVariant variant);

        int GetProductOptionShareCount(Guid optionKey);


        int GetProductAttributeUseCount(Guid attributeKey);
    }
}