namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Defines the OfferProviderResolver
    /// </summary>
    internal interface IOfferProviderResolver
    {
        /// <summary>
        /// The a <see cref="OfferProviderBase"/> by it's unique key.
        /// </summary>
        /// <param name="key">
        /// The key.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferProvider"/>.
        /// </returns>
        OfferProviderBase GetByKey(Guid key);

        /// <summary>
        /// The get offer providers.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable{OfferProviderBase}"/>.
        /// </returns>
        IEnumerable<OfferProviderBase> GetOfferProviders();

        /// <summary>
        /// Gets an offer provider by it's type T.
        /// </summary>
        /// <typeparam name="T">
        /// The type of offer provider
        /// </typeparam>
        /// <returns>
        /// The instantiated offer provider
        /// </returns>
        T GetOfferProvider<T>() where T : OfferProviderBase;
    }
}