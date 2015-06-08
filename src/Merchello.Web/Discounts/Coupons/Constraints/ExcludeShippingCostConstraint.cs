namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;
    using System.Linq;

    using Merchello.Core;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// The exclude shipping cost constraint.
    /// </summary>
    [OfferComponent("BE93358C-C7BC-489F-9177-DB570F6C7A1F", "Exclude shipping (freight) charges", "Shipping costs will be removed from the qualifying items.", RestrictToType = typeof(Coupon))]
    public class ExcludeShippingCostConstraint : CollectionAlterationCouponConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExcludeShippingCostConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The definition.
        /// </param>
        public ExcludeShippingCostConstraint(OfferComponentDefinition definition)
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
            return Attempt<ILineItemContainer>.Succeed(CreateNewLineContainer(value.Items.Where(x => x.LineItemType != LineItemType.Shipping)));
        }
    }
}