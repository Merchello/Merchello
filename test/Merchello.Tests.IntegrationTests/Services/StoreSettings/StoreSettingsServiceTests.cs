using System.Dynamic;
using System.Globalization;
using System.Linq;
using Merchello.Core.Models.Rdbms;
using Merchello.Core.Services;
using NUnit.Framework;
using StackExchange.Profiling;
using umbraco.cms.businesslogic.datatype;

namespace Merchello.Tests.IntegrationTests.Services.StoreSettings
{
    [TestFixture]
    [Category("Service Integration")]
    public class StoreSettingsServiceTests : ServiceIntegrationTestBase
    {
        private IStoreSettingService _settingsService;

        [TestFixtureSetUp]
        public void FixtureInit()
        {
            // assert we have our defaults setup
            var dtos = PreTestDataWorker.Database.Query<StoreSettingDto>("SELECT * FROM merchStoreSetting");

            if (!dtos.Any())
            {
                Assert.Ignore("StoreSetting defaults are not installed.");
            }
        }

        [SetUp]
        public void Init()
        {
            _settingsService = PreTestDataWorker.StoreSettingService;
        }

        /// <summary>
        /// Test asserts that a store setting can be retrieve by it's unique key (Guid)
        /// </summary>
        [Test]
        public void Can_Get_A_StoreSetting_By_Key()
        {
            //// Arrange
            var key = Core.Constants.StoreSettingKeys.CurrencyCodeSettingKey;
            var expected = "currencyCode";

            //// Act
            var storeSetting = _settingsService.GetByKey(key);

            //// Assert
            Assert.NotNull(storeSetting);
            Assert.AreEqual(expected, storeSetting.Name);
        }

        /// <summary>
        /// Test asserts that a store setting can be updated
        /// </summary>
        [Test]
        public void Can_Update_A_StoreSetting()
        {
            //// Arrange
            var key = Core.Constants.StoreSettingKeys.CurrencyCodeSettingKey;
            var region = new RegionInfo("DK");
            var expected = "DKK";

            //// Act
            var setting = _settingsService.GetByKey(key);
            setting.Value = region.ISOCurrencySymbol;
            _settingsService.Save(setting);

            var retrieved = _settingsService.GetByKey(key);

            //// Assert
            Assert.NotNull(retrieved);
            Assert.AreEqual(expected, retrieved.Value);
        }

        /// <summary>
        /// Test verifies that a new store setting can be created
        /// </summary>
        [Test]
        public void Can_Create_A_Store_Setting()
        {
            //// Arrange

            //// Act
            var setting = _settingsService.CreateStoreSettingWithKey("Testing", "Rusty", typeof (string).FullName);

            //// Assert
            Assert.NotNull(setting);
            Assert.IsTrue(setting.HasIdentity);
        }
        
        /// <summary>
        /// Test verifies that a storesetting can be deleted
        /// </summary>
        [Test]
        public void Can_Delete_A_StoreSetting()
        {
            //// Arrange
            var setting = _settingsService.CreateStoreSettingWithKey("Testing", "Rusty", typeof(string).FullName);
            var key = setting.Key;

            //// Act
            _settingsService.Delete(setting);
            var retrieved = _settingsService.GetByKey(key);

            //// Assert
            Assert.IsNull(retrieved);
        }

        /// <summary>
        /// Test verifies that GetAll works ;-)
        /// </summary>
        [Test]
        public void Can_Retrieve_All_StoreSettings()
        {
            //// Arrange
            
            //// Act
            var settings = _settingsService.GetAll();

            //// Assert
            Assert.IsTrue(settings.Any());
            Assert.IsTrue(8 <= settings.Count());

        }
    }
}