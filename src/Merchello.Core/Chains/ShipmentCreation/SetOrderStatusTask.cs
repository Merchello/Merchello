using System.Collections;
using System.Collections.Generic;

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
        /// The _shipment service.
        /// </summary>
        private readonly IShipmentService _shipmentService;

        /// <summary>
        /// Initializes a new instance of the <see cref="SetOrderStatusTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="keysToShip">
        /// The keys To Ship.
        /// </param>
        public SetOrderStatusTask(IMerchelloContext merchelloContext, IOrder order, IEnumerable<Guid> keysToShip) 
            : base(merchelloContext, order, keysToShip)
        {
            _orderService = MerchelloContext.Services.OrderService;
            _shipmentService = MerchelloContext.Services.ShipmentService;
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
            Guid orderStatusKey;

            // not fulfilled
            if (Order.ShippableItems().All(x => ((OrderLineItem)x).ShipmentKey == null))
            {
                orderStatusKey = Core.Constants.OrderStatus.NotFulfilled;
                return this.SaveOrderStatus(value, orderStatusKey);
            }

            if (Order.ShippableItems().Any(x => ((OrderLineItem)x).ShipmentKey == null))
            {
                orderStatusKey = Core.Constants.OrderStatus.Open;
                return this.SaveOrderStatus(value, orderStatusKey);
            }

            // now we need to look at all of the shipments to make sure the shipment statuses are either
            // shipped or delivered.  If either of those two, we will consider the shipment as 'Fulfilled',
            // otherwise the shipment will remain in the open status
            var shipmentKeys = Order.ShippableItems().Select(x => ((OrderLineItem)x).ShipmentKey.GetValueOrDefault()).Distinct();
            var shipments = _shipmentService.GetByKeys(shipmentKeys);
            orderStatusKey =
                shipments.All(x =>
                    x.ShipmentStatusKey.Equals(Core.Constants.ShipmentStatus.Delivered)
                    || x.ShipmentStatusKey.Equals(Core.Constants.ShipmentStatus.Shipped)) ?
                        Core.Constants.OrderStatus.Fulfilled :
                        Core.Constants.OrderStatus.Open;

            return this.SaveOrderStatus(value, orderStatusKey);
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