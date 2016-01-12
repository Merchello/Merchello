namespace Merchello.Core.Checkout
{
    using System.Collections.Generic;

    using Merchello.Core.Marketing.Offer;
    using Merchello.Core.Models;

    /// <summary>
    /// A manager for dealing with marketing offers during the checkout workflow.
    /// </summary>
    public interface ICheckoutOfferManager : ICheckoutContextManagerBase
    {
        /// <summary>
        /// Gets the offer codes.
        /// </summary>
        IEnumerable<string> OfferCodes { get; }

        /// <summary>
        /// Removes an offer code from the OfferCodes collection.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        void RemoveOfferCode(string offerCode);

        /// <summary>
        /// Clears the offer codes collection.
        /// </summary>
        void ClearOfferCodes();

        /// <summary>
        /// Attempts to redeem an offer to the sale.
        /// </summary>
        /// <param name="offerCode">
        /// The offer code.
        /// </param>
        /// <returns>
        /// The <see cref="IOfferRedemptionResult{ILineItem}"/>.
        /// </returns>
        IOfferRedemptionResult<ILineItem> RedeemCouponOffer(string offerCode);
    }
}