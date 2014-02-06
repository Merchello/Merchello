using Merchello.Core;
using Merchello.Core.Checkout;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;

namespace Merchello.Tests.Base.DataMakers
{
    internal class CheckoutMock : CheckoutBase
    {
        public CheckoutMock(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer) : base(merchelloContext, itemCache, customer)
        {
        }

    }
}