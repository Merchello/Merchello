namespace Merchello.Core.Marketing.Discounts.Constraints
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines a Discount Constraint.
    /// </summary>
    public interface IDiscountConstraint
    {
        /// <summary>
        /// Gets the category.
        /// </summary>
        DiscountCategory Category { get; }

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
        /// The <see cref="Attempt{T}"/> indicating whether or not the constraint can be enforced.
        /// </returns>
        Attempt<ILineItemContainer> Validate(ICustomerBase customer, ILineItemContainer collection);
    }
}