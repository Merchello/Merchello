namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using System;
    using System.Linq;
    using Umbraco.Core;

    /// <summary>
    /// A discount rule to prohibit a discount from being used with other discounts.
    /// </summary>
    [OfferComponent("BDFEF8AC-B572-43E6-AB42-C07678500C87", "Not usable with other coupons", "This coupon cannot be used with other coupons.", RestrictToType = typeof(Coupon))]
    public class NotUsableWithOtherCouponsConstraint : CouponConstraintBase
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

        /// <summary>
        /// Validates the constraint against the <see cref="ILineItemContainer"/>
        /// </summary>
        /// <param name="value">
        /// The value to object to which the constraint is to be applied.
        /// </param>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{ILineItemContainer}"/> indicating whether or not the constraint can be enforced.
        /// </returns>
        public override Attempt<ILineItemContainer> TryApply(ILineItemContainer value, ICustomerBase customer)
        {
            var overrideConstraintId = Guid.Parse("8106FD8A-9BA3-4FFD-B698-89C28037F5FC");
            return !(value.Items.Any(x => x.ContainsCoupon() && !x.ExtendedData.HasCouponConstraint(overrideConstraintId)))
                       ? this.Success(value)
                       : this.Fail(value, "One or more coupons have already been added.");
        }
    }
}