namespace Merchello.Core.Marketing.Discounts.Constraints
{
    using System;

    using Merchello.Core.Marketing.Discounts.Coupons.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount validation rule to restrict this discount to a selection of products.
    /// </summary>
    public class RestrictToProductSelectionConstraint : DiscountConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RestrictToProductSelectionConstraint"/> class.
        /// </summary>
        /// <param name="settings">
        /// The settings.
        /// </param>
        public RestrictToProductSelectionConstraint(OfferComponentDefinition settings)
            : base(settings)
        {
        }

        /// <summary>
        /// Gets the category.
        /// </summary>
        public override DiscountCategory Category
        {
            get
            {
                return DiscountCategory.Product;
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
            throw new NotImplementedException();
        }
    }
}