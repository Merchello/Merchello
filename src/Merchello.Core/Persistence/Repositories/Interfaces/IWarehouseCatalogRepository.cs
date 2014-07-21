namespace Merchello.Core.Persistence.Repositories
{
    using System;

    using Merchello.Core.Models;

    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the WarehouseCatalogRepository interface.
    /// </summary>
    internal interface IWarehouseCatalogRepository : IRepositoryQueryable<Guid, IWarehouseCatalog>
    {
    }
}