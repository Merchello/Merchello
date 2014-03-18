using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;

namespace Merchello.Core.Chains.ShipmentCreation
{
    internal class SetOrderStatusTask : OrderAttemptChainTaskBase
    {
        private readonly IOrderService _orderService;

        public SetOrderStatusTask(IMerchelloContext merchelloContext, IOrder order) 
            : base(merchelloContext, order)
        {
            _orderService = MerchelloContext.Services.OrderService;
        }

        public override Attempt<IShipment> PerformTask(IShipment value)
        {
            return Order.ShippableItems().All(x => ((OrderLineItem)x).ShipmentKey == null) 
                ? SaveOrderStatus(value, Constants.DefaultKeys.OrderStatus.NotFulfilled) 
                : SaveOrderStatus(value, Order.ShippableItems().All(x => ((OrderLineItem) x).ShipmentKey != null) 
                    ? Constants.DefaultKeys.OrderStatus.Fulfilled 
                    : Constants.DefaultKeys.OrderStatus.BackOrder);
        }

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