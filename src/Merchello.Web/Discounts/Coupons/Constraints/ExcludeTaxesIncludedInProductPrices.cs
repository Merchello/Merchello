namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The exclude shipping cost constraint.
    /// </summary>
    [OfferComponent("5260EC60-52A5-4E7E-8F0D-34F81047DE9F", "Exclude taxes included in product prices", "Taxes will be removed from the product items.", RestrictToType = typeof(Coupon))]
    public class ExcludeTaxesIncludedInProductPrices : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeTaxesIncludedInProductPrices"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ExcludeTaxesIncludedInProductPrices(OfferComponentDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        /// Gets a value indicating this reward does not require configuration.
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
            if (MerchelloContext.Current != null)
            {
                if (!MerchelloContext.Current.Gateways.Taxation.ProductPricingEnabled) return this.Success(value);
                var vistor = new ExcludeTaxesInProductPricesVisitor();
                value.Items.Accept(vistor);
                return this.Success(value);
            }

            return Attempt<ILineItemContainer>.Fail(new NullReferenceException("MerchelloContext was null"));
        }
    }
}