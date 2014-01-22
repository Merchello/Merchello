using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways.Shipping.RateTable;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Shipping
{
    [TestFixture]
    [Category("Shipping")]
    public class RateTableShippingProviderTests : ShippingProviderTestBase
    {
        private IShipCountry _shipCountry;

        [SetUp]
        public void Init()
        {
            _shipCountry = ShippingService.GetShipCountryByCountryCode(Catalog.Key, "US");
        }

        /// <summary>
        /// Test verifies that a GatewayShipMethod can be created 
        /// </summary>
        [Test]
        public void Can_Create_And_Persist_A_ShipMethod_With_UpdatedProvideData()
        {
            //// Arrange            
            // Get the RateTableShippingProvider
            var key = Constants.ProviderKeys.Shipping.RateTableShippingProviderKey;
            var rateTableProvider = MerchelloContext.Gateways.ResolveByKey<RateTableShippingGatewayProvider>(key);
            const decimal expected = 5M;

            //// Act
            var gwShipMethod = rateTableProvider.CreateShipMethod(RateTableShipMethod.QuoteType.PercentTotal, _shipCountry, "Vary By Weight - Ground");
            gwShipMethod.ShipMethod.Provinces["WA"].RateAdjustmentType = RateAdjustmentType.Numeric;
            gwShipMethod.ShipMethod.Provinces["WA"].RateAdjustment = expected;
            rateTableProvider.SaveShipMethod(gwShipMethod);

            var retrieved = rateTableProvider.ActiveShipMethods(_shipCountry).FirstOrDefault();

            //// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual(expected, retrieved.ShipMethod.Provinces["WA"].RateAdjustment);
        }
    }
}