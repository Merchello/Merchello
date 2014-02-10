using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents a Shipment Rate Quote Strategy
    /// </summary>
    public abstract class ShipmentRateQuoteStrategyBase : IShipmentRateQuoteStrategy
    {
        private readonly IShipment _shipment;
        private readonly IEnumerable<IGatewayShipMethod> _gatewayShipMethods;
        private readonly IRuntimeCacheProvider _runtimeCache;

        protected ShipmentRateQuoteStrategyBase(IShipment shipment, IGatewayShipMethod[] gatewayShipMethods, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(shipment, "shipment");
            Mandate.ParameterNotNull(gatewayShipMethods, "gatewayShipMethods");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");

            _shipment = shipment;
            _gatewayShipMethods = gatewayShipMethods;
            _runtimeCache = runtimeCache;
        }

        /// <summary>
        /// Quotes all available shipmethods
        /// </summary>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public abstract IEnumerable<IShipmentRateQuote> GetShipmentRateQuotes();

        /// <summary>
        /// Gets the collection of <see cref="GatewayShipMethodBase"/>
        /// </summary>
        protected IEnumerable<IGatewayShipMethod> GatewayShipMethods
        {
            get { return _gatewayShipMethods; }
        }

        /// <summary>
        /// Gets the <see cref="IShipment"/>
        /// </summary>
        protected IShipment Shipment
        {
            get { return _shipment; }
        }

        /// <summary>
        /// Gets the <see cref="IRuntimeCacheProvider"/>
        /// </summary>
        protected IRuntimeCacheProvider RuntimeCache
        {
            get { return _runtimeCache; }
        }

        /// <summary>
        /// Returns the cached <see cref="IShipmentRateQuote"/> if it exists
        /// </summary>
        protected IShipmentRateQuote TryGetCachedShipmentRateQuote(IShipment shipment, IGatewayShipMethod gatewayShipMethod)
        {
            return _runtimeCache.GetCacheItem(GetShipmentRateQuoteCacheKey(shipment, gatewayShipMethod)) as ShipmentRateQuote;
        }

        /// <summary>
        /// Creates a cache key for caching <see cref="IShipmentRateQuote"/>s
        /// </summary>
        /// <param name="shipment"></param>
        /// <param name="gatewayShipMethod"></param>
        /// <returns></returns>
        protected static string GetShipmentRateQuoteCacheKey(IShipment shipment, IGatewayShipMethod gatewayShipMethod)
        {
            return Cache.CacheKeys.ShippingGatewayProviderShippingRateQuoteCacheKey(shipment.Key, gatewayShipMethod.ShipMethod.Key, shipment.VersionKey);
        }
    }
}