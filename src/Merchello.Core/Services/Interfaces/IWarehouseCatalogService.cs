namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Services;

    /// <summary>
    /// Defines a warehouse catalog service.
    /// </summary>
    internal interface IWarehouseCatalogService : IService
    {
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
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IWarehouseCatalog CreateWarehouseCatalogWithKey(Guid warehouseKey, string name, string description = "", bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalog">
        /// The warehouse catalog.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IWarehouseCatalog warehouseCatalog, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalogs">
        /// The warehouse catalogs.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IWarehouseCatalog> warehouseCatalogs, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalog">
        /// The warehouse catalog.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <remarks>
        /// Cannot delete the default catalog in the default warehouse
        /// </remarks>
        void Delete(IWarehouseCatalog warehouseCatalog, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection of <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <param name="warehouseCatalogs">
        /// The warehouse catalogs.
        /// </param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <remarks>
        /// Cannot delete the default catalog in the default warehouse
        /// </remarks>
        void Delete(IEnumerable<IWarehouseCatalog> warehouseCatalogs, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IWarehouseCatalog"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IWarehouseCatalog GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of all <see cref="IWarehouseCatalog"/>.
        /// </summary>
        /// <returns>
        /// A collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IEnumerable<IWarehouseCatalog> GetAll();

        /// <summary>
        /// Get a collection of <see cref="IWarehouseCatalog"/> by warehouse key.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IEnumerable<IWarehouseCatalog> GetByWarehouseKey(Guid warehouseKey);
    }
}
