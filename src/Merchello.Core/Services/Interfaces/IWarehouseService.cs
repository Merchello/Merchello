namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the AddressService, which provides access to operations involving <see cref="IWarehouse"/>
    /// </summary>
    public interface IWarehouseService : IService
    {

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
        /// Gets the default <see cref="IWarehouse"/>
        /// </summary>
        /// <returns>
        /// The <see cref="IWarehouse"/>.
        /// </returns>
        IWarehouse GetDefaultWarehouse();

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

        #region Warehouse Catalog

        /// <summary>
        /// Creates warehouse catalog and persists it to the database.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IWarehouseCatalog CreateWarehouseCatalogWithKey(Guid warehouseKey, string name, string description = "");

        /// <summary>
        /// Saves a single <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalog">
        /// The warehouse catalog.
        /// </param>
        void Save(IWarehouseCatalog warehouseCatalog);

        /// <summary>
        /// Saves a collection of <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalogs">
        /// The warehouse catalogs.
        /// </param>
        void Save(IEnumerable<IWarehouseCatalog> warehouseCatalogs);

        /// <summary>
        /// Deletes a single <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalog">
        /// The warehouse catalog.
        /// </param>
        /// <remarks>
        /// Cannot delete the default catalog in the default warehouse
        /// </remarks>
        void Delete(IWarehouseCatalog warehouseCatalog);

        /// <summary>
        /// Deletes a collection of <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalogs">
        /// The warehouse catalogs.
        /// </param>
        /// <remarks>
        /// Cannot delete the default catalog in the default warehouse
        /// </remarks>
        void Delete(IEnumerable<IWarehouseCatalog> warehouseCatalogs);

        /// <summary>
        /// Gets a <see cref="IWarehouseCatalog"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IWarehouseCatalog GetWarehouseCatalogByKey(Guid key);

        /// <summary>
        /// Gets a collection of all <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IEnumerable<IWarehouseCatalog> GetAllWarehouseCatalogs();

        /// <summary>
        /// Get a collection of <see cref="IWarehouseCatalog"/> by warehouse key.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IEnumerable<IWarehouseCatalog> GetWarhouseCatalogByWarehouseKey(Guid warehouseKey);

        #endregion
    }
}
