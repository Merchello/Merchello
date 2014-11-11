namespace Merchello.Core.Persistence.Repositories
{
    using System;
    using Models;
    using Umbraco.Core.Persistence.Repositories;

    /// <summary>
    /// Marker interface for the ShipmentStatusRepository.
    /// </summary>
    internal interface IShipmentStatusRepository : IRepositoryQueryable<Guid, IShipmentStatus>
    {         
    }
}