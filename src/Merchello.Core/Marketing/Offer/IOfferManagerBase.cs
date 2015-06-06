namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core;

    /// <summary>
    /// Defines an Offer provider responsible for manage .
    /// </summary>
    /// <typeparam name="TOffer">
    /// The type of the Offer this provider is responsible for managing
    /// </typeparam>
    public interface IOfferManagerBase<TOffer> 
        where TOffer : OfferBase
    {
        /// <summary>
        /// Gets an offer by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="TOffer"/>.
        /// </returns>
        TOffer GetByKey(Guid key);

        /// <summary>
        /// Gets a collection of <see cref="TOffer"/> by their unique keys
        /// </summary>
        /// <param name="keys">
        /// The keys.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TOffer}"/>.
        /// </returns>
        IEnumerable<TOffer> GetByKeys(IEnumerable<Guid> keys);

        /// <summary>
        /// Gets an offer by it's offer code.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="customer">
        /// The customer.
        /// </param>
        /// <returns>
        /// The <see cref="Attempt"/>.
        /// </returns>
        Attempt<TOffer> GetByOfferCode(string offerCode, ICustomerBase customer);

        /// <summary>
        /// Gets an offer by it's offer code.
        /// </summary>
        /// <typeparam name="TConstraint">
        /// The type of constraint
        /// </typeparam>
        /// <typeparam name="TAward">
        /// The type of offer award
        /// </typeparam>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="customer">The customer</param>
        /// <returns>
        /// The <see cref="TOffer"/>.
        /// </returns>
        Attempt<TOffer> GetByOfferCode<TConstraint, TAward>(string offerCode, ICustomerBase customer)
            where TConstraint : class 
            where TAward : class;
    }
}