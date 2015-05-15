namespace Merchello.Core.Marketing.Discounts.Offer
{
    using System;

    /// <summary>
    /// The DiscountOfferProvider interface.
    /// </summary>
    public interface IDiscountOfferProvider
    {
        /// <summary>
        /// The create discount offer.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="offerStartDate">
        /// The offer start date.
        /// </param>
        /// <param name="offerExpiresDate">
        /// The offer expires date.
        /// </param>
        /// <param name="active">
        /// The active.
        /// </param>
        void CreateDiscountOffer(string name, DateTime offerStartDate, DateTime offerExpiresDate, bool active = true);
    }
}