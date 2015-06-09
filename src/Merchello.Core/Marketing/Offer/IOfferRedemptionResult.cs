namespace Merchello.Core.Marketing.Offer
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// The result that is in an attempt to apply a offer to a sale.
    /// </summary>
    /// <typeparam name="TAward">
    /// The type of award
    /// </typeparam>
    public interface IOfferRedemptionResult<TAward>
        where TAward : class
    {
        /// <summary>
        /// Gets or sets a value indicating whether or not the offer application was successful.
        /// </summary>
        bool Success { get; set; }

        /// <summary>
        /// Gets or sets the award.
        /// </summary>
        /// <remarks>
        /// Can be null on exception
        /// </remarks>
        TAward Award { get; set; }

        /// <summary>
        /// Gets or sets the exception.
        /// </summary>
        Exception Exception { get; set; }

        /// <summary>
        /// Gets the messages.
        /// </summary>
        IEnumerable<string> Messages { get; }

        /// <summary>
        /// Adds a message.
        /// </summary>
        /// <param name="msg">
        /// The message.
        /// </param>
        void AddMessage(string msg);

        /// <summary>
        /// Adds a collection of messages.
        /// </summary>
        /// <param name="messages">
        /// The messages.
        /// </param>
        void AddMessage(IEnumerable<string> messages);

    }
}