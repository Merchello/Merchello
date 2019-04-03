namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount rule to allow a discount to be used with other discounts.
    /// </summary>
    [OfferComponent("8106FD8A-9BA3-4FFD-B698-89C28037F5FC", "Can be used with other coupons", "This coupon can be used with other coupons, it will override any clashes with the 'Not usable with other coupons' constaint", RestrictToType = typeof(Coupon))]
    public class CanBeUsedWithOtherCouponsConstraint : CouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanBeUsedWithOtherCouponsConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        public CanBeUsedWithOtherCouponsConstraint(OfferComponentDefinition definition)
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
            return this.Success(value);
        }
    }
}