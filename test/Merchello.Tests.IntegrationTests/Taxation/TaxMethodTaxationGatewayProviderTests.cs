using System.Data;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways.Taxation;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Taxation
{
    [TestFixture]
    [Category("Taxation")]
    public class TaxMethodTaxationGatewayProviderTests : TaxationProviderTestBase
    {
        private ITaxationGatewayProvider _taxProvider;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var defaultCatalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");

            // we need a ShipCountry
            PreTestDataWorker.DeleteAllShipCountries();

            var us = PreTestDataWorker.StoreSettingService.GetCountryByCode("US");
            var usCountry = new ShipCountry(defaultCatalog.Key, us);
            PreTestDataWorker.ShipCountryService.Save(usCountry);

        }

        [SetUp]
        public void Init()
        {
            
            _taxProvider = MerchelloContext.Gateways.Taxation.GetProviderByKey(Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);

            PreTestDataWorker.DeleteAllCountryTaxRates(Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);
        }

        /// <summary>
        /// Test verifies that the tax provider can create a <see cref="ITaxMethod"/>
        /// </summary>
        [Test]
        public void Can_Create_TaxMethod()
        {
            //// Arrange
            const string countryCode = "US";
            const int expected = 1;

            //// Act
            _taxProvider.CreateTaxMethod(countryCode);
            var taxMethods = _taxProvider.GetAllGatewayTaxMethods();

            //// Assert
            Assert.NotNull(taxMethods);
            Assert.IsTrue(taxMethods.Any());
            Assert.AreEqual(expected, taxMethods.Count());
        }

        /// <summary>
        /// Test verifies that the US CountryTaxRate created tax provider contains provinces 
        /// </summary>
        [Test]
        public void TaxMethod_Created_Has_A_PopulatedTaxProvince_Collection()
        {
            //// Arrange
            const string countryCode = "US";
            var type = typeof(TaxProvince);

            //// Act
            var gwTaxMethod = _taxProvider.CreateTaxMethod(countryCode);

            //// Assert
            Assert.NotNull(gwTaxMethod);
            Assert.IsTrue(gwTaxMethod.TaxMethod.HasProvinces);
            Assert.AreEqual(type.Name, gwTaxMethod.TaxMethod.Provinces.First().GetType().Name);
        }

        /// <summary>
        /// Test verifies that a provider cannot add mulitple CountryRateTypes for a given country
        /// </summary>
        [Test]
        public void Provider_Cannot_Create_Multiple_TaxMethod_For_A_Country()
        {
            //// Arrange
            const string countryCode = "US";

            //// Act
            _taxProvider.CreateTaxMethod(countryCode);

            //// Assert
            Assert.Throws<ConstraintException>(() => _taxProvider.CreateTaxMethod(countryCode));
        }

        /// <summary>
        /// Test verifies that province data is persisted on save
        /// </summary>
        [Test]
        public void Can_Save_And_Retrieve_A_Value_In_Province_Data()
        {
            //// Arrange
            const string countryCode = "US";
            var gwTaxMethod = _taxProvider.CreateTaxMethod(countryCode, 5);
            Assert.IsTrue(gwTaxMethod.TaxMethod.HasProvinces);

            //// Act
            gwTaxMethod.TaxMethod.Provinces["WA"].PercentAdjustment = 3;
            _taxProvider.SaveTaxMethod(gwTaxMethod);

            var retrieved = _taxProvider.GetGatewayTaxMethodByCountryCode(countryCode);
            Assert.NotNull(retrieved);

            //// Assert
            Assert.IsTrue(retrieved.TaxMethod.HasProvinces);
            Assert.AreEqual(3, retrieved.TaxMethod.Provinces["WA"].PercentAdjustment);
            Assert.AreEqual(5, gwTaxMethod.TaxMethod.PercentageTaxRate);
        }
    }
}