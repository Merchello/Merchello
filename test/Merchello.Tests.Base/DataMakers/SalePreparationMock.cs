using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Sales;

namespace Merchello.Tests.Base.DataMakers
{
    using global::Umbraco.Core;

    using Merchello.Core.Marketing.Offer;

    internal class SalePreparationMock : SalePreparationBase
    {
        public SalePreparationMock(IMerchelloContext merchelloContext, IItemCache itemCache, ICustomerBase customer) 
            : base(merchelloContext, itemCache, customer)
        {

        }

        internal override Attempt<IOfferResult<TConstraint, TAward>> TryApplyOffer<TConstraint, TAward>(TConstraint validateAgainst, string offerCode)
        {
            throw new System.NotImplementedException();
        }
    }
}