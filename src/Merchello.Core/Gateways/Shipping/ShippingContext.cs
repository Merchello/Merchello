using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Services;

namespace Merchello.Core.Gateways.Shipping
{
    /// <summary>
    /// Represents the ShippingContext
    /// </summary>
    internal class ShippingContext : GatewayProviderTypedContextBase<ShippingGatewayProviderBase>, IShippingContext
    {
        private readonly IStoreSettingService _storeSettingService;

        public ShippingContext(IGatewayProviderService gatewayProviderService, IStoreSettingService storeSettingService,
            IGatewayProviderResolver resolver)
            : base(gatewayProviderService, resolver)
        {
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");

            _storeSettingService = storeSettingService;
        }

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
                quotes.AddRange(provider.QuoteShippingGatewayMethodsForShipment(shipment));
            }
            return quotes.OrderBy(x => x.Rate);
        }

        /// <summary>
        /// Returns a list of all countries that can be assigned to a shipment
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ICountry> GetAllowedShipmentDestinationCountries()
        {
            var shipCountries = GatewayProviderService.GetAllShipCountries().ToArray();

            var elseCountries = shipCountries.Where(x => x.CountryCode == "ELSE").ToArray();
            if (elseCountries.Any())
            {
                // get a list of all providers associated with the else countries
                var providers = new List<IShippingGatewayProvider>();
                foreach (var ec in elseCountries)
                {
                    providers.AddRange(GetGatewayProvidersByShipCountry(ec));
                }

                if (providers.Any(x => x.ShipMethods.Any()))
                {
                    return _storeSettingService.GetAllCountries();
                }
                
            }
            var countries = GatewayProviderService.GetAllShipCountries().Where(x => x.CountryCode != "ELSE").Select(x => new Country(x.CountryCode, x.Provinces));

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