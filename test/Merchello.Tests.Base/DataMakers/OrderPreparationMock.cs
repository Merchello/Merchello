using Merchello.Core;
using Merchello.Core.Checkout;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Orders;

namespace Merchello.Tests.Base.DataMakers
{
    internal class OrderPreparationMock : OrderPreparationBase
    {
        public OrderPreparationMock(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer) 
            : base(merchelloContext, itemCache, customer)
        {

        }

    }
}