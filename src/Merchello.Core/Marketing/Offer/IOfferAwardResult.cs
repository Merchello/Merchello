namespace Merchello.Core.Marketing.Offer
{
    using System.Collections;
    using System.Collections.Generic;

    /// <summary>
    /// Defines an OfferAwardResult.
    /// </summary>
    /// <typeparam name="T">
    /// The type of Award
    /// </typeparam>
    public interface IOfferAwardResult<T>
        where T : class
    {
        /// <summary>
        /// Gets or sets the award.
        /// </summary>
        T Award { get; set; }

        /// <summary>
        /// Gets or sets the messages.
        /// </summary>
        IEnumerable<string> Messages { get; set; }
    }
}