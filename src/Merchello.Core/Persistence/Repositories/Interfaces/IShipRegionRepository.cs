using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core.Persistence.Repositories;
using IShipCountry = Merchello.Core.Models.Interfaces.IShipCountry;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the region repository
    /// </summary>
    internal interface IShipRegionRepository : IRepositoryQueryable<Guid, IShipCountry>
    {
         
    }
}