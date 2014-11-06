namespace Merchello.Core.Gateways.Shipping
{
    using System.Collections.Generic;

    using Merchello.Core.Models;

    using Umbraco.Core.Cache;

    /// <summary>
    /// Represents a Shipment Rate Quote Strategy
    /// </summary>
    public abstract class ShipmentRateQuoteStrategyBase : IShipmentRateQuoteStrategy
    {
        /// <summary>
        /// The shipment.
        /// </summary>
        private readonly IShipment _shipment;

        /// <summary>
        /// The shipping gateway methods.
        /// </summary>
        private readonly IEnumerable<IShippingGatewayMethod> _shippingGatewayMethods;

        /// <summary>
        /// The runtime cache.
        /// </summary>
        private readonly IRuntimeCacheProvider _runtimeCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShipmentRateQuoteStrategyBase"/> class.
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
        protected ShipmentRateQuoteStrategyBase(IShipment shipment, IShippingGatewayMethod[] shippingGatewayMethods, IRuntimeCacheProvider runtimeCache)
        {
            Mandate.ParameterNotNull(shipment, "shipment");
            Mandate.ParameterNotNull(shippingGatewayMethods, "gatewayShipMethods");
            Mandate.ParameterNotNull(runtimeCache, "runtimeCache");

            _shipment = shipment;
            _shippingGatewayMethods = shippingGatewayMethods;
            _runtimeCache = runtimeCache;
        }

        /// <summary>
        /// Gets the collection of <see cref="ShippingGatewayMethodBase"/>
        /// </summary>
        protected IEnumerable<IShippingGatewayMethod> ShippingGatewayMethods
        {
            get { return _shippingGatewayMethods; }
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
        /// Quotes all available ship methods
        /// </summary>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public abstract IEnumerable<IShipmentRateQuote> GetShipmentRateQuotes();

        /// <summary>
        /// Creates a cache key for caching <see cref="IShipmentRateQuote"/>s
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="shippingGatewayMethod">
        /// The shipping Gateway Method.
        /// </param>
        /// <returns>
        /// The cache key.
        /// </returns>
        protected static string GetShipmentRateQuoteCacheKey(IShipment shipment, IShippingGatewayMethod shippingGatewayMethod)
        {
            return Cache.CacheKeys.ShippingGatewayProviderShippingRateQuoteCacheKey(shipment.Key, shippingGatewayMethod.ShipMethod.Key, shipment.VersionKey);
        }

        /// <summary>
        /// Returns the cached <see cref="IShipmentRateQuote"/> if it exists
        /// </summary>
        /// <param name="shipment">
        /// The shipment.
        /// </param>
        /// <param name="shippingGatewayMethod">
        /// The shipping Gateway Method.
        /// </param>
        /// <returns>
        /// The <see cref="IShipmentRateQuote"/>.
        /// </returns>
        protected IShipmentRateQuote TryGetCachedShipmentRateQuote(IShipment shipment, IShippingGatewayMethod shippingGatewayMethod)
        {
            return _runtimeCache.GetCacheItem(GetShipmentRateQuoteCacheKey(shipment, shippingGatewayMethod)) as ShipmentRateQuote;
        }
    }
}