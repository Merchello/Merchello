namespace Merchello.Web.Models.Coupons
{
    using Merchello.Core.Marketing.Discounts;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    /// <summary>
    /// Defines a coupon line item reward.
    /// </summary>
    [OfferComponent("A9943C55-37E6-4E6B-BBA4-7A10CF156429", "Coupon line item discount", "Generates a discount line item for a coupon")]
    public class CouponLineItemReward : DiscountLineItemReward
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CouponLineItemReward"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="container">
        /// The <see cref="ILineItemContainer"/>.
        /// </param>
        public CouponLineItemReward(OfferComponentDefinition definition, ICustomerBase customer, ILineItemContainer container)
            : base(definition, customer, container)
        {
        }

        public override bool Award()
        {
            throw new System.NotImplementedException();
        }
    }
}