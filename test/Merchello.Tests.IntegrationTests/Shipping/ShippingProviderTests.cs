using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Services;
using NUnit.Framework;
using Umbraco.Core.Cache;

namespace Merchello.Tests.IntegrationTests.Shipping
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
            Assert.IsNull(providers.FirstOrDefault(x => x.GatewayProviderType != GatewayProviderType.Shipping));
            Assert.AreEqual(expected, providers.Count());
            Console.WriteLine("Provider name: {0}", providers.First().Name);
        }


        ///// <summary>
        ///// Test verifies that a ShippingGateway class can be instantiated from a IGatewayProvider reference
        ///// </summary>
        //[Test]
        //public void Can_Instantiate_A_ShippingProvider_From_A_GatewayProvider()
        //{
        //    //// Arrange
        //    var provider = GatewayProviderService.GetGatewayProvidersByType(GatewayProviderType.Shipping).FirstOrDefault();
        //    Assert.NotNull(provider);

        //    //// Act
        //    var ctorArgs = new[] { typeof(IGatewayProviderService), typeof(IGatewayProviderSetting), typeof(IRuntimeCacheProvider) };
        //    var ctoArgValues = new object[] { GatewayProviderService, provider, MerchelloContext.Cache.RuntimeCache };
        //    var gateway = ActivatorHelper.CreateInstance<GatewayProviderBase>(Type.GetType(provider.TypeFullName), ctorArgs, ctoArgValues);

        //    //// Assert
        //    Assert.NotNull(gateway);
        //    Assert.IsTrue(GatewayProviderType.Shipping == gateway.GatewayProviderSetting.GatewayProviderType);
        //}

        /// <summary>
        /// Test verifies that a list of all active shipping providers can be retrieved from the ShippingGatewayContext
        /// </summary>
        [Test]
        public void Can_Instantiate_Active_ShippingProviders_From_The_ShippingGatewayContext()
        {
            //// Arrange
            
            //// Act
            var providers = MerchelloContext.Gateways.Shipping.GetAllActivatedProviders();

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

            //// Act
            var provider = MerchelloContext.Gateways.Shipping.GetAllActivatedProviders().FirstOrDefault();
            


            //// Assert
            Assert.NotNull(provider);
            Assert.NotNull(provider);
        }
        
        /// <summary>
        /// Test verifies that a provider can be associated with a ShipCountry
        /// </summary>
        [Test]
        public void Can_Add_A_Shipmethod_To_A_Provider_With_A_ShipCountry()
        {
            //// Arrange
            var country = ShipCountryService.GetShipCountryByCountryCode(Catalog.Key, "US");
            var shippingProvider = MerchelloContext.Gateways.Shipping.GetAllActivatedProviders().FirstOrDefault() as ShippingGatewayProviderBase;
            Assert.NotNull(shippingProvider);
            
            //// Act
            var resource = shippingProvider.ListResourcesOffered().FirstOrDefault();
            var gatewayShipMethod = shippingProvider.CreateShippingGatewayMethod(resource, country, "Ground");
            shippingProvider.SaveShippingGatewayMethod(gatewayShipMethod);

            //// Assert
            Assert.IsTrue(gatewayShipMethod.ShipMethod.HasIdentity);

        }


    }
}