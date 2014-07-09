namespace Merchello.Plugin.Taxation.Avalara.Provider
{
    using System.Collections.Generic;
    using Core.Gateways;
    using Core.Gateways.Taxation;
    using Core.Models;
    using Core.Services;
    using Umbraco.Core.Cache;

    /// <summary>
    /// The Avalara taxation gateway provider.
    /// </summary>
    [GatewayProviderActivation("DBC48C38-0617-44EA-989A-18AAD8D5DE52", "Avalara Sales Tax Provider", "Avalara Sales Tax Provider")]
    [GatewayProviderEditor("Avalara Taxation Provider Configuration", "~/App_Plugins/Merchello.Avalara/Dialogs/avalara.provider.configuration.html")]
    public class AvalaraTaxationGatewayProvider : TaxationGatewayProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvalaraTaxationGatewayProvider"/> class.
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
        public AvalaraTaxationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
        {
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            throw new System.NotImplementedException();
        }

        public override ITaxationGatewayMethod CreateTaxMethod(string countryCode, decimal taxPercentageRate)
        {
            throw new System.NotImplementedException();
        }

        public override ITaxationGatewayMethod GetGatewayTaxMethodByCountryCode(string countryCode)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<ITaxationGatewayMethod> GetAllGatewayTaxMethods()
        {
            throw new System.NotImplementedException();
        }
    }
}