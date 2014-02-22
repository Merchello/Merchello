using System.Data;
using System.Linq;
using Merchello.Core.Gateways.Taxation.FixedRate;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Taxation
{
    [TestFixture]
    [Category("Taxation")]
    public class TaxMethodTaxationGatewayProviderTests : TaxationProviderTestBase
    {
        private IFixedRateTaxationGatewayProvider _taxProvider;

        [SetUp]
        public void Init()
        {
            _taxProvider = (IFixedRateTaxationGatewayProvider)MerchelloContext.Gateways.Taxation.ResolveByKey(Core.Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);

            PreTestDataWorker.DeleteAllCountryTaxRates(Core.Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);
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
            var taxMethods = GatewayProviderService.GetTaxMethodsByProviderKey(_taxProvider.Key);

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
            var taxMethod = _taxProvider.CreateTaxMethod(countryCode);

            //// Assert
            Assert.NotNull(taxMethod);
            Assert.IsTrue(taxMethod.HasProvinces);
            Assert.AreEqual(type.Name, taxMethod.Provinces.First().GetType().Name);
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
            var taxMethod = _taxProvider.CreateTaxMethod(countryCode, 5);
            Assert.IsTrue(taxMethod.HasProvinces);

            //// Act
            taxMethod.Provinces["WA"].PercentRateAdjustment = 3;
            _taxProvider.SaveTaxMethod(taxMethod);

            var retrieved = _taxProvider.GetTaxMethodByCountryCode(countryCode);
            Assert.NotNull(retrieved);

            //// Assert
            Assert.IsTrue(retrieved.HasProvinces);
            Assert.AreEqual(3, retrieved.Provinces["WA"].PercentRateAdjustment);
            Assert.AreEqual(5, taxMethod.PercentageTaxRate);
        }
    }
}