namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Exceptions;
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    using Umbraco.Core;

    /// <summary>
    /// Sets and checks the limit number of redemptions for this coupon.
    /// </summary>
    [OfferComponent("BE8D4D3A-5E9C-442E-9300-84492E4483D4", "Limit maximum number of redemptions", "Limits to maximum number of times this coupon can be redeemed.",
        "~/App_Plugins/Merchello/Backoffice/Merchello/Dialogs/marketing.offerconstraint.redemptionlimit.html", typeof(Coupon))]
    public class MaximumNumberOfRedemptionsConstraint : CouponConstraintBase
    { 
        /// <summary>
        /// Initializes a new instance of the <see cref="MaximumNumberOfRedemptionsConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public MaximumNumberOfRedemptionsConstraint(OfferComponentDefinition definition)
            : base(definition)
        {
        }


        /// <summary>
        /// Gets the display configuration format.
        /// This text is used by the back office UI to display configured values
        /// </summary>
        public override string DisplayConfigurationFormat
        {
            get
            {
                var maximum = this.GetConfigurationValue("maximum");

                // price and operator
                if (string.IsNullOrEmpty(maximum)) return string.Empty;

                return string.Format("'Maximum number of redemptions: {0} '", MaximumRedemptions == 0 ? "No limit" : maximum);
            }
        }

        /// <summary>
        /// Gets the maximum number of redemptions configured.
        /// </summary>
        private int MaximumRedemptions
        {
            get
            {
                int max;
                return int.TryParse(this.GetConfigurationValue("maximum"), out max) ? max : 0;
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
            if (MaximumRedemptions == 0) return Attempt<ILineItemContainer>.Succeed(value);

            if (MerchelloContext.Current != null)
            {
                var offerRedeemedService = ((ServiceContext)MerchelloContext.Current.Services).OfferRedeemedService;
                var offerSettingsKey = this.OfferComponentDefinition.OfferSettingsKey;
                var remptionCount = offerRedeemedService.GetOfferRedeemedCount(offerSettingsKey);

                return remptionCount >= MaximumRedemptions
                           ? this.Fail(value, "Redemption count would exceed the maximum number of allowed")
                           : this.Success(value);

            }
           
            return this.Fail(value, "MerchelloContext was null");
        }
    }
}