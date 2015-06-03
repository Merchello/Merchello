namespace Merchello.Web.Discounts.Coupons.Rewards
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Marketing.Rewards;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A coupon reward for free shipping.
    /// </summary>
    [OfferComponent("A9375BF1-4125-4B0C-ACB9-BAD4E1BA17BE", "FREE shipping", "Applies a discount equal to the shipping charges.", RestrictToType = typeof(Coupon))]
    public class CouponFreeShippingReward : OfferRewardComponentBase<ILineItemContainer, ILineItem>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponFreeShippingReward"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public CouponFreeShippingReward(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets a value indicating that this component does not require configuration.
        /// </summary>      
        public override bool RequiresConfiguration
        {
            get
            {
                return false;
            }
        }

        public override Attempt<ILineItem> Award(ILineItemContainer validate, ICustomerBase customer)
        {
            throw new System.NotImplementedException();
        }
    }
}