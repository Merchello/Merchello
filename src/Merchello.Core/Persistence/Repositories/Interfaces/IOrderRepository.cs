using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Persistence.Repositories;

namespace Merchello.Core.Persistence.Repositories
{
    /// <summary>
    /// Maker interface for the OrderRepository
    /// </summary>
    internal interface IOrderRepository : IRepositoryQueryable<Guid, IOrder>
    {
        ///// <summary>
        ///// Gets a collection of <see cref="IOrderLineItem"/> by a <see cref="IShipment"/> key
        ///// </summary>
        ///// <param name="shipmentKey">The <see cref="IShipment"/> key</param>
        ///// <returns>A collection of <see cref="IOrderLineItem"/></returns>
        //IEnumerable<IOrderLineItem> GetOrderLineItemsByShipmentKey(Guid shipmentKey);
    }
}