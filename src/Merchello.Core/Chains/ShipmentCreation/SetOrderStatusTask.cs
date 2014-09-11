namespace Merchello.Core.Chains.ShipmentCreation
{
    using System;
    using System.Linq;
    using Models;
    using Services;
    using Umbraco.Core;

    /// <summary>
    /// The set order status task.
    /// </summary>
    internal class SetOrderStatusTask : OrderAttemptChainTaskBase
    {
        /// <summary>
        /// The _order service.
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetOrderStatusTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        public SetOrderStatusTask(IMerchelloContext merchelloContext, IOrder order) 
            : base(merchelloContext, order)
        {
            _orderService = MerchelloContext.Services.OrderService;
        }

        /// <summary>
        /// The perform task.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<IShipment> PerformTask(IShipment value)
        {            
            return Order.ShippableItems().All(x => ((OrderLineItem)x).ShipmentKey == null)
                ? SaveOrderStatus(value, Core.Constants.DefaultKeys.OrderStatus.NotFulfilled) : 
                SaveOrderStatus(
                    value,
                    Order.ShippableItems().Any(x => ((OrderLineItem)x).ShipmentKey == null) ? Core.Constants.DefaultKeys.OrderStatus.BackOrder : Core.Constants.DefaultKeys.OrderStatus.Fulfilled); 
        }

        /// <summary>
        /// The save order status.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="orderStatusKey">
        /// The order status key.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        private Attempt<IShipment> SaveOrderStatus(IShipment shipment, Guid orderStatusKey)
        {
            var orderStatus = _orderService.GetOrderStatusByKey(orderStatusKey);

            if (orderStatus == null) return Attempt<IShipment>.Fail(new NullReferenceException("Order status was not found"));

            Order.OrderStatus = orderStatus;
            _orderService.Save(Order);

            return Attempt<IShipment>.Succeed(shipment);
        }
    }
}