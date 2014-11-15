namespace Merchello.Core.Gateways.Shipping
{
    using System.Collections.Generic;

    using Merchello.Core.Strategies;

    /// <summary>
    /// Defines the shipment rate quote strategy
    /// </summary>
    public interface IShipmentRateQuoteStrategy : IStrategy
    {
        /// <summary>
        /// Quotes all available ship methods
        /// </summary>
        /// <param name="tryGetCached">
        /// The try Get Cached.
        /// </param>
        /// <returns>
        /// A collection of <see cref="IShipmentRateQuote"/>
        /// </returns>
        IEnumerable<IShipmentRateQuote> GetShipmentRateQuotes(bool tryGetCached = true);
    }
}