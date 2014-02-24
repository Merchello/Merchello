using System;
using System.Collections.Generic;
using System.Linq;
using Merchello.Core.Configuration;
using Merchello.Core.Models;
using Merchello.Core.Services;
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
        /// Creates a <see cref="IGatewayTaxMethod"/>
        /// </summary>
        /// <param name="countryCode">The two letter ISO Country Code</param>
        /// <param name="taxPercentageRate">The decimal percentage tax rate</param>
        /// <returns>The <see cref="IGatewayTaxMethod"/></returns>
        public override IGatewayTaxMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate)
        {
            var attempt = GatewayProviderService.CreateTaxMethodWithKey(GatewayProvider.Key, countryCode, taxPercentageRate);

            if (!attempt.Success)
            {
                LogHelper.Error<TaxationGatewayProviderBase>("CreateTaxMethod failed.", attempt.Exception);
                throw attempt.Exception;
            }

            return new FixRateTaxMethod(attempt.Result);
        }

        /// <summary>
        /// Gets a <see cref="ITaxMethod"/> by it's unique 'key' (Guid)
        /// </summary>
        /// <param name="countryCode">The two char ISO country code</param>
        /// <returns><see cref="ITaxMethod"/></returns>
        public override IGatewayTaxMethod GetGatewayTaxMethodByCountryCode(string countryCode)
        {
            var taxMethod = TaxMethods.FirstOrDefault(x => x.CountryCode == countryCode);

            return taxMethod != null ? new FixRateTaxMethod(taxMethod) : null;
        }

        /// <summary>
        /// Gets a collection of all <see cref="ITaxMethod"/> associated with this provider
        /// </summary>
        /// <returns>A collection of <see cref="ITaxMethod"/> </returns>
        public override IEnumerable<IGatewayTaxMethod> GetAllGatewayTaxMethods()
        {
            return TaxMethods.Select(taxMethod => new FixRateTaxMethod(taxMethod));
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