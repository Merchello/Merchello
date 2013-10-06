using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.Prototyping.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IProductVariant"/>
    /// </summary>
    public interface IProductService : IService
    {

        /// <summary>
        /// Creates a Product without saving it to the database
        /// </summary>
        IProduct CreateProduct(string name, string sku, decimal price);

        /// <summary>
        /// Creates a Product and saves it to the database
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sku"></param>
        /// <param name="price"></param>
        /// <returns></returns>
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
        /// Gets an <see cref="IProduct"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">Guid key of the Product to retrieve</param>
        /// <returns><see cref="IProductVariant"/></returns>
        IProduct GetByKey(Guid key);

        /// <summary>
        /// Gets list of <see cref="IProduct"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for Product objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        IEnumerable<IProduct> GetByKeys(IEnumerable<Guid> keys);

        ///// <summary>
        ///// Creates and saves a <see cref="IProductOption"/> object associated with a product
        ///// </summary>
        ///// <param name="product">The <see cref="IProduct"/> to which to add the <see cref="IProductOption"/></param>
        ///// <param name="name">The name of the option</param>
        ///// <param name="required">True/false indicating whether or not this option is required in which case will become part of a ProductVariant definition</param>
        ///// <returns></returns>
        //void SaveProductOption(IProduct product, string name, bool required = true);

        ///// <summary>
        ///// Saves a <see cref="IProductOption"/> associated with a <see cref="IProduct"/>
        ///// </summary>
        ///// <param name="product"></param>
        ///// <param name="productOption"></param>
        //void SaveProductOption(IProduct product, IProductOption productOption);

        /// <summary>
        /// True/false indicating whether or not a sku is already exists in the database
        /// </summary>
        /// <param name="sku">The sku to be tested</param>
        /// <returns></returns>
        bool SkuExists(string sku);
    }
}
