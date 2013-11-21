using System;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the region repository
    /// </summary>
    internal interface IRegionRepository : IRepositoryQueryable<Guid, IRegion>
    {
         
    }
}