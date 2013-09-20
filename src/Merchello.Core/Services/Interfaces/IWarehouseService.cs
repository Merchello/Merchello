﻿using System;
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
        /// <param name="id">int Id of the Warehouse to retrieve</param>
        /// <returns><see cref="IWarehouse"/></returns>
        IWarehouse GetById(int id);

        /// <summary>
        /// Gets list of <see cref="IWarehouse"/> objects given a list of Unique keys
        /// </summary>
        /// <param name="ids">List of int Id for Warehouse objects to retrieve</param>
        /// <returns>List of <see cref="IWarehouse"/></returns>
        IEnumerable<IWarehouse> GetByIds(IEnumerable<int> ids);

        /// <summary>
        /// Gets a list of warehouses associated with a ship method
        /// </summary>
        /// <param name="shipMethodId">The id of the <see cref="IShipMethod"/></param>
        /// <returns>A collection of <see cref="IWarehouse"/></returns>
        IEnumerable<IWarehouse> GetWarehousesForShipMethod(int shipMethodId);

    }
}
