using System.Collections.Generic;
using Merchello.Core.Strategies;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Defines the shipment rate quote strategy
    /// </summary>
    public interface IShipmentRateQuoteStrategy : IStrategy
    {
        /// <summary>
        /// Quotes all available shipmethods
        /// </summary>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        IEnumerable<IShipmentRateQuote> GetShipmentRateQuotes();
    }
}