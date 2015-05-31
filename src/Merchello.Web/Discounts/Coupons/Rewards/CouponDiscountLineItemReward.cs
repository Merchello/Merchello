namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Represents a discount line item reward.
    /// </summary>
    [OfferComponent("A1CCE36A-C5AA-4C50-B659-CC2FBDEAA7B3", "Discount the price", "Applies a discount according to configured price rules.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.couponreward.discountprice.html", typeof(Coupon))]
    public class CouponDiscountLineItemReward : OfferRewardComponentBase<ILineItemContainer, ILineItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponDiscountLineItemReward"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        public CouponDiscountLineItemReward(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        public override Attempt<ILineItem> Award(ILineItemContainer validate, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}