using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the ship method repository
    /// </summary>
    internal interface IShipMethodRepository : IRepositoryQueryable<Guid, IShipMethod>
    {
        
    }
}