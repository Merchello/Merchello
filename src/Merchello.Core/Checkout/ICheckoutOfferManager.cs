namespace Merchello.Core.Checkout
{
    using System.Collections.Generic;

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
    }
}