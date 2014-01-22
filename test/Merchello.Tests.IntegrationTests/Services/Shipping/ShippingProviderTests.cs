using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping.RateTable;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using NUnit.Framework;
using Umbraco.Core.Cache;

namespace Merchello.Tests.IntegrationTests.Services.Shipping
{

    [TestFixture]
    [Category("Shipping")]
    public class ShippingProviderTests : ShippingProviderTestBase
    {
              
        [SetUp]
        public void Init()
        {            
            
            
        }

        /// <summary>
        /// Test verifies that a list of all shipping providers can be retrieved.
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_All_Shipping_Providers()
        {
            //// Arrange
            var expected = 1;

            //// Act
            var providers = GatewayProviderService.GetGatewayProvidersByType(GatewayProviderType.Shipping);

            //// Assert
            Assert.IsTrue(providers.Any());
            Console.WriteLine("Provider name: {0}", providers.First().Name);
        }


        /// <summary>
        /// Test verifies that a ShippingGateway class can be instantiated from a IGatewayProvider reference
        /// </summary>
        [Test]
        public void Can_Instantiate_A_ShippingProvider_From_A_GatewayProvider()
        {
            //// Arrange
            var provider = GatewayProviderService.GetGatewayProvidersByType(GatewayProviderType.Shipping).FirstOrDefault();
            Assert.NotNull(provider);

            //// Act
            var ctorArgs = new[] { typeof(IGatewayProviderService), typeof(IGatewayProvider), typeof(IRuntimeCacheProvider) };
            var ctoArgValues = new object[] { GatewayProviderService, provider, MerchelloContext.Cache.RuntimeCache };
            var gateway = ActivatorHelper.CreateInstance<GatewayProviderBase>(Type.GetType(provider.TypeFullName), ctorArgs, ctoArgValues);

            //// Assert
            Assert.NotNull(gateway);
            Assert.IsTrue(GatewayProviderType.Shipping == gateway.GatewayProvider.GatewayProviderType);
        }

        /// <summary>
        /// Test verifies that a list of all shipping providers can be retrieved from the GatewayContext
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_All_ShippingProviders_From_The_GatewayContext()
        {
            //// Arrange
            const GatewayProviderType gatewayProviderType = GatewayProviderType.Shipping;

            //// Act
            var providers = MerchelloContext.Gateways.GetGatewayProviders(gatewayProviderType);

            //// Assert
            Assert.NotNull(providers);
            Assert.IsTrue(providers.Any());
        }

        /// <summary>
        /// Test verifies that a ShippingGateway class can be instantiated from a IGatewayProvider reference from the GatewayContext
        /// </summary>
        [Test]
        public void Can_Instantiate_A_ShippingProvider_From_The_GatewayContext()
        {
            //// Arrange
            const GatewayProviderType gatewayProviderType = GatewayProviderType.Shipping;
            var provider = MerchelloContext.Gateways.GetGatewayProviders(gatewayProviderType).FirstOrDefault();
            Assert.NotNull(provider);

            //// Act
            var shippingProvider = MerchelloContext.Gateways.ResolveByGatewayProvider<RateTableShippingGatewayProvider>(provider);

            //// Assert
            Assert.NotNull(shippingProvider);
        }
        
        /// <summary>
        /// Test verifies that a provider can be associated with a ShipCountry
        /// </summary>
        [Test]
        public void Can_Add_A_Shipmethod_To_A_Provider_With_A_ShipCountry()
        {
            //// Arrange
            var country = ShippingService.GetShipCountryByCountryCode(Catalog.Key, "US");
            var provider = MerchelloContext.Gateways.GetGatewayProviders(GatewayProviderType.Shipping).FirstOrDefault();
            var shippingProvider = MerchelloContext.Gateways.ResolveByGatewayProvider<RateTableShippingGatewayProvider>(provider);
            Assert.NotNull(shippingProvider);
            
            //// Act
            var resource = shippingProvider.ListResourcesOffered().FirstOrDefault();
            var gatewayShipMethod = shippingProvider.CreateShipMethod(resource, country, "Ground");
            shippingProvider.SaveShipMethod(gatewayShipMethod);

            //// Assert
            Assert.IsTrue(gatewayShipMethod.ShipMethod.HasIdentity);

        }

        ///// <summary>
        ///// Test verifies that a constraint violation is thrown if a duplicate shipmethod is added per for a country
        ///// </summary>
        //[Test]
        //public void Attempting_To_Add_A_Duplicate_Provider_ShipMethod_For_ShipCountry_Results_In_A_Error()
        //{
        //    var country = _shippingService.GetShipCountryByCountryCode(_catalog.Key, "US");
        //    var provider = _merchelloContext.Gateways.GetGatewayProviders(GatewayProviderType.Shipping).FirstOrDefault();
        //    var shippingProvider = _merchelloContext.Gateways.GetShippingGatewayProvider(provider);
        //    Assert.NotNull(shippingProvider);
        //    var resource = shippingProvider.ListAvailableMethods().FirstOrDefault();
        //    var gatewayShipMethod = shippingProvider.CreateShipMethod(resource, country, "Ground");
        //    shippingProvider.SaveShipMethod(gatewayShipMethod);

        //    //// Act

        //    //// Assert
        //    Assert.Throws<ConstraintException>(() => shippingProvider.CreateShipMethod(resource, country, "Ground"));

        //}

    }
}