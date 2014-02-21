using System;
using System.Collections.Generic;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Taxation.FlatRate
{
    /// <summary>
    /// Represents the CountryTaxRateTaxationGatewayProvider.  
    /// </summary>
    /// <remarks>
    /// 
    /// This is Merchello's default TaxationGatewayProvider
    /// 
    /// </remarks>
    public class FlatRateTaxationGatewayProvider : TaxationGatewayProviderBase, IFlatRateTaxationGatewayProvider
    {
        public FlatRateTaxationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        public ITaxMethod CreateTaxMethod(string countryCode)
        {
            return CreateTaxMethod(countryCode, 0);
        }

        /// <summary>
        /// Attempts to create a <see cref="ITaxMethod"/> for a given provider and country.  If the provider already 
        /// defines a tax rate for the country, the creation fails.
        /// </summary>
        /// <param name="countryCode">The two character ISO country code</param>
        /// <param name="percentageTaxRate">The tax rate in percentage for the country</param>
        public ITaxMethod CreateTaxMethod(string countryCode, decimal percentageTaxRate)
        {
            var attempt = GatewayProviderService.CreateTaxMethodWithKey(GatewayProvider.Key, countryCode, percentageTaxRate);

            if (!attempt.Success) throw attempt.Exception;

            return attempt.Result;
        }

        /// <summary>
        /// Saves a single instance of a <see cref="ITaxMethod"/>
        /// </summary>
        /// <param name="taxMethod">The <see cref="ITaxMethod"/> to save</param>
        public void SaveTaxMethod(ITaxMethod taxMethod)
        {
            GatewayProviderService.Save(taxMethod);
        }

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ITaxMethod"/></returns>
        public ITaxMethod GetTaxMethodByCountryCode(string countryCode)
        {
            return GatewayProviderService.GetTaxMethodByCountryCode(GatewayProvider.Key, countryCode);
        }

        /// <summary>
        /// Gets a collection of all <see cref="ITaxMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxMethod"/> </returns>
        public IEnumerable<ITaxMethod> GetAllTaxMethods()
        {
            return GatewayProviderService.GetTaxMethodsByProviderKey(GatewayProvider.Key);
        }

        /// <summary>
        /// Calculates the tax amount for an invoice
        /// </summary>
        /// <param name="invoice"><see cref="IInvoice"/></param>
        /// <param name="taxAddress">The <see cref="IAddress"/> to base taxation rates.  Either origin or destination address.</param>
        /// <returns><see cref="IInvoiceTaxResult"/></returns>
        public override IInvoiceTaxResult CalculateTaxForInvoice(IInvoice invoice, IAddress taxAddress)
        {
            var countryTaxRate = GetTaxMethodByCountryCode(taxAddress.CountryCode);
            if (countryTaxRate == null) return null;

            var ctrValues = new object[] { invoice, taxAddress, countryTaxRate };

            var typeName = MerchelloConfiguration.Current.GetStrategyElement(Constants.StrategyTypeAlias.DefaultInvoiceTaxRateQuote).Type;

            var attempt = ActivatorHelper.CreateInstance<InvoiceTaxationStrategyBase>(typeName, ctrValues);

            if (!attempt.Success)
            {
                LogHelper.Error<FlatRateTaxationGatewayProvider>("Failed to instantiate the tax rate quote strategy '" + typeName +"'", attempt.Exception);
                throw attempt.Exception;
            }

            return CalculateTaxForInvoice(attempt.Result);
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