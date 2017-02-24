namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Persistence.Querying;

    using Models;
    using Models.Rdbms;

    using Umbraco.Core.Persistence;

    /// <summary>
    /// Marker interface for the address repository
    /// </summary>
    public interface IProductRepository : IPagedRepository<IProduct, ProductDto>, IStaticEntityCollectionRepository<IProduct>, IPortForwardProductRepository, IProductBackOfficeRepository
    {
        /// <summary>
        /// Gets a collection of <see cref="IProduct"/> that has detached content of type.
        /// </summary>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProduct}"/>.
        /// </returns>
        IEnumerable<IProduct> GetByDetachedContentType(Guid detachedContentTypeKey);

        /// <summary>
        /// The get key for slug.
        /// </summary>
        /// <param name="slug">
        /// The slug.
        /// </param>
        /// <returns>
        /// The <see cref="Guid"/>.
        /// </returns>
        Guid GetKeyForSlug(string slug);

        /// <summary>
        /// Gets or sets a value Indicating whether or not a SKU is already exists in the database
        /// </summary>
        /// <param name="sku">The SKU to be tested</param>
        /// <returns>A value indicating whether or not a SKU is already exists in the database</returns>
        bool SkuExists(string sku);


        #region Filter Queries


        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionKey">
        /// The option key.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysWithOption(Guid optionKey, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);



        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysWithOption(string optionName, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceName">
        /// The choice name.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysWithOption(string optionName, string choiceName, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionName">
        /// The option name.
        /// </param>
        /// <param name="choiceNames">
        /// The choice names.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysWithOption(string optionName, IEnumerable<string> choiceNames, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys with option.
        /// </summary>
        /// <param name="optionNames">
        /// The option names.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysWithOption(IEnumerable<string> optionNames, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys in price range.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysInPriceRange(decimal min, decimal max, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys in price range.
        /// </summary>
        /// <param name="min">
        /// The min.
        /// </param>
        /// <param name="max">
        /// The max.
        /// </param>
        /// <param name="taxModifier">
        /// The tax modifier.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysInPriceRange(decimal min, decimal max, decimal taxModifier, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysByManufacturer(string manufacturer, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys by manufacturer.
        /// </summary>
        /// <param name="manufacturer">
        /// The manufacturer.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysByManufacturer(IEnumerable<string> manufacturer, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys by barcode.
        /// </summary>
        /// <param name="barcode">
        /// The barcode.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysByBarcode(string barcode, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys by barcode.
        /// </summary>
        /// <param name="barcodes">
        /// The barcodes.
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysByBarcode(IEnumerable<string> barcodes, long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);

        /// <summary>
        /// The get products keys in stock.
        /// </summary>
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
        /// <param name="includeAllowOutOfStockPurchase">
        /// The include allow out of stock purchase.
        /// </param>
        /// <returns>
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysInStock(long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending, bool includeAllowOutOfStockPurchase = false);

        /// <summary>
        /// The get products keys on sale.
        /// </summary>
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
        /// The <see cref="Page{Guid}"/>.
        /// </returns>
        Page<Guid> GetProductsKeysOnSale(long page, long itemsPerPage, string orderExpression, SortDirection sortDirection = SortDirection.Descending);


        int CountKeysThatExistInAllCollections(Guid[] collectionKeys);

        IEnumerable<Tuple<IEnumerable<Guid>, int>> CountKeysThatExistInAllCollections(IEnumerable<Guid[]> collectionKeysGroups);

        #endregion
    }
}
