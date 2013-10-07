using System;
using System.Collections;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    public interface IProductVariantService : IService
    {
        /// <summary>
        /// Creates a <see cref="IProductVariant"/> of the <see cref="IProduct"/> passed defined by the collection of <see cref="IProductAttribute"/>
        /// </summary>
        /// <param name="product"><see cref="IProduct"/></param>
        /// <param name="attributes"><see cref="IProductVariant"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>Either a new <see cref="IProductVariant"/> or, if one already exists with associated attributes, the existing <see cref="IProductVariant"/></returns>
        IProductVariant CreateVariantWithKey(IProduct product, ProductAttributeCollection attributes, bool raiseEvents = true);

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
        /// Gets an <see cref="IProductVariant"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">Guid key of the Product to retrieve</param>
        /// <returns><see cref="IProductVariant"/></returns>
        IProductVariant GetByKey(Guid key);

        /// <summary>
        /// Gets list of <see cref="IProductVariant"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for ProductVariant objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        IEnumerable<IProductVariant> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets a collection of <see cref="IProductVariant"/> object for a given Product Key
        /// </summary>
        /// <param name="productKey">Guid product key of the <see cref="IProductVariant"/> collection to retrieve</param>
        /// <returns>A collection of <see cref="IProductVariant"/></returns>
        IEnumerable<IProductVariant> GetByProductKey(Guid productKey);

    }
}