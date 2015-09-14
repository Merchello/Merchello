namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;
    using Merchello.Core.Models.Rdbms;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Defines the product variant repository
    /// </summary>
    internal interface IProductVariantRepository : IRepositoryQueryable<Guid, IProductVariant> 
    {
        /// <summary>
        /// Returns <see cref="IProductVariant"/> given the product and the collection of attribute ids that defines the<see cref="IProductVariant"/>
        /// </summary>
        /// <param name="product">
        /// The product.
        /// </param>
        /// <param name="attributeKeys">
        /// The attribute Keys.
        /// </param>
        /// <returns>
        /// The <see cref="IProductVariant"/>.
        /// </returns>
        IProductVariant GetProductVariantWithAttributes(IProduct product, Guid[] attributeKeys);

        /// <summary>
        /// Compares the <see cref="ProductAttributeCollection"/> with other <see cref="IProductVariant"/>s of the <see cref="IProduct"/> pass
        /// to determine if the a variant already exists with the attributes passed
        /// </summary>
        /// <param name="product">The <see cref="IProduct"/> to reference</param>
        /// <param name="attributes"><see cref="ProductAttributeCollection"/> to compare</param>
        /// <returns>True/false indicating whether or not a <see cref="IProductVariant"/> already exists with the <see cref="ProductAttributeCollection"/> passed</returns>
        bool ProductVariantWithAttributesExists(IProduct product, ProductAttributeCollection attributes);
            
        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> object for a given Product Key
        /// </summary>
        /// <param name="productKey">GUID product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        IEnumerable<IProductVariant> GetByProductKey(Guid productKey);

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> objects associated with a given warehouse 
        /// </summary>
        /// <param name="warehouseKey">The 'unique' id of the warehouse</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        IEnumerable<IProductVariant> GetByWarehouseKey(Guid warehouseKey);

        /// <summary>
        /// True/false indicating whether or not a SKU is already exists in the database
        /// </summary>
        /// <param name="sku">
        /// The SKU to be tested
        /// </param>
        /// <returns>
        /// The <see cref="bool"/> indicating whether on not the SKU exists
        /// </returns>
        bool SkuExists(string sku);

        ///// <summary>
        ///// The delete all detached content for culture.
        ///// </summary>
        ///// <param name="cultureName">
        ///// The culture name.
        ///// </param>
        //void DeleteAllDetachedContentForCulture(string cultureName);



        #region Filter Queries

        // get a list of all products with Option "Color"
        // get a list of all variants with Option "Color" and Choice "Red"       
        // get a list of all products with price range between min and max (this will have to account for product taxation)
        // get a list of all variants with a manufacturer
        // get a list of all variants that are instock (or allow out of stock purchase)

        #endregion
    }
}