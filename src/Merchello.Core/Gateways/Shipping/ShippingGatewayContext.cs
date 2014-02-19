using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents the ShippingGatewayContext
    /// </summary>
    internal class ShippingGatewayContext : ProviderTypedGatewayContextBase<ShippingGatewayProviderBase>, IShippingGatewayContext
    {
        public ShippingGatewayContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache) 
            : base(gatewayProviderService, runtimeCache)
        { }

        /// <summary>
        /// Returns a collection of all <see cref="IShipmentRateQuote"/> that are available for the <see cref="IShipment"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to quote</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public IEnumerable<IShipmentRateQuote> GetShipRateQuotesForShipment(IShipment shipment)
        {
            var providers = GatewayProviderResolver.ResolveByGatewayProviderType(GatewayProviderType.Shipping);
            var quotes = new List<IShipmentRateQuote>();
            foreach (var provider in providers)
            {
                quotes.AddRange(((ShippingGatewayProviderBase)provider).QuoteAvailableShipMethodsForShipment(shipment));
            }
            return quotes.OrderBy(x => x.Rate);
        }

        /// <summary>
        /// Returns a list of all countries that can be assigned to a shipment
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICountry> GetAllowedShipmentDestinationCountries()
        {
            var countries = GatewayProviderService.GetAllShipCountries().Select(x => new Country(x.CountryCode, x.Provinces));

            return countries.Distinct();
        }

        public IEnumerable<IShippingGatewayProvider> GetGatewayProvidersByShipCountry(IShipCountry shipCountry)
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<ShippingGatewayProviderBase> ResolveAllActiveProviders()
        {
        }

        /// <summary>
        /// Resolves a shipping gateway provider by it's unique key
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A shipping gateway provider</returns>
        public override ShippingGatewayProviderBase ResolveByKey(Guid key)
        {
            return GatewayProviderResolver.ResolveByKey<ShippingGatewayProviderBase>(key);
        }
    }
}