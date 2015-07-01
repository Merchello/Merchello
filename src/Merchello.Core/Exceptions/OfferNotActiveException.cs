namespace Merchello.Core.Exceptions
{
    using System;

    /// <summary>
    /// An exception used when an offer redemption attempts fails
    /// </summary>
    public class OfferRedemptionException : Exception 
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferRedemptionException"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public OfferRedemptionException(string message)
            : base(message)
        {
        }
    }
}