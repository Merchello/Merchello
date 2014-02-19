using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents the ShippingContext
    /// </summary>
    internal class ShippingContext : GatewayProviderTypedContextBase<ShippingGatewayProviderBase>, IShippingContext
    {
        public ShippingContext(IGatewayProviderService gatewayProviderService, IRuntimeCacheProvider runtimeCache) 
            : base(gatewayProviderService, runtimeCache)
        { }

        /// <summary>
        /// Returns a collection of all <see cref="IShipmentRateQuote"/> that are available for the <see cref="IShipment"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to quote</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public IEnumerable<IShipmentRateQuote> GetShipRateQuotesForShipment(IShipment shipment)
        {
            var providers = ResolveAllActiveProviders();
            var quotes = new List<IShipmentRateQuote>();
            foreach (var provider in providers)
            {
                quotes.AddRange(provider.QuoteAvailableShipMethodsForShipment(shipment));
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

        /// <summary>
        /// Resolves all active shipping gateway providers for a given <see cref="IShipCountry"/>
        /// </summary>
        /// <returns>A collection of all active shipping gateway providers</returns>
        public IEnumerable<IShippingGatewayProvider> GetGatewayProvidersByShipCountry(IShipCountry shipCountry)
        {
            var gatewayProviders = GatewayProviderService.GetGatewayProvidersByShipCountry(shipCountry);

            return
                gatewayProviders.Select(
                    provider => GatewayProviderResolver.ResolveByGatewayProvider<ShippingGatewayProviderBase>(provider));
        }


        /// <summary>
        /// Resolves all active shipping gateway providers
        /// </summary>
        /// <returns>A collection of all active shipping gateway providers</returns>
        public override IEnumerable<ShippingGatewayProviderBase> ResolveAllActiveProviders()
        {
            return GatewayProviderResolver.ResolveByGatewayProviderType<ShippingGatewayProviderBase>(GatewayProviderType.Shipping);
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