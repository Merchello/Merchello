namespace Merchello.Core.Discounts
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines a discount constraint.
    /// </summary>
    public interface IDiscountConstraint
    {
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
        Attempt<ILineItemContainer> Validate(ICustomerBase customer, ILineItemContainer collection);
    }
}
