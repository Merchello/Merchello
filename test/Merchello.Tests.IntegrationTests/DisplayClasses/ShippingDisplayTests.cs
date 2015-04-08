using System;
using System.Linq;
using System.Collections.Generic;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Shipping.FixedRate;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.Shipping;
using Merchello.Web;
using Merchello.Web.Editors;
using Merchello.Web.Models;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    [TestFixture]
    public class ShippingDisplayTests : ShippingProviderTestBase
    {
        private IWarehouse _warehouse;
        private IWarehouseCatalog _warehouseCatalog;
        private IShipCountry _shipCountry;
        private FixedRateShippingGatewayProvider _fixedRateProvider;
        private FixedRateShippingGatewayMethod _gwshipMethod;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var warehouseService = PreTestDataWorker.WarehouseService;
            _warehouse = warehouseService.GetDefaultWarehouse();

            _warehouseCatalog = _warehouse.DefaultCatalog();

            var key = Constants.ProviderKeys.Shipping.FixedRateShippingProviderKey;
            _fixedRateProvider = (FixedRateShippingGatewayProvider)MerchelloContext.Current.Gateways.Shipping.CreateInstance(key);

            var shipCountryService = PreTestDataWorker.ShipCountryService;
            _shipCountry = shipCountryService.GetShipCountryByCountryCode(_warehouseCatalog.Key, "US");           
        }

        [SetUp]
        public void Init()
        {
            
            _fixedRateProvider.DeleteAllActiveShipMethods(_shipCountry);

            _gwshipMethod = (FixedRateShippingGatewayMethod)_fixedRateProvider.CreateShipMethod(FixedRateShippingGatewayMethod.QuoteType.VaryByWeight, _shipCountry, "Ground (VBW)");
            _gwshipMethod.RateTable.AddRow(0, 10, 5);
            _gwshipMethod.RateTable.AddRow(10, 15, 10);
            _gwshipMethod.RateTable.AddRow(15, 25, 25);
            _gwshipMethod.RateTable.AddRow(25, 10000, 100);

            _fixedRateProvider.SaveShippingGatewayMethod(_gwshipMethod);
    
            if (_gwshipMethod.ShipMethod.Provinces.Any(x => x.AllowShipping == false))
            {
                Assert.Ignore("One of the Provinces was null");
            }
        }


        [Test]
        public void Can_Build_ShipMethodDisplay_From_ShipMethod()
        {
            //// Arrange

            //// Act
            var shipMethod =_gwshipMethod.ShipMethod;
            var shipProvince = shipMethod.Provinces.First();
            shipProvince.RateAdjustmentType = RateAdjustmentType.Percentage;

            var shipMethodDisplay = shipMethod.ToShipMethodDisplay();
            var shipProvinceDisplay = shipMethodDisplay.Provinces.First();

            //// Assert
            Assert.NotNull(shipMethodDisplay);
            Assert.AreEqual(shipMethod.Key, shipMethodDisplay.Key);
            Assert.AreEqual(shipMethod.Name, shipMethodDisplay.Name);
            Assert.AreEqual(shipMethod.ProviderKey, shipMethodDisplay.ProviderKey);
            Assert.AreEqual(shipMethod.Provinces.Count(), shipMethodDisplay.Provinces.Count());
            Assert.AreEqual(shipMethod.ServiceCode, shipMethodDisplay.ServiceCode);
            Assert.AreEqual(shipMethod.ShipCountryKey, shipMethodDisplay.ShipCountryKey);
            Assert.AreEqual(shipMethod.Taxable, shipMethodDisplay.Taxable);
            Assert.AreEqual(shipMethod.Surcharge, shipMethodDisplay.Surcharge);

            Assert.NotNull(shipProvinceDisplay);
            Assert.AreEqual(shipProvince.AllowShipping, shipProvinceDisplay.AllowShipping); // TODO this is the failing mapping
            Assert.AreEqual(shipProvince.Name, shipProvinceDisplay.Name);
            Assert.AreEqual(shipProvince.Code, shipProvinceDisplay.Code);
            Assert.AreEqual(shipProvince.RateAdjustment, shipProvinceDisplay.RateAdjustment);
            Assert.AreEqual(shipProvince.RateAdjustmentType, shipProvinceDisplay.RateAdjustmentType);
        }

        [Test]
        public void Can_Build_ShipMethod_From_ShipMethodDisplay()
        {
            //// Arrange

            //// Act
            var shipMethodDisplay = _gwshipMethod.ShipMethod.ToShipMethodDisplay();
            var shipProvinceDisplay = shipMethodDisplay.Provinces.First();

            shipMethodDisplay.Surcharge = 99M;
            shipProvinceDisplay.RateAdjustment = 99M;

            var shipMethod = shipMethodDisplay.ToShipMethod(_gwshipMethod.ShipMethod);
            var shipProvince = shipMethod.Provinces.First();

            //// Assert
            Assert.NotNull(shipMethod);
            Assert.AreEqual(shipMethod.Key, shipMethodDisplay.Key);
            Assert.AreEqual(shipMethod.Name, shipMethodDisplay.Name);
            Assert.AreEqual(shipMethod.ProviderKey, shipMethodDisplay.ProviderKey);
            Assert.AreEqual(shipMethod.Provinces.Count(), shipMethodDisplay.Provinces.Count());
            Assert.AreEqual(shipMethod.ServiceCode, shipMethodDisplay.ServiceCode);
            Assert.AreEqual(shipMethod.ShipCountryKey, shipMethodDisplay.ShipCountryKey);
            Assert.AreEqual(shipMethod.Taxable, shipMethodDisplay.Taxable);
            Assert.AreEqual(shipMethod.Surcharge, shipMethodDisplay.Surcharge);

            Assert.NotNull(shipProvince);
            Assert.AreEqual(shipProvince.AllowShipping, shipProvinceDisplay.AllowShipping);
            Assert.AreEqual(shipProvince.Name, shipProvinceDisplay.Name);
            Assert.AreEqual(shipProvince.Code, shipProvinceDisplay.Code);
            Assert.AreEqual(shipProvince.RateAdjustment, shipProvinceDisplay.RateAdjustment);
            Assert.AreEqual(shipProvince.RateAdjustmentType, shipProvinceDisplay.RateAdjustmentType);
        }


        [Test]
        public void Can_Build_ShipRateTableDisplay_From_ShipRateTable()
        {
            //// Arrange

            //// Act
            var shipRateTable = _gwshipMethod.RateTable;
            var shipRateTier = _gwshipMethod.RateTable.Rows.First();

            var shipRateTableDisplay = shipRateTable.ToShipFixedRateTableDisplay();
            var shipRateTierDisplay = shipRateTableDisplay.Rows.First();

            //// Assert
            Assert.NotNull(shipRateTableDisplay);
            Assert.AreEqual(shipRateTable.ShipMethodKey, shipRateTableDisplay.ShipMethodKey);
            Assert.AreEqual(shipRateTable.Rows.Count(), shipRateTableDisplay.Rows.Count());

            Assert.NotNull(shipRateTierDisplay);
            Assert.AreEqual(shipRateTier.Key, shipRateTierDisplay.Key);
            Assert.AreEqual(shipRateTier.ShipMethodKey, shipRateTierDisplay.ShipMethodKey);
            Assert.AreEqual(shipRateTier.RangeLow, shipRateTierDisplay.RangeLow);
            Assert.AreEqual(shipRateTier.RangeHigh, shipRateTierDisplay.RangeHigh);
            Assert.AreEqual(shipRateTier.Rate, shipRateTierDisplay.Rate);
        }


        [Test]
        public void Can_Build_ShipRateTable_From_ShipRateTableDisplay()
        {
            //// Arrange

            //// Act
            var shipRateTableDisplay = _gwshipMethod.RateTable.ToShipFixedRateTableDisplay();
            var shipRateTierDisplay = shipRateTableDisplay.Rows.First();

            shipRateTierDisplay.Rate = 15M;
            var rows = shipRateTableDisplay.Rows as List<ShipRateTierDisplay>;
            rows.Add(new ShipRateTierDisplay()
                    {
                        RangeLow = 100M,
                        RangeHigh = 150M,
                        Rate = 99M
                    });

         
            var shipRateTable = shipRateTableDisplay.ToShipRateTable(_gwshipMethod.RateTable);
            // if you want to save it .. it should go here.

            var shipRateTier = _gwshipMethod.RateTable.Rows.First();

            //// Assert
            Assert.NotNull(shipRateTable);
            Assert.AreEqual(shipRateTable.ShipMethodKey, shipRateTableDisplay.ShipMethodKey);
                        
            Assert.AreEqual(shipRateTable.Rows.Count(), shipRateTableDisplay.Rows.Count());

            Assert.NotNull(shipRateTier);
            Assert.AreEqual(shipRateTier.Key, shipRateTierDisplay.Key);
            Assert.AreEqual(shipRateTier.ShipMethodKey, shipRateTierDisplay.ShipMethodKey);
            Assert.AreEqual(shipRateTier.RangeLow, shipRateTierDisplay.RangeLow);
            Assert.AreEqual(shipRateTier.RangeHigh, shipRateTierDisplay.RangeHigh);
            Assert.AreEqual(shipRateTier.Rate, shipRateTierDisplay.Rate);

            foreach (var row in shipRateTable.Rows.OrderBy(x => x.RangeLow))
            {
                Console.WriteLine("Low: {1} {0}High: {2} {0}Rate: {3}", '\t', row.RangeLow, row.RangeHigh, row.Rate);
            }
        }

        [Test]
        public void Can_Build_ShipCountryDisplay_From_ShipCountry()
        {
            //// Arrange

            //// Act
            var shipCountryDisplay = _shipCountry.ToShipCountryDisplay();

            //// Assert
            Assert.NotNull(shipCountryDisplay);
            Assert.AreEqual(_shipCountry.Key, shipCountryDisplay.Key);
            Assert.AreEqual(_shipCountry.Name, shipCountryDisplay.Name);
            Assert.AreEqual(_shipCountry.ProvinceLabel, shipCountryDisplay.ProvinceLabel);
            Assert.AreEqual(_shipCountry.CountryCode, shipCountryDisplay.CountryCode);
        }

        // JASON: May not be needed.
        //[Test]
        //public void Can_Build_ShipCountry_From_ShipCountryDisplay()
        //{
        //    //// Arrange
        //    var shipCountryService = PreTestDataWorker.ShipCountryService;
        //    var shipCountryDisplay = shipCountryService.GetShipCountryByCountryCode(_warehouseCatalog.Key, "US").ToShipCountryDisplay();

        //    //// Act
        //    shipCountryDisplay.ProvinceLabel = "test";
        //    var shipCountry = shipCountryDisplay.ToShipCountry(shipCountryService.GetShipCountryByCountryCode(_warehouseCatalog.Key, "US"));

        //    //// Assert
        //    Assert.NotNull(shipCountry);
        //    Assert.AreEqual(shipCountryDisplay.Key, shipCountry.Key);
        //    Assert.AreEqual(shipCountryDisplay.Name, shipCountry.Name);
        //    Assert.AreEqual(shipCountryDisplay.ProvinceLabel, shipCountry.ProvinceLabel);
        //    Assert.AreEqual(shipCountryDisplay.CountryCode, shipCountry.CountryCode);
        //}

    }
}
