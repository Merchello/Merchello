namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    [OfferComponent("A1CCE36A-C5AA-4C50-B659-CC2FBDEAA713", "Test reward", "REMOVE THIS.", RestrictToType = typeof(Coupon))]
    public class TestDiscountReward :  OfferRewardComponentBase<IInvoice, ILineItem>
    {
        public TestDiscountReward(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        public override bool RequiresConfiguration
        {
            get
            {
                return false;
            }
        }

        public override Attempt<ILineItem> Award(IInvoice validate, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}