namespace Merchello.Core.Chains.ShipmentCreation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Models;
    using Umbraco.Core;

    /// <summary>
    /// The add shippable order line items to shipment task.
    /// </summary>
    internal class AddShippableOrderLineItemsToShipmentTask : OrderAttemptChainTaskBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddShippableOrderLineItemsToShipmentTask"/> class.
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
        public AddShippableOrderLineItemsToShipmentTask(IMerchelloContext merchelloContext, IOrder order, IEnumerable<Guid> keysToShip)
            : base(merchelloContext, order, keysToShip)
        {
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
            var unfulfilled = Order.UnfulfilledItems(MerchelloContext).Where(x => x.BackOrder == false && KeysToShip.Contains(x.Key)).ToArray();

            if (unfulfilled.Count() != Order.Items.Count(x => KeysToShip.Contains(x.Key))) return Attempt<IShipment>.Fail(new InvalidOperationException("The order contains items that are either on back order or cannot be shipped."));

            foreach (var item in unfulfilled)
            {
                value.Items.Add(item);
            }

            return Attempt<IShipment>.Succeed(value);
        }
    }

}