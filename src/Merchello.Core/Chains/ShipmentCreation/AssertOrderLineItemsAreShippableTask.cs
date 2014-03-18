using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core;

namespace Merchello.Core.Chains.ShipmentCreation
{
    internal class AssertOrderLineItemsAreShippableTask : OrderAttemptChainTaskBase
    {
        public AssertOrderLineItemsAreShippableTask(IMerchelloContext merchelloContext, IOrder order) 
            : base(merchelloContext, order)
        {
        }

        public override Attempt<IShipment> PerformTask(IShipment value)
        {
            var unfulfilled = Order.UnfulfilledItems(MerchelloContext);

            return Attempt<IShipment>.Succeed(value);

            // if(unfulfilled.Count() != Order.Items.Count) return Attempt<IShipment>.Fail(new );

        }
    }
}