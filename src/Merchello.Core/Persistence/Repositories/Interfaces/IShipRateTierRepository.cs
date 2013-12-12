using System;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the ship rate tier repository
    /// </summary>
    public interface IShipRateTierRepository : IRepositoryQueryable<Guid, IShipRateTier>
    {
         
    }
}