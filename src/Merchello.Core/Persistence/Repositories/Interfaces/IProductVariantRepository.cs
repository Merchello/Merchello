using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Models.Rdbms;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Defines the product variant repository
    /// </summary>
    internal interface IProductVariantRepository : IRepositoryQueryable<Guid, IProductVariant> 
    {
        /// <summary>
        /// Returns <see cref="IProductVariant"/> given the product and the collection of attribute ids that defines the<see cref="IProductVariant"/>
        /// </summary>
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
        /// <param name="productKey">Guid product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        IEnumerable<IProductVariant> GetByProductKey(Guid productKey);

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> objects associated with a given warehouse 
        /// </summary>
        /// <param name="warehouseKey">The 'unique' id of the warehouse</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        IEnumerable<IProductVariant> GetByWarehouseKey(Guid warehouseKey);

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        bool SkuExists(string sku);
    }
}