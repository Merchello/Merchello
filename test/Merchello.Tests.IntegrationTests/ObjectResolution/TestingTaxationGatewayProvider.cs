using System.Collections.Generic;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [GatewayProviderActivation("518B5FDF-C414-4309-99D5-E61028311A2F", "Taxation Provider For Testing", "Taxation Provider  for Object Resolution Testing")]
    public class TestingTaxationGatewayProvider : TaxationGatewayProviderBase
    {
        public TestingTaxationGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
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