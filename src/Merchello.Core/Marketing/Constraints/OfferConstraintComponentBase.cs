namespace Merchello.Core.Marketing.Discounts
{
    using Merchello.Core.Marketing.Constraints;
    using Merchello.Core.Marketing.Discounts.Constraints;
    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// A base class for Discount Rules
    /// </summary>
    public abstract class OfferConstraintComponentBase : OfferComponentBase, IOfferConstraintComponent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferConstraintComponentBase"/> class.
        /// </summary>
        /// <param name="definition">
        /// The <see cref="OfferComponentDefinition"/>.
        /// </param>
        protected OfferConstraintComponentBase(OfferComponentDefinition definition)
            : base(definition)
        {
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
        public abstract Attempt<ILineItemContainer> Validate(ICustomerBase customer, ILineItemContainer collection);
    }
}