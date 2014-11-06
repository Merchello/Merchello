namespace Merchello.Core.Gateways.Shipping
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Cache;

    /// <summary>
    /// Represents the default Shipment Rate Quoting Strategy
    /// </summary>
    internal class DefaultShipmentRateQuoteStrategy : ShipmentRateQuoteStrategyBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultShipmentRateQuoteStrategy"/> class.
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="shippingGatewayMethods">
        /// The shipping gateway methods.
        /// </param>
        /// <param name="runtimeCache">
        /// The runtime cache.
        /// </param>
        public DefaultShipmentRateQuoteStrategy(
            IShipment shipment,
            IShippingGatewayMethod[] shippingGatewayMethods,
            IRuntimeCacheProvider runtimeCache)
            : base(shipment, shippingGatewayMethods, runtimeCache)
        {            
        }

        /// <summary>
        /// Quotes all available ship methods
        /// </summary>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public override IEnumerable<IShipmentRateQuote> GetShipmentRateQuotes()
        {
            var quotes = new List<IShipmentRateQuote>();

            foreach (var gwShipMethod in ShippingGatewayMethods)
            {
                var rateQuote = TryGetCachedShipmentRateQuote(Shipment, gwShipMethod);

                if (rateQuote == null)
                {
                    //// http://issues.merchello.com/youtrack/issue/M-458
                    //// Clones the shipment so that with each iteration so that we can have the same shipment
                    //// with different ship methods
                    var attempt = gwShipMethod.QuoteShipment(Shipment.Clone());
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