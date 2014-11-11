namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the ProductService, which provides access to operations involving <see cref="IProduct"/>
    /// </summary>
    public interface IProductService : IPageCachedService<IProduct>
    {
        /// <summary>
        /// Creates a Product without saving it to the database
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="sku">
        /// The sku.
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
        /// The sku.
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
        /// Gets list of <see cref="IProduct"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for Product objects to retrieve</param>
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
        /// Returns the count of all products
        /// </summary>
        /// <returns>
        /// The count as an <see cref="int"/>.
        /// </returns>
        int ProductsCount();

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns>A value indication whether or not the SKU exists</returns>
        bool SkuExists(string sku);
    }
}
