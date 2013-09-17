using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IProduct"/>
    /// </summary>
    public interface IProductService : IService
    {

        /// <summary>
        /// Creates a Product
        /// </summary>
        IProduct CreateProduct(string sku, string name, decimal price);

        /// <summary>
        /// Creates a Product
        /// </summary>
        IProduct CreateProduct(string sku, string name, decimal price, decimal costOfGoods, decimal salePrice, decimal weight, decimal length, decimal width, decimal height, string brief, string description, bool taxable, bool shippable, bool download, string downloadUrl, bool template);

        /// <summary>
        /// Saves a single <see cref="IProduct"/> object
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
        /// <param name="id">int Id of the Product to retrieve</param>
        /// <returns><see cref="IProduct"/></returns>
        IProduct GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IProduct"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for Product objects to retrieve</param>
        /// <returns>List of <see cref="IProduct"/></returns>
        IEnumerable<IProduct> GetByIds(IEnumerable<int> ids);

    }
}
