using System;
using System.Collections.Generic;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Core.Gateways.Taxation.CountryTaxRate
{
    /// <summary>
    /// Represents the CountryTaxRateTaxationGatewayProvider.  
    /// </summary>
    /// <remarks>
    /// 
    /// This is Merchello's default TaxationGatewayProvider
    /// 
    /// </remarks>
    public class CountryTaxRateTaxationGatewayProvider : TaxationGatewayProviderBase, ICountryTaxRateTaxationGatewayProvider
    {
        public CountryTaxRateTaxationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        public ICountryTaxRate CreateCountryTaxRate(string countryCode)
        {
            return CreateCountryTaxRate(countryCode, 0);
        }

        /// <summary>
        /// Attempts to create a <see cref="ICountryTaxRate"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        public ICountryTaxRate CreateCountryTaxRate(string countryCode, decimal percentageTaxRate)
        {
            var attempt = GatewayProviderService.CreateCountryTaxRateWithKey(GatewayProvider.Key, countryCode, percentageTaxRate);

            if (!attempt.Success) throw attempt.Exception;

            return attempt.Result;
        }


        /// <summary>
        /// Gets a <see cref="ICountryTaxRate"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ICountryTaxRate"/></returns>
        public ICountryTaxRate GetCountryTaxRateByCountryCode(string countryCode)
        {
            return GatewayProviderService.GetCountryTaxRateByCountryCode(GatewayProvider.Key, countryCode);
        }

        /// <summary>
        /// Gets a collection of all <see cref="ICountryTaxRate"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ICountryTaxRate"/> </returns>
        public IEnumerable<ICountryTaxRate> GetAllCountryTaxRates()
        {
            return GatewayProviderService.GetCountryTaxRatesByProviderKey(GatewayProvider.Key);
        }

        public override IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice)
        {
            throw new NotImplementedException();
        }


        public override string Name
        {
            get { return "Fixed Rate Tax Provider"; }
        }

        public override Guid Key
        {
            get { return Constants.ProviderKeys.Taxation.CountryTaxRateTaxationProviderKey; }
        }
    }
}