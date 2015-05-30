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
        /// <param name="customer">
        /// The <see cref="ICustomerBase"/>.
        /// </param>
        /// <param name="collection">
        /// The collection.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt{T}"/> indicating whether or not the constraint can be enforced.
        /// </returns>
        Attempt<T> Validate(ICustomerBase customer, ILineItemContainer collection);
    }
}