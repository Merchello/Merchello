using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Sales;

namespace Merchello.Tests.Base.DataMakers
{
    internal class SalePreparationMock : SalePreparationBase
    {
        public SalePreparationMock(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer) 
            : base(merchelloContext, itemCache, customer)
        {

        }

    }
}