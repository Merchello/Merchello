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
    /// Defines the AddressService, which provides access to operations involving <see cref="IShipMethod"/>
    /// </summary>
    public interface IShipMethodService : IService
    {

        /// <summary>
        /// Creates a ShipMethod
        /// </summary>
        IShipMethod CreateShipMethod(string name, Guid providerKey, ShipMethodType shipMethodType);

        /// <summary>
        /// Creates a ShipMethod
        /// </summary>
        IShipMethod CreateShipMethod(string name, Guid providerKey, Guid shipMethodTypeFieldKey);

        /// <summary>
        /// Saves a single <see cref="IShipMethod"/> object
        /// </summary>
        /// <param name="shipMethod">The <see cref="IShipMethod"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IShipMethod shipMethod, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IShipMethod"/> objects
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IShipMethod> shipMethodList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IShipMethod"/> object
        /// </summary>
        /// <param name="shipMethod"><see cref="IShipMethod"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IShipMethod shipMethod, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IShipMethod"/> objects
        /// </summary>
        /// <param name="shipMethodList">Collection of <see cref="IShipMethod"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IShipMethod> shipMethodList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IShipMethod"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="id">int Id of the ShipMethod to retrieve</param>
        /// <returns><see cref="IShipMethod"/></returns>
        IShipMethod GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IShipMethod"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for ShipMethod objects to retrieve</param>
        /// <returns>List of <see cref="IShipMethod"/></returns>
        IEnumerable<IShipMethod> GetByIds(IEnumerable<int> ids);

    }
}
