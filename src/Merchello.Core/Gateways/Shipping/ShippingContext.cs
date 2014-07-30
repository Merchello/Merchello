namespace Merchello.Core.Gateways.Shipping
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Merchello.Core.Models;
    using Merchello.Core.Services;

    /// <summary>
    /// Represents the ShippingContext
    /// </summary>
    internal class ShippingContext : GatewayProviderTypedContextBase<ShippingGatewayProviderBase>, IShippingContext
    {
        /// <summary>
        /// The store setting service.
        /// </summary>
        private readonly IStoreSettingService _storeSettingService;

        /// <summary>
        /// Initializes a new instance of the <see cref="ShippingContext"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="storeSettingService">
        /// The store setting service.
        /// </param>
        /// <param name="resolver">
        /// The resolver.
        /// </param>
        public ShippingContext(IGatewayProviderService gatewayProviderService, IStoreSettingService storeSettingService, IGatewayProviderResolver resolver)
            : base(gatewayProviderService, resolver)
        {
            Mandate.ParameterNotNull(storeSettingService, "storeSettingService");

            _storeSettingService = storeSettingService;
        }

        /// <summary>
        /// Returns an instance of an 'active' GatewayProvider associated with a GatewayMethod based given the unique Key (GUID) of the GatewayMethod
        /// </summary>
        /// <param name="gatewayMethodKey">The unique key (GUID) of the <see cref="IGatewayMethod"/></param>
        /// <returns>An instantiated GatewayProvider</returns>
        public override ShippingGatewayProviderBase GetProviderByMethodKey(Guid gatewayMethodKey)
        {
            return GetAllActivatedProviders()
                .FirstOrDefault(x => ((ShippingGatewayProviderBase)x)
                    .ShipMethods.Any(y => y.Key == gatewayMethodKey)) as ShippingGatewayProviderBase;
        }

        /// <summary>
        /// Returns a collection of all <see cref="IShipmentRateQuote"/> that are available for the <see cref="IShipment"/>
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment"/> to quote</param>
        /// <returns>A collection of <see cref="IShipmentRateQuote"/></returns>
        public IEnumerable<IShipmentRateQuote> GetShipRateQuotesForShipment(IShipment shipment)
        {
            var providers = GatewayProviderResolver.GetActivatedProviders<ShippingGatewayProviderBase>() as IEnumerable<ShippingGatewayProviderBase>;
            var quotes = new List<IShipmentRateQuote>();

            if (providers == null) return quotes;

            foreach (var provider in providers)
            {
                quotes.AddRange(provider.QuoteShippingGatewayMethodsForShipment(shipment));
            }

            return quotes.OrderBy(x => x.Rate);
        }

        /// <summary>
        /// Returns a list of all countries that can be assigned to a shipment
        /// </summary>
        /// <returns>A collection of <see cref="ICountry"/></returns>
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
        /// <param name="shipCountry">
        /// The ship Country.
        /// </param>
        /// <returns>
        /// A collection of all active shipping gateway providers
        /// </returns>
        public IEnumerable<IShippingGatewayProvider> GetGatewayProvidersByShipCountry(IShipCountry shipCountry)
        {
            var gatewayProviders = GatewayProviderService.GetGatewayProvidersByShipCountry(shipCountry);

            return
                gatewayProviders.Select(
                    provider => GatewayProviderResolver.GetProviderByKey<ShippingGatewayProviderBase>(provider.Key));
        }
    }
}