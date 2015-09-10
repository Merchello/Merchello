namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Models;

    /// <summary>
    /// Defines the ProductService, which provides access to operations involving <see cref="IProduct"/>
    /// </summary>
    public interface IProductService : IStaticCollectionService<IProduct>, IPageCachedService<IProduct>
    {
        /// <summary>
        /// Creates a Product without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        IProduct CreateProduct(string name, string sku, decimal price);

        /// <summary>
        /// Creates a Product and saves it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <param name="price">
        /// The price.
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        IProduct CreateProductWithKey(string name, string sku, decimal price);

        /// <summary>
        /// Saves a single <see cref="IProductVariant"/> object
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IProduct product, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IProduct"/> objects
        /// </summary>
        /// <param name="productList">Collection of <see cref="IProduct"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IProduct> productList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IProduct"/> object
        /// </summary>
        /// <param name="product"><see cref="IProduct"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IProduct product, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IProduct"/> objects
        /// </summary>
        /// <param name="productList">Collection of <see cref="IProduct"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IProduct> productList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IProduct"/> by it's unique SKU.
        /// </summary>
        /// <param name="sku">
        /// The product SKU.
        /// </param>
        /// <returns>
        /// The <see cref="IProduct"/>.
        /// </returns>
        IProduct GetBySku(string sku);

        /// <summary>
        /// Gets list of <see cref="IProduct"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of GUID keys for Product objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        IEnumerable<IProduct> GetByKeys(IEnumerable<Guid> keys);
            
        /// <summary>
        /// Gets a collection of all <see cref="IProduct"/>.
        /// </summary>
        /// <returns>
        /// The collection of all <see cref="IProduct"/>.
        /// </returns>
        IEnumerable<IProduct> GetAll();

        /// <summary>
        /// The get product variants by product key.
        /// </summary>
        /// <param name="productKey">
        /// The product key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{IProductVariant}"/>.
        /// </returns>
        IEnumerable<IProductVariant> GetProductVariantsByProductKey(Guid productKey);

        /// <summary>
        /// The get product variant by key.
        /// </summary>
        /// <param name="productVariantKey">
        /// The product variant key.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        IProductVariant GetProductVariantByKey(Guid productVariantKey);

        /// <summary>
        /// Get's a <see cref="IProductVariant"/> by it's unique SKU.
        /// </summary>
        /// <param name="sku">
        /// The SKU.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        IProductVariant GetProductVariantBySku(string sku);

        /// <summary>
        /// Returns the count of all products
        /// </summary>
        /// <returns>
        /// The count as an <see cref="int"/>.
        /// </returns>
        int ProductsCount();

        /// <summary>
        /// True/false indicating whether or not a SKU is already exists in the database
        /// </summary>
        /// <param name="sku">The SKU to be tested</param>
        /// <returns>A value indication whether or not the SKU exists</returns>
        bool SkuExists(string sku);

        #region Detached Content

        /// <summary>
        /// Removes detached content from the product.
        /// </summary>
        /// <param name="product">
        /// The product variants.
        /// </param>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key
        /// </param>
        /// <param name="raiseEvents">
        /// The raise events.
        /// </param>
        void RemoveDetachedContent(IProduct product, Guid detachedContentTypeKey, bool raiseEvents = true);

        /// <summary>
        /// Removes detached content from the collection of products
        /// </summary>
        /// <param name="products">
        /// The product variants.
        /// </param>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        void RemoveDetachedContent(IEnumerable<IProduct> products, Guid detachedContentTypeKey, bool raiseEvents = true);

        /// <summary>
        /// Gets a collect of products by detached content type.
        /// </summary>
        /// <param name="detachedContentTypeKey">
        /// The detached content type key.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{Product}"/>.
        /// </returns>
        IEnumerable<IProduct> GetByDetachedContentType(Guid detachedContentTypeKey); 

        #endregion

        //#region Filter Queries


        ///// <summary>
        ///// The get products keys with option.
        ///// </summary>
        ///// <param name="optionName">
        ///// The option name.
        ///// </param>
        ///// <param name="choiceNames">
        ///// The choice names.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The sort by.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysWithOption(string optionName, IEnumerable<string> choiceNames, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys with option.
        ///// </summary>
        ///// <param name="optionName">
        ///// The option name.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysWithOption(string optionName, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys with option.
        ///// </summary>
        ///// <param name="optionName">
        ///// The option name.
        ///// </param>
        ///// <param name="choiceName">
        ///// The choice name.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysWithOption(string optionName, string choiceName, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys with option.
        ///// </summary>
        ///// <param name="optionNames">
        ///// The option names.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysWithOption(IEnumerable<string> optionNames, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys in price range.
        ///// </summary>
        ///// <param name="min">
        ///// The min.
        ///// </param>
        ///// <param name="max">
        ///// The max.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysInPriceRange(decimal min, decimal max, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys in price range.
        ///// </summary>
        ///// <param name="min">
        ///// The min.
        ///// </param>
        ///// <param name="max">
        ///// The max.
        ///// </param>
        ///// <param name="taxModifier">
        ///// The tax modifier.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysInPriceRange(decimal min, decimal max, decimal taxModifier, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys by manufacturer.
        ///// </summary>
        ///// <param name="manufacturer">
        ///// The manufacturer.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysByManufacturer(string manufacturer, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys by manufacturer.
        ///// </summary>
        ///// <param name="manufacturer">
        ///// The manufacturer.
        ///// </param>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysByManufacturer(IEnumerable<string> manufacturer, long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        ///// <summary>
        ///// The get products keys in stock.
        ///// </summary>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <param name="includeAllowOutOfStockPurchase">
        ///// The include allow out of stock purchase.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysInStock(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending, bool includeAllowOutOfStockPurchase = false);

        ///// <summary>
        ///// The get products keys on sale.
        ///// </summary>
        ///// <param name="page">
        ///// The page.
        ///// </param>
        ///// <param name="itemsPerPage">
        ///// The items per page.
        ///// </param>
        ///// <param name="sortBy">
        ///// The order expression.
        ///// </param>
        ///// <param name="sortDirection">
        ///// The sort direction.
        ///// </param>
        ///// <returns>
        ///// The <see cref="Page{Guid}"/>.
        ///// </returns>
        //Page<Guid> GetProductsKeysOnSale(long page, long itemsPerPage, string sortBy = "", SortDirection sortDirection = SortDirection.Descending);

        //#endregion
    }
}
