namespace Merchello.Web.Discounts.Coupons.Constraints
{
    using System;

    using Merchello.Core.Marketing.Discounts;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A rule to enforce one discount per customer.
    /// </summary>
    [OfferComponent("A035E592-5D09-40BD-BFF6-73C3A4E9DDA2", "One coupon per customer", "The customer may only ever use this coupon once.", RestrictToType = typeof(Coupon))]
    public class OneCouponPerCustomerConstraint : DiscountConstraintBase
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
        /// Gets the category.
        /// </summary>
        public override DiscountCategory Category
        {
            get
            {
                return DiscountCategory.Customer;
            }
        }

        /// <summary>
        /// Validates the constraint against the <see cref="ILineItemContainer"/>
        /// </summary>
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{ILineItemContainer}"/> indicating whether or not the constraint can be enforced.
        /// </returns>
        public override Attempt<ILineItemContainer> Validate(ICustomerBase customer, ILineItemContainer collection)
        {
            if (customer.IsAnonymous)
            {
                var anonymousException = new Exception("Customer must be signed in to use this discount.");
                return Attempt<ILineItemContainer>.Fail(collection, anonymousException);
            }

            throw new NotImplementedException();
        }        
    }
}