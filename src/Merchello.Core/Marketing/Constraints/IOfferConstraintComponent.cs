namespace Merchello.Core.Marketing.Constraints
{
    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines a Discount Constraint.
    /// </summary>
    /// <typeparam name="T">
    /// The type of object to attempt to return
    /// </typeparam>
    public interface IOfferConstraintComponent<T>
    {
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
        Attempt<T> TryApply(T value, ICustomerBase customer);
    }
}