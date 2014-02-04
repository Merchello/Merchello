using System.Linq;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.Respositories;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    public class SettingsServiceTests
    {
        private IStoreSettingService _storeSettingService;

        [SetUp]
        public void Init()
        {
            _storeSettingService = new StoreSettingService();
        }
        
        /// <summary>
        /// Test verifies that the service correctly removes countries passed an array of country codes
        /// </summary>
        [Test]

        public void Can_Retrieve_A_RegionList_That_Excludes_Countries()
        {
            //// Arrange
            var excludes = new[] {"SA", "DK"};

            //// Act
            var regions = _storeSettingService.GetAllCountries(excludes);

            //// Assert
            Assert.IsTrue(regions.Any());
            Assert.IsFalse(regions.Contains(new Country("SA")));
            Assert.IsFalse(regions.Contains(new Country("DK")));

        }

        /// <summary>
        /// Test verifies that the service correctly returns the RegionInfo for Denmark given the country code DK
        /// </summary>
        [Test]
        public void Can_Retrieve_Denmark_Region_By_DK_Code()
        {
            //// Arrange
            const string countryCode = "DK";

            //// Act
            var denmark = _storeSettingService.GetCountryByCode(countryCode);

            //// Assert
            Assert.NotNull(denmark);
            Assert.AreEqual(countryCode, denmark.CountryCode);
        }

       


        //[Test]
        //public void Can_Serialize_ProvinceCodes()
        //{
        //    var states = _warehouseService.GetRegionByCode("US").Provinces;

        //    var json = JsonConvert.SerializeObject(states.ToArray());

        //    Console.Write(json);

        //    var reversed = JsonConvert.DeserializeObject<IEnumerable<Province>>(json);


        //    Console.Write(json);
        //}
    }

  
}