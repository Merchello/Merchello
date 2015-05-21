namespace Merchello.Core.Marketing.Offer
{
    using System;

    /// <summary>
    /// The DiscountOfferProvider interface.
    /// </summary>
    public interface IOfferProvider
    {
        /// <summary>
        /// Creates a <see cref="IOffer"/>.
        /// </summary>
        /// <param name="name">
        /// The name of the offer.
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
        IOffer CreateOffer(string name, DateTime offerStartDate, DateTime offerExpiresDate, bool active = true);
    }
}