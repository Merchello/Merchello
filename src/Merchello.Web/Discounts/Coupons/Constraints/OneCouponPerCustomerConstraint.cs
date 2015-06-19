namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// A rule to enforce one discount per customer.
    /// </summary>
    [OfferComponent("A035E592-5D09-40BD-BFF6-73C3A4E9DDA2", "One coupon per customer", "The customer may only ever use this coupon once.  Not usable by anonymous customers.", RestrictToType = typeof(Coupon))]
    public class OneCouponPerCustomerConstraint : CouponConstraintBase
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
            if (customer.IsAnonymous) return Attempt<ILineItemContainer>.Fail(new OfferRedemptionException("Cannot be applied by anonymous customers."));

            if (MerchelloContext.Current != null)
            {
                var offerRedeemedService = ((ServiceContext)MerchelloContext.Current.Services).OfferRedeemedService;
                var offerSettingsKey = this.OfferComponentDefinition.OfferSettingsKey;
                var remptions = offerRedeemedService.GetByOfferSettingsKeyAndCustomerKey(offerSettingsKey, customer.Key);

                return remptions.Any()
                           ? this.Fail(value, "Customer has already redeemed this offer.")
                           : this.Success(value);

            }
           
            return Attempt<ILineItemContainer>.Fail(new NullReferenceException("MerchelloContext was null"));
        }
    }
}