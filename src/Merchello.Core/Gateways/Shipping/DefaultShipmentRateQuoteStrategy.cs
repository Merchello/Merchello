using System.Collections.Generic;
using Merchello.Core.Models;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents the default Shipment Rate Quoting Strategy
    /// </summary>
    internal class DefaultShipmentRateQuoteStrategy : ShipmentRateQuoteStrategyBase
    {
        public DefaultShipmentRateQuoteStrategy(IShipment shipment, IGatewayShipMethod[] gatewayShipMethods, IRuntimeCacheProvider runtimeCache) 
            : base(shipment, gatewayShipMethods, runtimeCache)
        { }

        /// <summary>
        /// Quotes all available shipmethods
        /// </summary>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public override IEnumerable<IShipmentRateQuote> GetShipmentRateQuotes()
        {
            var quotes = new List<IShipmentRateQuote>();
            foreach (var gwShipMethod in GatewayShipMethods)
            {
                var rateQuote = TryGetCachedShipmentRateQuote(Shipment, gwShipMethod);

                if (rateQuote == null)
                {
                    var attempt = gwShipMethod.QuoteShipment(Shipment);
                    if (attempt.Success)
                    {
                        rateQuote = attempt.Result;

                        RuntimeCache.GetCacheItem(GetShipmentRateQuoteCacheKey(Shipment, gwShipMethod), () => rateQuote);
                    }
                }
                if (rateQuote != null) quotes.Add(rateQuote);
            }

            return quotes;
        }
    }
}