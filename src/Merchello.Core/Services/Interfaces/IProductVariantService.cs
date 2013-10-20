using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines a ProductVariantService
    /// </summary>
    public interface IProductVariantService : IService
    {

        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="attributes"><see cref="IProductVariant"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        IProductVariant CreateProductVariantWithId(IProduct product, ProductAttributeCollection attributes, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="name">The name of the product variant</param>
        /// <param name="sku">The unique sku of the product variant</param>
        /// <param name="price">The price of the product variant</param>
        /// <param name="attributes"><see cref="IProductVariant"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        IProductVariant CreateProductVariantWithId(IProduct product, string name, string sku, decimal price, ProductAttributeCollection attributes, bool raiseEvents = true);

        /// <summary>
        /// Saves a single instance of a <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariant"></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IProductVariant productVariant, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariantList">The collection of <see cref="IProductVariant"/> to be saved</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IProductVariant> productVariantList, bool raiseEvents = true);

        /// <summary>
        /// Ensures that all <see cref="IProductVariant"/> except the "master" variant for the <see cref="IProduct"/> have attributes
        /// </summary>
        /// <param name="product"><see cref="IProduct"/> to varify</param>
        void EnsureProductVariantsHaveAttributes(IProduct product);

        /// <summary>
        /// Ensures that every <see cref="IProductVariant"/> for every <see cref="IProduct"/> (except it's master variant) in the collection has related <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="productList">The collection of <see cref="IProduct"/> to ensure</param>
        void EnsureProductVariantsHaveAttributes(IEnumerable<IProduct> productList);

        /// <summary>
        /// Deletes a single <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariant">The <see cref="IProductVariant"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IProductVariant productVariant, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IProductVariant"/>
        /// </summary>
        /// <param name="productVariantList">The collction of <see cref="IProductVariant"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IProductVariant> productVariantList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IProductVariant"/> object by its unique id
        /// </summary>
        /// <param name="id">id of the Product to retrieve</param>
        /// <returns><see cref="IProductVariant"/></returns>
        IProductVariant GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IProductVariant"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of ids for ProductVariant objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        IEnumerable<IProductVariant> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> objects for a given Product Key
        /// </summary>
        /// <param name="productKey">Guid product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        IEnumerable<IProductVariant> GetByProductKey(Guid productKey);

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> objects associated with a given warehouse 
        /// </summary>
        /// <param name="warehouseId">The 'unique' id of the warehouse</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        IEnumerable<IProductVariant> GetByWarehouseId(int warehouseId);

        /// <summary>
        /// Returns <see cref="IProductVariant"/> given the product and the collection of attribute ids that defines the<see cref="IProductVariant"/>
        /// </summary>
        IProductVariant GetProductVariantWithAttributes(IProduct product, int[] attributeIds);

        /// <summary>
        /// Compares the <see cref="ProductAttributeCollection"/> with other <see cref="IProductVariant"/>s of the <see cref="IProduct"/> pass
        /// to determine if the a variant already exists with the attributes passed
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to reference</param>
        /// <param name="attributes"><see cref="ProductAttributeCollection"/> to compare</param>
        /// <returns>True/false indicating whether or not a <see cref="IProductVariant"/> already exists with the <see cref="ProductAttributeCollection"/> passed</returns>
        bool ProductVariantWithAttributesExists(IProduct product, ProductAttributeCollection attributes);

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        bool SkuExists(string sku);

    }
}