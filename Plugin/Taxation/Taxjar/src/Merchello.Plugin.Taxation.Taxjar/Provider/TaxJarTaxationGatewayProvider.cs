namespace Merchello.Plugin.Taxation.Taxjar.Provider
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;

    using Core.Gateways;
    using Core.Gateways.Taxation;
    using Core.Models;
    using Core.Services;

    using Umbraco.Core;
    using Umbraco.Core.Cache;
    using Umbraco.Core.Logging;

    /// <summary>
    /// The TaxJar taxation gateway provider.
    /// </summary>
    [GatewayProviderActivation("ddb4b1e5-75ec-4c3f-9a05-a51dbad34b4a", "Taxjar Provider", "Taxjar Provider")]
    [GatewayProviderEditor("Taxjar Taxation Provider Configuration", "~/App_Plugins/Merchello.Taxjar/Dialogs/taxjar.provider.configuration.html")]
    public class TaxJarTaxationGatewayProvider : TaxationGatewayProviderBase, ITaxJarTaxationGatewayProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TaxjarTaxationGatewayProvider"/> class.
        /// </summary>
        /// <param name="gatewayProviderService">
        /// The gateway provider service.
        /// </param>
        /// <param name="gatewayProviderSettings">
        /// The gateway provider settings.
        /// </param>
        /// <param name="runtimeCacheProvider">
        /// The runtime cache provider.
        /// </param>
        public TaxJarTaxationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        /// <summary>
        /// Gets a collection of available <see cref="IGatewayResource"/>.
        /// </summary>
        /// <returns>
        /// The collection of <see cref="IGatewayResource"/>
        /// </returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            var countryCodes = GatewayProviderService.GetAllShipCountries().Select(x => x.CountryCode).Distinct();

            var resources =
                countryCodes.Select(x => new GatewayResource(x, x + "-TaxJar"))
                    .Where(code => TaxMethods.FirstOrDefault(x => x.CountryCode.Equals(code.ServiceCode)) == null);

            return resources;
        }

        /// <summary>
        /// Creates a TaxJar tax method
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <param name="taxPercentageRate">
        /// The tax percentage rate.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxationGatewayMethod"/>.
        /// </returns>
        /// <exception cref="Exception">
        /// Throws an exception if tax rate creation fails
        /// </exception>
        public override ITaxationGatewayMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate)
        {
            var attempt = ListResourcesOffered().FirstOrDefault(x => x.ServiceCode.Equals(countryCode)) != null
                ? GatewayProviderService.CreateTaxMethodWithKey(GatewayProviderSettings.Key, countryCode, taxPercentageRate)
                : Attempt<ITaxMethod>.Fail(new ConstraintException("A TaxJar tax method has already been defined for " + countryCode));


            if (attempt.Success)
            {
                return new TaxJarTaxationGatewayMethod(attempt.Result, ExtendedData);
            }

            LogHelper.Error<TaxationGatewayProviderBase>("CreateTaxMethod failed.", attempt.Exception);

            throw attempt.Exception;
        }

        /// <summary>
        /// Gets the TaxJar gateway tax method by country code.
        /// </summary>
        /// <param name="countryCode">
        /// The country code.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxationGatewayMethod"/>.
        /// </returns>
        public override ITaxationGatewayMethod GetGatewayTaxMethodByCountryCode(string countryCode)
        {
            var taxMethod = TaxMethods.FirstOrDefault(x => x.CountryCode == countryCode);

            return taxMethod != null ? new TaxJarTaxationGatewayMethod(taxMethod, ExtendedData) : null;
        }

        /// <summary>
        /// Gets all gateway tax methods associated with the <see cref="TaxJarTaxationGatewayProvider"/>
        /// </summary>
        /// <returns>
        /// The collection of all TaxJar tax methods.
        /// </returns>
        public override IEnumerable<ITaxationGatewayMethod> GetAllGatewayTaxMethods()
        {
            return TaxMethods.Select(taxMethod => new TaxJarTaxationGatewayMethod(taxMethod, ExtendedData));
        }

        /// <summary>
        /// Returns an <see cref="ITaxJarTaxationGatewayMethod"/> given the <see cref="ITaxMethod"/> (settings)
        /// </summary>
        /// <param name="taxMethod">
        /// The tax method.
        /// </param>
        /// <returns>
        /// The <see cref="ITaxJarTaxationGatewayMethod"/>.
        /// </returns>
        public ITaxJarTaxationGatewayMethod GetTaxJarTaxationGatewayMethod(ITaxMethod taxMethod)
        {
            return new TaxJarTaxationGatewayMethod(taxMethod, ExtendedData);
        }
    }
}