using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Taxation;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Taxation
{
    [TestFixture]
    [Category("Taxation")]
    public class TaxationProviderTests : TaxationProviderTestBase
    {
        /// <summary>
        /// Test verifies that a list of all taxation providers can be retrieved.
        /// </summary>
        [Test]
        public void Can_Retrieve_A_List_Of_All_Taxation_Providers()
        {
            //// Arrange
            var expected = 1;

            //// Act
            var providers = GatewayProviderService.GetGatewayProvidersByType(GatewayProviderType.Taxation);

            //// Assert
            Assert.IsTrue(providers.Any());
            Assert.IsNull(providers.FirstOrDefault(x => x.GatewayProviderType != GatewayProviderType.Taxation));
            Assert.AreEqual(expected, providers.Count());
            Console.WriteLine("Provider name: {0}", providers.First().Name);
        }

        ///// <summary>
        ///// Test verifies that a list of all taxation providers can be retrieved from the GatewayContext
        ///// </summary>
        //[Test]
        //public void Can_Retrieve_A_List_Of_All_TaxationProviders_From_The_GatewayContext()
        //{
        //    //// Arrange
        //    const GatewayProviderType gatewayProviderType = GatewayProviderType.Taxation;

        //    //// Act
        //    var providers = MerchelloContext.Gateways.GetGatewayProviders(gatewayProviderType);

        //    //// Assert
        //    Assert.NotNull(providers);
        //    Assert.IsTrue(providers.Any());
        //}

        ///// <summary>
        ///// Test verifies that a TaxationGateway class can be instantiated from a IGatewayProvider reference from the GatewayContext
        ///// </summary>
        //[Test]
        //public void Can_Instantiate_A_ShippingProvider_From_The_GatewayContext()
        //{
        //    //// Arrange
        //    const GatewayProviderType gatewayProviderType = GatewayProviderType.Taxation;
        //    var provider = MerchelloContext.ShippingGateways.GetGatewayProviders(gatewayProviderType).FirstOrDefault();
        //    Assert.NotNull(provider);

        //    //// Act
        //    var taxationProvider = ((ProviderTypedGatewayContextBase<>)MerchelloContext.ShippingGateways).ResolveByGatewayProvider<TaxationGatewayProviderBase>(provider);

        //    //// Assert
        //    Assert.NotNull(taxationProvider);
        //    Assert.AreEqual(typeof(CountryTaxRateTaxationGatewayProvider).Name, taxationProvider.GetType().Name);
        //}
    }
}