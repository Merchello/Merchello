using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IAddress"/>
    /// </summary>
    public interface IAddressService : IService
    {
        /// <summary>
        /// Creates a Address
        /// </summary>
        /// <param name="id"></param>
        /// <param name="customerPk"></param>
        /// <param name="label"></param>
        /// <returns><see cref="IAddress"/></returns>
        IAddress CreateAddress(int id, Guid customerPk, string label);

        /// <summary>
        /// Saves a single <see cref="IAddress"/> object
        /// </summary>
        /// <param name="address">The <see cref="IAddress"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IAddress address, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IAddress"/> objects
        /// </summary>
        /// <param name="addresss"></param>
        /// <param name="raiseEvents"></param>
        void Save(IEnumerable<IAddress> addresss, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IAddress"/> object
        /// </summary>
        /// <param name="address"><see cref="IAddress"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IAddress address, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IAddress"/> objects
        /// </summary>
        /// <param name="addresses">Collection of <see cref="IAddress"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IAddress> addresses, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IAddress"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the Address to retrieve</param>
        /// <returns><see cref="IAddress"/></returns>
        IAddress GetByKey(int id);

        /// <summary>
        /// Gets list of <see cref="IAddress"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of Guid pk for Addresss to retrieve</param>
        /// <returns>List of <see cref="IAddress"/></returns>
        IEnumerable<IAddress> GetByKeys(IEnumerable<Guid> keys);

    }
}
