using System;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;

namespace Merchello.Core.Chains.ShipmentCreation
{
    /// <summary>
    /// Removes order items from inventory
    /// </summary>
    /// <remarks>
    /// 
    /// Note: this behavior is different than most other tasks as it actually updates database information.
    /// 
    /// </remarks>
    internal class RemoveShipmentOrderItemsFromInventoryAndPersistShipmentTask : OrderAttemptChainTaskBase
    {

        private readonly IShipmentService _shipmentService;
        private readonly IOrderService _orderService;
        private readonly IProductVariantService _productVariantService;

        public RemoveShipmentOrderItemsFromInventoryAndPersistShipmentTask(IMerchelloContext merchelloContext, IOrder order) 
            : base(merchelloContext, order)
        {
            _productVariantService = MerchelloContext.Services.ProductVariantService;
            _shipmentService = MerchelloContext.Services.ShipmentService;
            _orderService = MerchelloContext.Services.OrderService;
        }

        public override Attempt<IShipment> PerformTask(IShipment value)
        {
            var trackableItems = Order.InventoryTrackedItems().ToArray();

            var variants = _productVariantService.GetByKeys(trackableItems.Select(x => x.ExtendedData.GetProductVariantKey())).ToArray();

            if (variants.Any())
            {
                foreach (var item in trackableItems)
                {
                    var variant = variants.FirstOrDefault(x => x.Key == item.ExtendedData.GetProductVariantKey());
                    if (variant == null) return Attempt<IShipment>.Fail(new NullReferenceException("A ProductVariant reference in the order could not be found"));

                    var inventory = variant.CatalogInventories.FirstOrDefault(x => x.CatalogKey == item.ExtendedData.GetWarehouseCatalogKey());
                    if (inventory == null) return Attempt<IShipment>.Fail(new NullReferenceException("An inventory record could not be found for an order line item"));

                    inventory.Count -= item.Quantity;

                }

                _productVariantService.Save(variants);
            }

            // persist the shipment and update the line items
            if (value.ShipMethodKey == Guid.Empty) value.ShipMethodKey = null;
            _shipmentService.Save(value);

            foreach (var shipItem in value.Items)
            {
                ((OrderLineItem) Order.Items.First(x => x.Key == shipItem.Key)).ShipmentKey = value.Key;
            }

            _orderService.Save(Order);

            return Attempt<IShipment>.Succeed(value);
        }
    }
}