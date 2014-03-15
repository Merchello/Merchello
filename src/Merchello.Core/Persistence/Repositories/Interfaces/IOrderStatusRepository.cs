using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Marker interface for the Order Status Repository
    /// </summary>
    internal interface IOrderStatusRepository : IRepositoryQueryable<Guid, IOrderStatus>
    { }
}