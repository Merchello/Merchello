using System.Collections.Generic;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Umbraco.Core.Cache;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [GatewayProviderActivation("61D8BC55-5D72-4244-A63B-E942C1D4AB47", "Shipping Provider For Testing", "Shipping Provider for Object Resolution Testing")]
    public class TestingShippingGatewayProvider : ShippingGatewayProviderBase
    {
        private static IEnumerable<IGatewayResource> AvailableResources
        {
            get
            {
                return new List<IGatewayResource>()
                {
                    new GatewayResource("ShipTest1", "Ship Test 1"),
                    new GatewayResource("ShipTest2", "Ship Test 2"),
                    new GatewayResource("ShipTest3", "Ship Test 3"),
                    new GatewayResource("ShipTest4", "Ship Test 4")
                };
            }
        }

        public TestingShippingGatewayProvider(IGatewayProviderService gatewayProviderService, IGatewayProviderSettings gatewayProviderSettings, IRuntimeCacheProvider runtimeCacheProvider) 
            : base(gatewayProviderService, gatewayProviderSettings, runtimeCacheProvider)
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
            return AvailableResources;
        }
    }
}