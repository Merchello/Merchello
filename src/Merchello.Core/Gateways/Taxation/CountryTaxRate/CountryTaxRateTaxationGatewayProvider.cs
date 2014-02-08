using System;
using System.Collections.Generic;
using Merchello.Core.Configuration;
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
        /// Saves a single instance of a <see cref="ICountryTaxRate"/>
        /// </summary>
        /// <param name="countryTaxRate">The <see cref="ICountryTaxRate"/> to save</param>
        public void SaveCountryTaxRate(ICountryTaxRate countryTaxRate)
        {
            GatewayProviderService.Save(countryTaxRate);
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

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <param name="taxAddress">The <see cref="IAddress"/> to base taxation rates.  Either origin or destination address.</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        public override IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress)
        {
            var countryTaxRate = GetCountryTaxRateByCountryCode(taxAddress.CountryCode);
            if (countryTaxRate == null) return null;

            var ctrArgs = new[] {typeof (IInvoice), typeof (IAddress), typeof (ICountryTaxRate)};
            var ctrValues = new object[] { invoice, taxAddress, countryTaxRate };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultInvoiceTaxRateQuote).Type;

            var strategy = ActivatorHelper.CreateInstance<InvoiceTaxationStrategyBase>(Type.GetType(typeName), ctrArgs, ctrValues);

            return CalculateTaxForInvoice(strategy);
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