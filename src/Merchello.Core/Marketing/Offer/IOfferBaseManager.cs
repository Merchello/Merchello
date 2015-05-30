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
    public interface IOfferBaseManager<TOffer> where TOffer : OfferBase
    {
        /// <summary>
        /// Creates a <see cref="IOffer"/> without saving it's settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <returns>
        /// The <see cref="OfferBase"/>.
        /// </returns>
        TOffer Create(string name, string offerCode, bool active = true);

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
        /// <param name="offerEndsDate">
        /// The offer expires date.
        /// </param>
        /// <param name="active">
        /// A value indicating whether or not this offer is active.  Overrides the valid date period.
        /// </param>
        /// <returns>
        /// The <see cref="IOffer"/>.
        /// </returns>
        TOffer Create(string name, string offerCode, DateTime offerStartDate, DateTime offerEndsDate, bool active = true);

        /// <summary>
        /// Creates a <see cref="IOffer"/> without saving it's settings to the database.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        /// <returns>
        /// The <see cref="OfferBase"/>.
        /// </returns>
        TOffer CreateWithKey(string name, string offerCode, bool active = true);

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
        /// <param name="offerEndsDate">
        /// The offer expires date.
        /// </param>
        /// <param name="active">
        /// A value indicating whether or not this offer is active.  Overrides the valid date period.
        /// </param>
        /// <returns>
        /// The <see cref="IOffer"/>.
        /// </returns>
        TOffer CreateWithKey(string name, string offerCode, DateTime offerStartDate, DateTime offerEndsDate, bool active = true);

        /// <summary>
        /// Saves the offer
        /// </summary>
        /// <param name="offer">
        /// The offer.
        /// </param>
        void Save(TOffer offer);

        /// <summary>
        /// Deletes the offer
        /// </summary>
        /// <param name="offer">
        /// The offer.
        /// </param>
        void Delete(TOffer offer);

        /// <summary>
        /// Gets the collection of offers this provider manages.
        /// </summary>
        /// <param name="activeOnly">
        /// The active only.
        /// </param>
        /// <returns>
        /// The <see cref="IEnumerable{TOffer}"/>.
        /// </returns>
        IEnumerable<TOffer> Get(bool activeOnly = true);

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