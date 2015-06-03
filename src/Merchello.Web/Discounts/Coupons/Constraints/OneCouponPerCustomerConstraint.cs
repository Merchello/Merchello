namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;

    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A rule to enforce one discount per customer.
    /// </summary>
    [OfferComponent("A035E592-5D09-40BD-BFF6-73C3A4E9DDA2", "One coupon per customer", "The customer may only ever use this coupon once.", RestrictToType = typeof(Coupon))]
    public class OneCouponPerCustomerConstraint : OfferConstraintComponentBase<ILineItemContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OneCouponPerCustomerConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        public OneCouponPerCustomerConstraint(OfferComponentDefinition definition)
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

        public override Attempt<ILineItemContainer> Apply(ILineItemContainer value, ICustomerBase customer)
        {
            if (customer.IsAnonymous)
            {
                var anonymousException = new Exception("Customer must be signed in to use this discount.");
                return Attempt<ILineItemContainer>.Fail(value, anonymousException);
            }

            throw new NotImplementedException();
        }
    }
}