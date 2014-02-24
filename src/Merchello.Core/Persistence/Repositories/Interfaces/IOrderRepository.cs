using System;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Maker interface for the OrderRepository
    /// </summary>
    internal interface IOrderRepository : IRepositoryQueryable<Guid, IOrder>
    {
         
    }
}