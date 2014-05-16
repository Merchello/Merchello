using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the customer registry repository
    /// </summary>
    internal interface IItemCacheRepository : IRepositoryQueryable<Guid, IItemCache>
    {

    }
}
