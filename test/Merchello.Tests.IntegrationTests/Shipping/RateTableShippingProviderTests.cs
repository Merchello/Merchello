using System.Linq;
using Examine;
using Merchello.Core;
using Merchello.Core.Gateways.Shipping.RateTable;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web;
using Merchello.Web.Models;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Shipping
{
    [TestFixture]
    [Category("Shipping")]
    public class RateTableShippingProviderTests : ShippingProviderTestBase
    {
        private IShipCountry _shipCountry;
        private ICustomerBase _customer;
        private IBasket _basket;
        private const int ProductCount = 5;
        private IAddress _destination;

        [SetUp]
        public void Init()
        {
            _destination = new Address()
            {
                Name = "Mindfly Web Design Studio",
                Address1 = "114 W. Magnolia St.  Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US"
            };

            PreTestDataWorker.DeleteAllItemCaches();         
            _customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            _basket = Basket.GetBasket(MerchelloContext, _customer);

            for (var i = 0; i < ProductCount; i++) _basket.AddItem(PreTestDataWorker.MakeExistingProduct());
            ExamineManager.Instance.IndexProviderCollection["MerchelloProductIndexer"].RebuildIndex();  
            _basket.AddItem(PreTestDataWorker.MakeExistingProduct(false));

            Basket.Save(MerchelloContext, _basket);

            _shipCountry = ShippingService.GetShipCountryByCountryCode(Catalog.Key, "US");

        }

        /// <summary>
        /// Test verifies that a GatewayShipMethod can be created 
        /// </summary>
        [Test]
        public void Can_Create_And_Persist_A_GatewayShipMethod_With_UpdatedProvideData()
        {
            //// Arrange            
            // Get the RateTableShippingProvider
            var key = Constants.ProviderKeys.Shipping.RateTableShippingProviderKey;
            var rateTableProvider = MerchelloContext.Gateways.ResolveByKey<RateTableShippingGatewayProvider>(key);
            const decimal expected = 5M;
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);

            //// Act
            var gwShipMethod = rateTableProvider.CreateShipMethod(RateTableShipMethod.QuoteType.VaryByWeight, _shipCountry, "Vary By Weight - Ground");
            gwShipMethod.ShipMethod.Provinces["WA"].RateAdjustmentType = RateAdjustmentType.Numeric;
            gwShipMethod.ShipMethod.Provinces["WA"].RateAdjustment = expected;
            rateTableProvider.SaveShipMethod(gwShipMethod);

            var retrieved = rateTableProvider.GetActiveShipMethods(_shipCountry).FirstOrDefault();

            //// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual(expected, retrieved.ShipMethod.Provinces["WA"].RateAdjustment);
        }

        /// <summary>
        /// Test verifies that a gateway ship method can be created with a ratetable and persisted to the database
        /// </summary>
        [Test]
        public void Can_Create_And_Persist_A_GatewayShipMethod_With_A_RateTable()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.RateTableShippingProviderKey;
            var rateTableProvider = MerchelloContext.Gateways.ResolveByKey<RateTableShippingGatewayProvider>(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var expected = 4;

            //// Act
            var gwshipMethod = (RateTableShipMethod)rateTableProvider.CreateShipMethod(RateTableShipMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10);
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 10000, 100);

            
            // have to call this via the static method due o the MerchelloContext.Current not present in the ShipRateTable object.
            ShipRateTable.Save(GatewayProviderService, MerchelloContext.Cache.RuntimeCache, gwshipMethod.RateTable);

            var retrieved = (RateTableShipMethod)rateTableProvider.GetActiveShipMethods(_shipCountry).First();

            ////// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual("Ground (VBW)", retrieved.ShipMethod.Name);
            Assert.AreEqual(expected, retrieved.RateTable.Rows.Count());
        }

        //[Test]
        //public void Can_Get_A_Quote_For_A_Shipment()
        //{
        //    //// Arrange
        //    var key = Constants.ProviderKeys.Shipping.RateTableShippingProviderKey;
        //    var rateTableProvider = MerchelloContext.Gateways.ResolveByKey<RateTableShippingGatewayProvider>(key);
        //    rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
        //    var gwshipMethod = (RateTableShipMethod)rateTableProvider.CreateShipMethod(RateTableShipMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
        //    gwshipMethod.RateTable.AddRow(0, 10, 5);
        //    gwshipMethod.RateTable.AddRow(10, 15, 10);
        //    gwshipMethod.RateTable.AddRow(15, 25, 25);
        //    gwshipMethod.RateTable.AddRow(25, 10000, 100);
        //    ShipRateTable.Save(GatewayProviderService, MerchelloContext.Cache.RuntimeCache, gwshipMethod.RateTable);

        //    //// Act
        //    var shipments = _basket.PackageBasket(MerchelloContext, _destination);

        //    var attempt = gwshipMethod.QuoteShipment(shipments.First());

        //    //// Assert
        //    Assert.IsTrue(attempt.Success);
        //}
    }
}