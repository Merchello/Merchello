namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an Offer provider responsible for manage .
    /// </summary>
    /// <typeparam name="TOffer">
    /// The type of the Offer this provider is responsible for managing
    /// </typeparam>
    public interface IOfferBaseManager<out TOffer> where TOffer : OfferBase
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
    }
}