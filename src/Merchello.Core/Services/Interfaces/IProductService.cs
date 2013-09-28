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
    /// Defines the AddressService, which provides access to operations involving <see cref="IProductActual"/>
    /// </summary>
    public interface IProductService : IService
    {

        /// <summary>
        /// Creates a Product
        /// </summary>
        IProductActual CreateProduct(string sku, string name, decimal price);
      

        /// <summary>
        /// Saves a single <see cref="IProductActual"/> object
        /// </summary>
        /// <param name="productActual">The <see cref="IProductActual"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IProductActual productActual, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IProductActual"/> objects
        /// </summary>
        /// <param name="productList">Collection of <see cref="IProductActual"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IProductActual> productList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IProductActual"/> object
        /// </summary>
        /// <param name="productActual"><see cref="IProductActual"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IProductActual productActual, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IProductActual"/> objects
        /// </summary>
        /// <param name="productList">Collection of <see cref="IProductActual"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IProductActual> productList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IProductActual"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">Guid key of the Product to retrieve</param>
        /// <returns><see cref="IProductActual"/></returns>
        IProductActual GetByKey(Guid key);

        /// <summary>
        /// Gets list of <see cref="IProductActual"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid keys for Product objects to retrieve</param>
        /// <returns>List of <see cref="IProductActual"/></returns>
        IEnumerable<IProductActual> GetByKeys(IEnumerable<Guid> keys);

    }
}
