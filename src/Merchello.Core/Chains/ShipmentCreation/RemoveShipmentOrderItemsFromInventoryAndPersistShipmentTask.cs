namespace Merchello.Core.Chains.ShipmentCreation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Services;
    using Umbraco.Core;

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
        /// <summary>
        /// The shipment service.
        /// </summary>
        private readonly IShipmentService _shipmentService;

        /// <summary>
        /// The order service.
        /// </summary>
        private readonly IOrderService _orderService;

        /// <summary>
        /// The product variant service.
        /// </summary>
        private readonly IProductVariantService _productVariantService;

        /// <summary>
        /// Initializes a new instance of the <see cref="RemoveShipmentOrderItemsFromInventoryAndPersistShipmentTask"/> class.
        /// </summary>
        /// <param name="merchelloContext">
        /// The merchello context.
        /// </param>
        /// <param name="order">
        /// The order.
        /// </param>
        /// <param name="keysToShip">
        /// The keys to ship.
        /// </param>
        public RemoveShipmentOrderItemsFromInventoryAndPersistShipmentTask(IMerchelloContext merchelloContext, IOrder order, IEnumerable<Guid> keysToShip) 
            : base(merchelloContext, order, keysToShip)
        {
            _productVariantService = MerchelloContext.Services.ProductVariantService;
            _shipmentService = MerchelloContext.Services.ShipmentService;
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
            var trackableItems = Order.InventoryTrackedItems().Where(x => KeysToShip.Contains(x.Key)).ToArray();

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

            return Attempt<IShipment>.Succeed(value);
        }
    }
}