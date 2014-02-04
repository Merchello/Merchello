using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the ship rate tier repository
    /// </summary>
    internal interface IShipRateTierRepository : IRepositoryQueryable<Guid, IShipRateTier>
    {
         
    }
}