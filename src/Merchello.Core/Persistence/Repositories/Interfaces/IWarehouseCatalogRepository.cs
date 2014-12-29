namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the WarehouseCatalogRepository interface.
    /// </summary>
    internal interface IWarehouseCatalogRepository : IRepositoryQueryable<Guid, IWarehouseCatalog>
    {
        /// <summary>
        /// Gets a collection of <see cref="IWarehouseCatalog"/> by a warehouse key.
        /// </summary>
        /// <param name="warehouseKey">
        /// The warehouse key.
        /// </param>
        /// <returns>
        /// The collection of <see cref="IWarehouseCatalog"/>.
        /// </returns>
        IEnumerable<IWarehouseCatalog> GetWarehouseCatalogsByWarehouseKey(Guid warehouseKey);
    }
}