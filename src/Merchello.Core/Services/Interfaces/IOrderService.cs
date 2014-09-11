namespace Merchello.Core.Services
{
    using System;
    using System.Collections.Generic;
    using Models;
    using Umbraco.Core.Services;

    /// <summary>
    /// Defines the OrderService.
    /// </summary>
    public interface IOrderService : IPageCachedService<IOrder>
    {
        /// <summary>
        /// Creates a <see cref="IOrder"/> without saving it to the database
        /// </summary>
        /// <param name="orderStatusKey">The <see cref="IOrderStatus"/> key</param>
        /// <param name="invoiceKey">The invoice key</param>
        /// <param name="raiseEvents">Optional boolean indicating whether or not to raise events</param>
        /// <returns>The <see cref="IOrder"/></returns>
        IOrder CreateOrder(Guid orderStatusKey, Guid invoiceKey, bool raiseEvents = true);

        /// <summary>
        /// Creates a <see cref="IOrder"/> without saving it to the database
        /// </summary>
        /// <param name="orderStatusKey">
        /// The <see cref="IOrderStatus"/> key
        /// </param>
        /// <param name="invoiceKey">
        /// The invoice key
        /// </param>
        /// <param name="orderNumber">
        /// The order Number.
        /// </param>
        /// <param name="raiseEvents">
        /// Optional boolean indicating whether or not to raise events
        /// </param>
        /// <returns>
        /// The <see cref="IOrder"/>.
        /// </returns>
        /// <remarks>
        /// Order number must be a positive integer value or zero
        /// </remarks>
        IOrder CreateOrder(Guid orderStatusKey, Guid invoiceKey, int orderNumber, bool raiseEvents = true);

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
        /// Gets a <see cref="IOrder"/> given it's unique 'OrderNumber'
        /// </summary>
        /// <param name="orderNumber">The order number of the <see cref="IOrder"/> to be retrieved</param>
        /// <returns><see cref="IOrder"/></returns>
        IOrder GetByOrderNumber(int orderNumber);

        /// <summary>
        /// Gets a collection of <see cref="IOrder"/> for a given <see cref="IInvoice"/> key
        /// </summary>
        /// <param name="invoiceKey">The <see cref="IInvoice"/> key</param>
        /// <returns>A collection of <see cref="IOrder"/></returns>
        IEnumerable<IOrder> GetOrdersByInvoiceKey(Guid invoiceKey);            
            
        /// <summary>
        /// Gets list of <see cref="IOrder"/> objects given a list of Keys
        /// </summary>
        /// <param name="keys">List of guid 'key' for the invoices to retrieve</param>
        /// <returns>List of <see cref="IOrder"/></returns>
        IEnumerable<IOrder> GetByKeys(IEnumerable<Guid> keys);

        #region OrderStatus

        /// <summary>
        /// Gets an <see cref="IOrderStatus"/> by it's key
        /// </summary>
        /// <param name="key">The <see cref="IInvoiceStatus"/> key</param>
        /// <returns><see cref="IInvoiceStatus"/></returns>
        IOrderStatus GetOrderStatusByKey(Guid key);

        /// <summary>
        /// Returns a collection of all <see cref="IOrderStatus"/>
        /// </summary>
        IEnumerable<IOrderStatus> GetAllOrderStatuses();


        #endregion
         
    }
}