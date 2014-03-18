using System;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.ShipmentCreation
{
    internal class AddShippableOrderLineItemsToShipmentTask : OrderAttemptChainTaskBase
    {
        public AddShippableOrderLineItemsToShipmentTask(IMerchelloContext merchelloContext, IOrder order) 
            : base(merchelloContext, order)
        {
        }

        public override Attempt<IShipment> PerformTask(IShipment value)
        {
            var unfulfilled = Order.UnfulfilledItems(MerchelloContext).Where(x => x.BackOrder == false).ToArray();

            if(unfulfilled.Count() != Order.Items.Count) return Attempt<IShipment>.Fail(new InvalidOperationException("The order contains items that are either on back order or cannot be shipped."));

            foreach (var item in unfulfilled)
            {
                value.Items.Add(item);
            }

            return Attempt<IShipment>.Succeed(value);
        }
    }

}