using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Web;
using Merchello.Web.Workflow;
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
        private const int ProductCount = 3;
        private IAddress _destination;
        private const decimal WeightPerProduct = 2M;
        private const decimal PricePerProduct = 10M;

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
            _basket = Basket.GetBasket(MerchelloContext.Current, _customer);

            for (var i = 0; i < ProductCount; i++) _basket.AddItem(PreTestDataWorker.MakeExistingProduct(true, WeightPerProduct, PricePerProduct));



            Basket.Save(MerchelloContext.Current, _basket);

            _shipCountry = ShipCountryService.GetShipCountryByCountryCode(Catalog.Key, "US");

        }

        /// <summary>
        /// Test verifies that a GatewayShipMethod can be created 
        /// </summary>
        [Test]
        public void Can_Create_And_Persist_A_GatewayShipMethod_With_UpdatedProvideData()
        {
            //// Arrange            
            // Get the RateTableShippingProvider
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            const decimal expected = 5M;
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);

            //// Act
            var gwShipMethod = rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Vary By Weight - Ground");
            gwShipMethod.ShipMethod.Provinces["WA"].RateAdjustmentType = RateAdjustmentType.Numeric;
            gwShipMethod.ShipMethod.Provinces["WA"].RateAdjustment = expected;
            rateTableProvider.SaveShippingGatewayMethod(gwShipMethod);

            var retrieved = rateTableProvider.GetAllShippingGatewayMethods(_shipCountry).FirstOrDefault();

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
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var expected = 4;

            //// Act
            var gwshipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10);
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 10000, 100);

            // have to call this via the static method due o the MerchelloContext.Current not present in the ShipRateTable object.
            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable);

            var retrieved = (FixedRateShippingGatewayMethod)rateTableProvider.GetAllShippingGatewayMethods(_shipCountry).First();

            ////// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual("Ground (VBW)", retrieved.ShipMethod.Name);
            Assert.AreEqual(expected, retrieved.RateTable.Rows.Count());
        }



        /// <summary>
        /// Can quote a shipment with a VaryByRate FixedRateTable
        /// </summary>
        [Test]
        public void Can_Get_A_Quote_For_A_Shipment_VaryByWeight()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10); // total weight should be 10M so we should hit this tier
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 10000, 100);

            var expectedRate = 5M;

            //// Act
            var shipments = _basket.PackageBasket(MerchelloContext.Current, _destination);

            var attempt = gwshipMethod.QuoteShipment(shipments.First());            

            //// Assert
            Assert.IsTrue(attempt.Success);
            Assert.AreEqual(expectedRate, attempt.Result.Rate);
        }

        /// <summary>
        /// Can quote a shipment with a VaryByPrice FixedRateTable
        /// </summary>
        [Test]
        public void Can_Get_A_Quote_For_A_Shipment_VaryByPrice()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, _shipCountry, "Ground (Vary By Price)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10); 
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 60, 30); // total price should be 30M so we should hit this tier
            gwshipMethod.RateTable.AddRow(25, 10000, 50);

            var expectedRate = 30M; 

            //// Act
            var shipments = _basket.PackageBasket(MerchelloContext.Current, _destination);

            var attempt = gwshipMethod.QuoteShipment(shipments.First());

            Console.Write("Basket price {0}", _basket.TotalBasketPrice);

            //// Assert
            Assert.IsTrue(attempt.Success);
            Assert.AreEqual(expectedRate, attempt.Result.Rate);
        }

        /// <summary>
        /// GetActiveShipMethods returns a valid list of GatewayShipMethods
        /// </summary>
        [Test]
        public void Can_Return_A_Valid_List_Of_ActiveShipMethods()
        {
            //// Arrange
            var dkCountry = ShipCountryService.GetShipCountryByCountryCode(Catalog.Key, "DK");
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod1 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, _shipCountry, "Ground (PercentTotal) 1");
            var gwshipMethod2 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, _shipCountry, "Ground (PercentTotal) 2");
            var gwshipMethod3 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, dkCountry, "Ground (PercentTotal) 3");

            //// Act
            var shipments = _basket.PackageBasket(MerchelloContext.Current, _destination);
            Assert.IsTrue(shipments.Any());
            var retrievedMethods = rateTableProvider.GetShippingGatewayMethodsForShipment(shipments.First());

            //// Assert
            Assert.IsTrue(retrievedMethods.Any());
            Assert.AreEqual(2, retrievedMethods.Count());
        }

        /// <summary>
        /// Test confirms that valid quotes are returned for all available/active shipmethods for the provider
        /// </summary>
        [Test]
        public void Can_Get_Quotes_For_All_Active_ShipMethods()
        {

            #region BackOffice
            

            //// Arrange
            var dkCountry = ShipCountryService.GetShipCountryByCountryCode(Catalog.Key, "DK");
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;

            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);

            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod1 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, _shipCountry, "Ground (Vary By Pricc) 1");
            gwshipMethod1.RateTable.AddRow(0, 10, 5);
            gwshipMethod1.RateTable.AddRow(10, 15, 10);
            gwshipMethod1.RateTable.AddRow(15, 25, 25);
            gwshipMethod1.RateTable.AddRow(25, 60, 30); // total price should be 50M so we should hit this tier
            gwshipMethod1.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod1);    
            
            var gwshipMethod2 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod2.RateTable.AddRow(0, 10, 5);
            gwshipMethod2.RateTable.AddRow(10, 15, 10); // total weight should be 10M so we should hit this tier
            gwshipMethod2.RateTable.AddRow(15, 25, 25);
            gwshipMethod2.RateTable.AddRow(25, 10000, 100);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod2);

            var gwshipMethod3 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, dkCountry, "Ground (Vary By Price) 3");
            gwshipMethod3.RateTable.AddRow(0, 10, 5);
            gwshipMethod3.RateTable.AddRow(10, 15, 10);
            gwshipMethod3.RateTable.AddRow(15, 25, 25);
            gwshipMethod3.RateTable.AddRow(25, 60, 30); // total price should be 50M so we should hit this tier
            gwshipMethod3.RateTable.AddRow(25, 10000, 50);
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod3);

            #endregion

            // var shipments =  CurrentCustomer.Basket.PackageBasket(_destinationAddress);

            //// Act
            var shipments = _basket.PackageBasket(MerchelloContext.Current, _destination);
            Assert.IsTrue(shipments.Any());

            var quotes = rateTableProvider.QuoteShippingGatewayMethodsForShipment(shipments.First()).OrderBy(x => x.Rate);

            Console.Write("Basket total price: {0}", _basket.TotalBasketPrice);

            //// Assert
            Assert.IsTrue(quotes.Any());
            Assert.AreEqual(2, quotes.Count()); 
            Assert.AreEqual(5M, quotes.First().Rate);
            Assert.AreEqual(30M, quotes.Last().Rate);
        }

        /// <summary>
        /// Test verifies that an adjusted rate is returned for Alaska
        /// </summary>
        [Test]
        public void Can_Get_A_Numerically_Adjusted_Quote_For_Alaska_Region()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);

            var gwshipMethod1 = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByPrice, _shipCountry, "Ground (PercentTotal) 1");
            gwshipMethod1.RateTable.AddRow(0, 10, 5);
            gwshipMethod1.RateTable.AddRow(10, 15, 10);
            gwshipMethod1.RateTable.AddRow(15, 25, 25);
            gwshipMethod1.RateTable.AddRow(25, 60, 30); // total price should be 50M so we should hit this tier
            gwshipMethod1.RateTable.AddRow(25, 10000, 50);
            gwshipMethod1.ShipMethod.Provinces["AK"].RateAdjustment = 5M;
            rateTableProvider.SaveShippingGatewayMethod(gwshipMethod1);
            _destination.Region = "AK";

            //// Act
            var shipments = _basket.PackageBasket(MerchelloContext.Current, _destination);
            Assert.IsTrue(shipments.Any());
            var quotes = MerchelloContext.Current.Gateways.Shipping.GetShipRateQuotesForShipment(shipments.First());
            
            // var invoice = _basket.CheckOut();
            Console.Write("Basket total price: {0}", _basket.TotalBasketPrice);
            //// Assert
            Assert.IsTrue(quotes.Any());
            Assert.AreEqual(1, quotes.Count());
            Assert.AreEqual(35M, quotes.First().Rate);

        }

        /// <summary>
        /// Test verifies that a gateway ship method rate table row can be deleted and the row range values are
        /// adjusted correctly
        /// </summary>
        [Test]
        public void Can_Delete_A_Row_From_An_Existing_RateTable()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10);
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 10000, 100);
            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable);
           

            //// Act
            var row = gwshipMethod.RateTable.Rows.FirstOrDefault(x => x.RangeLow == 15);
            ShippingFixedRateTable.DeleteRow(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable, row);

            var retrieved = (FixedRateShippingGatewayMethod)rateTableProvider.GetAllShippingGatewayMethods(_shipCountry).First();

            ////// Assert
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.RateTable.Rows.FirstOrDefault(x => x.RangeLow == 15));
            Assert.AreEqual(3, retrieved.RateTable.Rows.Count());
        }


        /// <summary>
        /// Test verifies that a gateway ship method rate table row can be inserted at the end of the rate table row range values are
        /// adjusted correctly
        /// </summary>
        [Test]
        public void Can_Insert_A_Row_At_The_End_Of_An_Existing_RateTable()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10);
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 35, 100);
            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable);


            //// Act

            gwshipMethod.RateTable.AddRow(36, 38, 100);
            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable);

            var retrieved = (FixedRateShippingGatewayMethod)rateTableProvider.GetAllShippingGatewayMethods(_shipCountry).First();

            ////// Assert
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.RateTable.Rows.FirstOrDefault(x => x.RangeLow == 35));
            Assert.AreEqual(5, retrieved.RateTable.Rows.Count());
        }

        /// <summary>
        /// Test verifies that a gateway ship method rate table row can be inserted to split of the rate table row range values are
        /// adjusted correctly
        /// </summary>
        [Test]
        public void Can_Insert_A_Row_At_The_Middle_Of_An_Existing_RateTable_Row()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10);
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 35, 100);
            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable);


            //// Act

            gwshipMethod.RateTable.AddRow(18, 22, 100);
            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable);

            var retrieved = (FixedRateShippingGatewayMethod)rateTableProvider.GetAllShippingGatewayMethods(_shipCountry).First();

            ////// Assert
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.RateTable.Rows.FirstOrDefault(x => x.RangeLow == 18));
            Assert.AreEqual(5, retrieved.RateTable.Rows.Count());
        }

        /// <summary>
        /// Test verifies that a gateway ship method rate table row can be inserted to split of the rate table row range values are
        /// adjusted correctly
        /// </summary>
        [Test]
        public void Can_Insert_A_Row_At_The_Middle_Of_An_Existing_RateTable_Row_After_A_Deletion()
        {
            //// Arrange
            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            var rateTableProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);
            rateTableProvider.DeleteAllActiveShipMethods(_shipCountry);
            var gwshipMethod = (FixedRateShippingGatewayMethod)rateTableProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            gwshipMethod.RateTable.AddRow(0, 10, 5);
            gwshipMethod.RateTable.AddRow(10, 15, 10);
            gwshipMethod.RateTable.AddRow(15, 25, 25);
            gwshipMethod.RateTable.AddRow(25, 35, 100);
            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable);


            //// Act
            var row = gwshipMethod.RateTable.Rows.FirstOrDefault(x => x.RangeLow == 15);
            ShippingFixedRateTable.DeleteRow(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, gwshipMethod.RateTable, row);
            
            
            var retrieved = (FixedRateShippingGatewayMethod)rateTableProvider.GetAllShippingGatewayMethods(_shipCountry).First();
            retrieved.RateTable.AddRow(17, 22, 100);

            ShippingFixedRateTable.Save(GatewayProviderService, MerchelloContext.Current.Cache.RuntimeCache, retrieved.RateTable);


            ////// Assert
            Assert.NotNull(retrieved);
            Assert.NotNull(retrieved.RateTable.Rows.FirstOrDefault(x => x.RangeLow == 15));
            Assert.NotNull(retrieved.RateTable.Rows.FirstOrDefault(x => x.RangeHigh == 22));
            Assert.NotNull(retrieved.RateTable.Rows.FirstOrDefault(x => x.RangeLow == 22));
            Assert.AreEqual(4, retrieved.RateTable.Rows.Count());
        }

    }
}