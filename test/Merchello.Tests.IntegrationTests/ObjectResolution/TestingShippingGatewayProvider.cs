using System.Collections.Generic;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [GatewayProviderActivation("61D8BC55-5D72-4244-A63B-E942C1D4AB47", "Shipping Provider For Testing", "Shipping Provider For Testing")]
    public class TestingShippingGatewayProvider : ShippingGatewayProviderBase
    {
        public TestingShippingGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProvider gatewayProvider, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProvider, runtimeCacheProvider)
        {
        }

        public override IShippingGatewayMethod CreateShippingGatewayMethod(IGatewayResource gatewayResource, IShipCountry shipCountry,
            string name)
        {
            throw new System.NotImplementedException();
        }

        public override void SaveShippingGatewayMethod(IShippingGatewayMethod shippingGatewayMethod)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<IShippingGatewayMethod> GetAllShippingGatewayMethods(IShipCountry shipCountry)
        {
            throw new System.NotImplementedException();
        }

        public override IEnumerable<IGatewayResource> ListResourcesOffered()
        {
            throw new System.NotImplementedException();
        }
    }
}