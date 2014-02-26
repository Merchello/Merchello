using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;

namespace Merchello.Core.Gateways.Taxation.FixedRate
{
    /// <summary>
    /// Represents the CountryTaxRateTaxationGatewayProvider.  
    /// </summary>
    /// <remarks>
    /// 
    /// This is Merchello's default TaxationGatewayProvider
    /// 
    /// </remarks>
    public class FixedRateTaxationGatewayProvider : TaxationGatewayProviderBase, IFixedRateTaxationGatewayProvider
    {
        public FixedRateTaxationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        { }

        /// <summary>
        /// Creates a <see cref="ITaxationGatewayMethod"/>
        /// </summary>
        /// <param name="countryCode">The two letter ISO Country Code</param>
        /// <param name="taxPercentageRate">The decimal percentage tax rate</param>
        /// <returns>The <see cref="ITaxationGatewayMethod"/></returns>
        public override ITaxationGatewayMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate)
        {
            var attempt = ListResourcesOffered().FirstOrDefault(x => x.ServiceCode.Equals(countryCode)) != null
                ? GatewayProviderService.CreateTaxMethodWithKey(GatewayProvider.Key, countryCode, taxPercentageRate)
                : Attempt<ITaxMethod>.Fail(new InvalidOperationException("A fixed tax rate method has already been defined for " + countryCode));


            if (!attempt.Success)
            {
                LogHelper.Error<TaxationGatewayProviderBase>("CreateTaxMethod failed.", attempt.Exception);
                throw attempt.Exception;
            }

            return new FixRateMethod(attempt.Result);
        }

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ITaxMethod"/></returns>
        public override ITaxationGatewayMethod GetGatewayTaxMethodByCountryCode(string countryCode)
        {
            var taxMethod = TaxMethods.FirstOrDefault(x => x.CountryCode == countryCode);

            return taxMethod != null ? new FixRateMethod(taxMethod) : null;
        }

        /// <summary>
        /// Gets a collection of all <see cref="ITaxMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxMethod"/> </returns>
        public override IEnumerable<ITaxationGatewayMethod> GetAllGatewayTaxMethods()
        {
            return TaxMethods.Select(taxMethod => new FixRateMethod(taxMethod));
        }

        /// <summary>
        /// Returns a collection of all possible gateway methods associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="IGatewayResource"/></returns>
        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            var countryCodes = GatewayProviderService.GetAllShipCountries().Select(x => x.CountryCode).Distinct();

            return
                countryCodes.Select(x => new GatewayResource(x, x + "-FixedRate"))
                    .Where(code => TaxMethods.FirstOrDefault(x => x.CountryCode.Equals(code.ServiceCode)) == null);

        }

        /// <summary>
        /// The name of the TaxationProvider
        /// </summary>
        public override string Name
        {
            get { return "Fixed Rate Tax Provider"; }
        }

        /// <summary>
        /// The fixed pk associated with the TaxationProvider
        /// </summary>
        public override Guid Key
        {
            get { return Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey; }
        }
    }
}