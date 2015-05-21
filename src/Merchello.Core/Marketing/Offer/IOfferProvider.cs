namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The DiscountOfferProvider interface.
    /// </summary>
    public interface IOfferProvider
    {
        /// <summary>
        /// Gets the key.
        /// </summary>
        Guid Key { get; }

        /// <summary>
        /// Creates a <see cref="IOffer"/> without saving it's settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name of the offer.
        /// </param>
        /// <param name="offerCode">
        /// The offer code
        /// </param>
        /// <param name="offerStartDate">
        /// The start of the offer valid period.
        /// </param>
        /// <param name="offerExpiresDate">
        /// The offer expires date.
        /// </param>
        /// <param name="active">
        /// A value indicating whether or not this offer is active.  Overrides the valid date period.
        /// </param>
        /// <returns>
        /// The <see cref="IOffer"/>.
        /// </returns>
        OfferBase CreateOffer(string name, string offerCode, DateTime offerStartDate, DateTime offerExpiresDate, bool active = true);

        /// <summary>
        /// Creates a <see cref="IOffer"/> and saves its settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name of the offer.
        /// </param>
        /// <param name="offerCode">
        /// The offer code
        /// </param>
        /// <param name="offerStartDate">
        /// The start of the offer valid period.
        /// </param>
        /// <param name="offerExpiresDate">
        /// The offer expires date.
        /// </param>
        /// <param name="active">
        /// A value indicating whether or not this offer is active.  Overrides the valid date period.
        /// </param>
        /// <returns>
        /// The <see cref="IOffer"/>.
        /// </returns>
        OfferBase CreateOfferWithKey(string name, string offerCode, DateTime offerStartDate, DateTime offerExpiresDate, bool active = true);

        /// <summary>
        /// Saves the offer
        /// </summary>
        /// <param name="offer">
        /// The offer.
        /// </param>
        void Save(OfferBase offer);

        /// <summary>
        /// Deletes the offer
        /// </summary>
        /// <param name="offer">
        /// The offer.
        /// </param>
        void Delete(OfferBase offer);


        IEnumerable<OfferBase> GetOffers(bool activeOnly = true); 
            
        /// <summary>
        /// Gets a collection of all <see cref="IOffer"/>s managed by this provider.
        /// </summary>
        /// <typeparam name="T">
        /// </typeparam>
        /// <param name="activeOnly">
        /// Optional value indicating whether or not to only return offers marked as active
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{T}"/>.
        /// </returns>
        IEnumerable<T> GetOffers<T>(bool activeOnly = true) where T : OfferBase;


        OfferBase GetByKey(Guid key);

        /// <summary>
        /// Gets an offer managed by this provider by it's key
        /// </summary>
        /// <param name="key">The OfferSettings key</param>
        /// <returns>
        /// The <see cref="IOffer"/>
        /// </returns>
        T GetByKey<T>(Guid key);
    }
}