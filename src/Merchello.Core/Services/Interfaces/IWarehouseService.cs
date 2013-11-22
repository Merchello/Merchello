using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IWarehouse"/>
    /// </summary>
    public interface IWarehouseService : IService
    {

        /// <summary>
        /// Creates a Warehouse
        /// </summary>
        IWarehouse CreateWarehouse(string name);

        /// <summary>
        /// Saves a single <see cref="IWarehouse"/> object
        /// </summary>
        /// <param name="warehouse">The <see cref="IWarehouse"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IWarehouse warehouse, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IWarehouse"/> objects
        /// </summary>
        /// <param name="warehouseList">Collection of <see cref="IWarehouse"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IWarehouse> warehouseList, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IWarehouse"/> object
        /// </summary>
        /// <param name="warehouse"><see cref="IWarehouse"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IWarehouse warehouse, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IWarehouse"/> objects
        /// </summary>
        /// <param name="warehouseList">Collection of <see cref="IWarehouse"/> to delete</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IWarehouse> warehouseList, bool raiseEvents = true);

        /// <summary>
        /// Gets an <see cref="IWarehouse"/> object by its 'UniqueId'
        /// </summary>
        /// <param name="key">key of the Warehouse to retrieve</param>
        /// <returns><see cref="IWarehouse"/></returns>
        IWarehouse GetByKey(Guid key);

        /// <summary>
        /// Gets list of <see cref="IWarehouse"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="keys">List of int Id for Warehouse objects to retrieve</param>
        /// <returns>List of <see cref="IWarehouse"/></returns>
        IEnumerable<IWarehouse> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets a list of warehouses associated with a ship method
        /// </summary>
        /// <param name="shipMethodKey">The key of the <see cref="IShipMethod"/></param>
        /// <returns>A collection of <see cref="IWarehouse"/></returns>
        IEnumerable<IWarehouse> GetWarehousesForShipMethod(Guid shipMethodKey);


        /// <summary>
        /// Returns the <see cref="Merchello.Core.Models.ICountry"/> for the country code passed.
        /// </summary>
        /// <param name="countryCode">The two letter ISO Region code (country code)</param>
        /// <returns><see cref="RegionInfo"/> for the country corresponding the the country code passed</returns>
        ICountryBase GetCountryByCode(string countryCode);

        /// <summary>
        /// Returns a Region collection for all countries
        /// </summary>
        /// <returns>A collection of <see cref="Merchello.Core.Models.ICountry"/></returns>
        IEnumerable<ICountryBase> GetAllCountries();

        /// <summary>
        /// Returns a RegionInfo collection for all countries excluding codes passed
        /// </summary>
        /// <param name="excludeCountryCodes">A collection of country codes to exclude from the result set</param>
        /// <returns>A collection of <see cref="RegionInfo"/></returns>
        IEnumerable<ICountryBase> GetAllCountries(string[] excludeCountryCodes);

    }
}
