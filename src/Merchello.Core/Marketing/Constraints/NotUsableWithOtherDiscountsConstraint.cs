namespace Merchello.Core.Marketing.Discounts.Constraints
{
    using System;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A discount rule to prohibit a discount from being used with other discounts.
    /// </summary>
    [OfferComponent("BDFEF8AC-B572-43E6-AB42-C07678500C87", "Not usable with other discounts", "This discount cannot be used with other discounts.")]
    public class NotUsableWithOtherDiscountsConstraint : DiscountConstraintBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotUsableWithOtherDiscountsConstraint"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        public NotUsableWithOtherDiscountsConstraint(OfferComponentDefinition definition)
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
                return DiscountCategory.Sale;                
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