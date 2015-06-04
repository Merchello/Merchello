namespace Merchello.Core.Marketing.Offer
{
    using System.Collections.Generic;

    /// <summary>
    /// The offer reward result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Award
    /// </typeparam>
    public class OfferAwardResult<T> : IOfferAwardResult<T> where T : class
    {
        /// <summary>
        /// Gets or sets the award.
        /// </summary>
        public T Award { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        public IEnumerable<string> Messages { get; set; }
    }
}