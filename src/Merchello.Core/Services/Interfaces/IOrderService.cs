using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Services;

namespace Merchello.Core.Services
{
    public interface IOrderService : IService
    {
        /// <summary>
        /// Creates a <see cref="IOrder"/> without saving it to the database
        /// </summary>
        /// <param name="orderStatusKey">The <see cref="IOrderStatus"/> key</param>
        /// <param name="invoiceKey">The invoice key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IOrder"/></returns>
        IOrder CreateOrder(Guid orderStatusKey, Guid invoiceKey, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IOrder"/> and saves it to the database
        /// </summary>
        /// <param name="orderStatusKey">The <see cref="IOrderStatus"/> key</param>
        /// <param name="invoiceKey">The invoice key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns><see cref="IOrder"/></returns>
        IOrder CreateOrderWithKey(Guid orderStatusKey, Guid invoiceKey, bool raiseEvents = true);

        /// <summary>
        /// Saves a single <see cref="IOrder"/>
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/> to save</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IOrder order, bool raiseEvents = true);

        /// <summary>
        /// Saves a collection of <see cref="IOrder"/>
        /// </summary>
        /// <param name="orders">The collection of <see cref="IOrder"/></param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Save(IEnumerable<IOrder> orders, bool raiseEvents = true);

        /// <summary>
        /// Deletes a single <see cref="IOrder"/>
        /// </summary>
        /// <param name="order">The <see cref="IOrder"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IOrder order, bool raiseEvents = true);

        /// <summary>
        /// Deletes a collection <see cref="IOrder"/>
        /// </summary>
        /// <param name="orders">The collection of <see cref="IOrder"/> to be deleted</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        void Delete(IEnumerable<IOrder> orders, bool raiseEvents = true);

        /// <summary>
        /// Gets a <see cref="IOrder"/> given it's unique 'key' (Guid)
        /// </summary>
        /// <param name="key">The <see cref="IOrder"/>'s unique 'key' (Guid)</param>
        /// <returns><see cref="IOrder"/></returns>
        IOrder GetByKey(Guid key);

        /// <summary>
        /// Gets a <see cref="IOrder"/> given it's unique 'OrderNumber'
        /// </summary>
        /// <param name="orderNumber">The order number of the <see cref="IOrder"/> to be retrieved</param>
        /// <returns><see cref="IOrder"/></returns>
        IOrder GetByOrderNumber(int orderNumber);

        /// <summary>
        /// Gets list of <see cref="IOrder"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of guid 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IOrder"/></returns>
        IEnumerable<IOrder> GetByKeys(IEnumerable<Guid> keys);
         
    }
}