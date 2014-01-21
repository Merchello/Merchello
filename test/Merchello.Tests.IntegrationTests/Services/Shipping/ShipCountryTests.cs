using System.Data;
using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Shipping
{
    [TestFixture]
    [Category("Shipping")]
    public class ShipCountryTests : ServiceIntegrationTestBase
    {

        private IWarehouseCatalog _catalog;
        private ISettingsService _settingsService;
        private IShippingService _shippingService;
        
        [TestFixtureSetUp]
        public void FixtureInit()
        {
            // assert we have our defaults setup
            var dtos = PreTestDataWorker.Database.Query<WarehouseDto>("SELECT * FROM merchWarehouse");
            var catalogs = PreTestDataWorker.Database.Query<WarehouseCatalogDto>("SELECT * FROM merchWarehouseCatalog");

            if (!dtos.Any() || !catalogs.Any())
            {
                Assert.Ignore("Warehouse defaults are not installed.");
            }

            // TODO : This is only going to be the case for the initial Merchello release
            _catalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();

            if (_catalog == null)
            {
                Assert.Ignore("Warehouse Catalog is null");
            }

            _settingsService = PreTestDataWorker.SettingsService;
            _shippingService = PreTestDataWorker.ShippingService;
        }

        [SetUp]
        public void Init()
        {
            PreTestDataWorker.DeleteAllShipCountries();
        }

        /// <summary>
        /// Test verifies that a ship country can be created and saved associated with a warehouse catalog
        /// </summary>
        [Test]
        public void Can_Create_And_Save_A_ShipCountry()
        {
            //// Arrange
            const string countryCode = "US";
            var country = _settingsService.GetCountryByCode(countryCode);

            //// Act
            var shipCountry = new ShipCountry(_catalog.Key, country);
            Assert.IsFalse(shipCountry.HasIdentity);
            
            _shippingService.Save(shipCountry);

            //// Assert
            Assert.IsTrue(shipCountry.HasIdentity);

        }

        /// <summary>
        /// Test verifies that an error is thrown if two of the same ship countries are added to the same warehouse catalog
        /// </summary>
        [Test]
        public void Can_Verify_Attempting_To_Add_Two_Of_The_Same_Country_To_A_Catalog_Errors()
        {
            //// Arrange
            const string countryCode = "US";
            var country = _settingsService.GetCountryByCode(countryCode);

            //// Act
            var shipCountry1 = new ShipCountry(_catalog.Key, country);
            var shipCountry2 = new ShipCountry(_catalog.Key, country);

            _shippingService.Save(shipCountry1);

            //// Assert
            Assert.Throws<ConstraintException>(() => _shippingService.Save(shipCountry2));
        }

        /// <summary>
        /// Test verifies that a ShipCountry can be retrieved by it's unique key
        /// </summary>
        [Test]
        public void Can_Retrieve_A_ShipCountry()
        {
            //// Arrange
            const string countryCode = "US";
            var country = _settingsService.GetCountryByCode(countryCode);
            var shipCountry = new ShipCountry(_catalog.Key, country);
            _shippingService.Save(shipCountry);
            Assert.IsTrue(shipCountry.HasIdentity);
            var key = shipCountry.Key;

            //// Act
            var retrieved = _shippingService.GetShipCountryByKey(key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual(shipCountry.CountryCode, retrieved.CountryCode);

        }

        /// <summary>
        /// Test verifies that a ShipCountry can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_A_ShipCountry()
        {
            //// Arrange
            const string countryCode = "US";
            var country = _settingsService.GetCountryByCode(countryCode);
            var shipCountry = new ShipCountry(_catalog.Key, country);
            _shippingService.Save(shipCountry);
            Assert.IsTrue(shipCountry.HasIdentity);
            var key = shipCountry.Key;

            //// Act
            _shippingService.Delete(shipCountry);            

            //// Assert
            Assert.IsNull(_shippingService.GetShipCountryByKey(key));
        }

        /// <summary>
        /// Test verifies that a collection of ShipCountries can be retrieved for a warehouse catalog
        /// </summary>
        [Test]
        public void Can_Retreive_A_List_Of_ShipCountries_By_WarehouseCatalog()
        {
            //// Arrange
            var countries = new[]
            {
                new ShipCountry(_catalog.Key, _settingsService.GetCountryByCode("US")),
                new ShipCountry(_catalog.Key, _settingsService.GetCountryByCode("FR")),
                new ShipCountry(_catalog.Key, _settingsService.GetCountryByCode("AU")),
                new ShipCountry(_catalog.Key, _settingsService.GetCountryByCode("GB")),
                new ShipCountry(_catalog.Key, _settingsService.GetCountryByCode("TR"))
            };

            var expected = countries.Count();

            foreach (var country in countries)
            { 
                _shippingService.Save(country);
            }

            //// Act
            var retrieved = _shippingService.GetShipCountriesByCatalogKey(_catalog.Key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.IsTrue(retrieved.Any());
            Assert.AreEqual(expected, retrieved.Count());
        }
    }
}