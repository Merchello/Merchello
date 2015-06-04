namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;

    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount rule to prohibit a discount from being used with other discounts.
    /// </summary>
    [OfferComponent("BDFEF8AC-B572-43E6-AB42-C07678500C87", "Not usable with other discounts", "This discount cannot be used with other discounts.", RestrictToType = typeof(Coupon))]
    public class NotUsableWithOtherCouponsConstraint : OfferConstraintComponentBase<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotUsableWithOtherCouponsConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        public NotUsableWithOtherCouponsConstraint(OfferComponentDefinition definition)
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


        public override Attempt<ILineItemContainer> TryApply(ILineItemContainer value, ICustomerBase customer)
        {
            throw new NotImplementedException();
        }
    }
}