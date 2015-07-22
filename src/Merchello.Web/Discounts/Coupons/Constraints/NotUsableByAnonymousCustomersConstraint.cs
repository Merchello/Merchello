namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The not usable by anonymous customers constraint.
    /// </summary>
    [OfferComponent("49B7FFBD-B2C5-4629-BC88-401D3B3136D6", "Not usable by anonymous customers", "This coupon cannot be used by anonymous customers.", RestrictToType = typeof(Coupon))]
    public class NotUsableByAnonymousCustomersConstraint : CouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotUsableByAnonymousCustomersConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public NotUsableByAnonymousCustomersConstraint(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets a value indicating whether requires configuration.
        /// </summary>
        public override bool RequiresConfiguration
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// The try apply.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        public override Attempt<ILineItemContainer> TryApply(ILineItemContainer value, ICustomerBase customer)
        {
            return customer.IsAnonymous ? this.Fail(value, "Customer cannont be anonymous") : this.Success(value);
        }
    }
}