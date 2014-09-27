namespace Merchello.Plugin.Payments.Braintree.Api
{
    using System.Collections;
    using System.Collections.Generic;

    using global::Braintree;

    /// <summary>
    /// Defines the BraintreeSubscriptionApiProvider.
    /// </summary>
    public interface IBraintreeSubscriptionApiProvider
    {
        /// <summary>
        /// Gets a list of all discounts.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{Discount}"/>.
        /// </returns>
        IEnumerable<Discount> GetAllDiscounts(); 
            
        /// <summary>
        /// Gets a list of all AddOn(s).
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{AddOn}"/>.
        /// </returns>
        IEnumerable<AddOn> GetAllAddOns();
    }
}