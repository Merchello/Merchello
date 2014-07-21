using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.Configuration;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.Services.StoreSettings;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web.Models.ContentEditing;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.DisplayClasses
{
    [TestFixture]
    public class SettingDisplayTests : DatabaseIntegrationTestBase
    {
        private IWarehouse _warehouse;
        private IWarehouseCatalog _warehouseCatalog;
        protected IStoreSettingService StoreSettingService;
        protected IMerchelloContext MerchelloContext;

        private readonly Guid _currencyCodeKey = Constants.StoreSettingKeys.CurrencyCodeKey;
        private readonly Guid _nextInvoiceNumberKey = Constants.StoreSettingKeys.NextInvoiceNumberKey;
        private readonly Guid _nextOrderNumberkey = Constants.StoreSettingKeys.NextOrderNumberKey;
        private readonly Guid _timeFormatKey = Constants.StoreSettingKeys.TimeFormatKey;
        private readonly Guid _dateFormatKey = Constants.StoreSettingKeys.DateFormatKey;
        private readonly Guid _globalShippableKey = Constants.StoreSettingKeys.GlobalShippableKey;
        private readonly Guid _globalShippingIsTaxableKey = Constants.StoreSettingKeys.GlobalShippingIsTaxableKey;
        private readonly Guid _globalTaxableKey = Constants.StoreSettingKeys.GlobalTaxableKey;
        private readonly Guid _globalTrackInventoryKey = Constants.StoreSettingKeys.GlobalTrackInventoryKey;

        [SetUp]
        public void Init()
        {
            var warehouseService = PreTestDataWorker.WarehouseService;
            _warehouse = warehouseService.GetDefaultWarehouse();

            StoreSettingService = PreTestDataWorker.StoreSettingService;
            //IStoreSetting currencyCode = new StoreSetting()
            //{
            //    Key = _currencyCodeKey,
            //    Name = "currencyCode",
            //    Value = "USD"
            //};
            //StoreSettingService.Save(currencyCode);

            //IStoreSetting nextInvoiceNumber = new StoreSetting()
            //{
            //    Key = _nextInvoiceNumberKey,
            //    Name = "currencyCode",
            //    Value = "10"
            //};
            //StoreSettingService.Save(nextInvoiceNumber);

            //IStoreSetting nextOrderNumber = new StoreSetting()
            //{
            //    Key = _nextOrderNumberkey,
            //    Name = "currencyCode",
            //    Value = "10"
            //};
            //StoreSettingService.Save(nextOrderNumber);

            //IStoreSetting timeFormat = new StoreSetting()
            //{
            //    Key = _timeFormatKey,
            //    Name = "currencyCode",
            //    Value = "am-pm"
            //};

            //StoreSettingService.Save(timeFormat);

            //IStoreSetting dateFormat = new StoreSetting()
            //{
            //    Key = _dateFormatKey,
            //    Name = "currencyCode",
            //    Value = "dd-mm-yyyy"
            //};
            //StoreSettingService.Save(dateFormat);

            //IStoreSetting globalShippable = new StoreSetting()
            //{
            //    Key = _globalShippableKey,
            //    Name = "currencyCode",
            //    Value = "false"
            //};
            //StoreSettingService.Save(globalShippable);

            //IStoreSetting globalShippingIsTaxable = new StoreSetting()
            //{
            //    Key = _globalShippingIsTaxableKey,
            //    Name = "currencyCode",
            //    Value = "false"
            //};
            //StoreSettingService.Save(globalShippingIsTaxable);

            //IStoreSetting globalTaxable = new StoreSetting()
            //{
            //    Key = _globalTaxableKey,
            //    Name = "currencyCode",
            //    Value = "false"
            //};
            //StoreSettingService.Save(globalTaxable); 
            
            //IStoreSetting globalTrackInventory = new StoreSetting()
            //{
            //    Key = _globalTrackInventoryKey,
            //    Name = "currencyCode",
            //    Value = "false"
            //};
            //StoreSettingService.Save(globalTrackInventory);
        }

        [Test]
        public void Can_Build_StoreSettingsDisplay_From_StoreSettings()
        {
            var expectedSettings = StoreSettingService.GetAll();
            var settingDisplay = new SettingDisplay();


            var actualSettings = settingDisplay.ToStoreSettingDisplay(expectedSettings);

            Assert.AreEqual(expectedSettings.First(x => x.Key == _currencyCodeKey).Value.ToLower().ToLower(), actualSettings.currencyCode.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _nextInvoiceNumberKey).Value.ToLower(), actualSettings.nextInvoiceNumber.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _nextOrderNumberkey).Value.ToLower(), actualSettings.nextOrderNumber.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _dateFormatKey).Value.ToLower(), actualSettings.dateFormat.ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _timeFormatKey).Value.ToLower(), actualSettings.timeFormat.ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalShippableKey).Value.ToLower(), actualSettings.globalShippable.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalShippingIsTaxableKey).Value.ToLower(), actualSettings.globalShippingIsTaxable.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalTaxableKey).Value.ToLower(), actualSettings.globalTaxable.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalTrackInventoryKey).Value.ToLower(), actualSettings.globalTrackInventory.ToString().ToLower());
        }

        [Test]
        public void Can_Build_StoreSettings_From_StoreSettingsDisplay()
        {
            var expectedSettings = new SettingDisplay()
            {
                currencyCode = "USD",
                dateFormat =  "mm-dd-yyyy",
                globalShippable = true,
                globalShippingIsTaxable = true,
                globalTaxable = true,
                globalTrackInventory = true,
                nextInvoiceNumber = 2,
                nextOrderNumber = 2,
                timeFormat = "am-pm"
            };

            IEnumerable<IStoreSetting> actualSettings = new List<IStoreSetting>();
            actualSettings = expectedSettings.ToStoreSetting();

            Assert.AreEqual(expectedSettings.currencyCode.ToLower(), actualSettings.First(x => x.Key == _currencyCodeKey).Value.ToLower().ToLower());
            Assert.AreEqual(expectedSettings.dateFormat.ToLower(), actualSettings.First(x => x.Key == _dateFormatKey).Value.ToLower());
            Assert.AreEqual(expectedSettings.globalShippable.ToString().ToLower(), actualSettings.First(x => x.Key == _globalShippableKey).Value.ToLower());
            Assert.AreEqual(expectedSettings.globalShippingIsTaxable.ToString().ToLower(), actualSettings.First(x => x.Key == _globalShippingIsTaxableKey).Value.ToLower());
            Assert.AreEqual(expectedSettings.globalTaxable.ToString().ToLower(), actualSettings.First(x => x.Key == _globalTaxableKey).Value.ToLower());
            Assert.AreEqual(expectedSettings.globalTrackInventory.ToString().ToLower(), actualSettings.First(x => x.Key == _globalTrackInventoryKey).Value.ToLower());
            Assert.AreEqual(expectedSettings.nextInvoiceNumber.ToString().ToLower(), actualSettings.First(x => x.Key == _nextInvoiceNumberKey).Value.ToLower());
            Assert.AreEqual(expectedSettings.nextOrderNumber.ToString().ToLower(), actualSettings.First(x => x.Key == _nextOrderNumberkey).Value.ToLower());
            Assert.AreEqual(expectedSettings.timeFormat.ToLower(), actualSettings.First(x => x.Key == _timeFormatKey).Value.ToLower());
        }

        [Test]
        public void Can_Build_StoreSettingsDisplay_From_StoreSettings_And_Change_Setting()
        {
            var expectedSettings = StoreSettingService.GetAll();
            var settingDisplay = new SettingDisplay();

            expectedSettings.First(x => x.Key == _currencyCodeKey).Value = "CAD";

            var actualSettings = settingDisplay.ToStoreSettingDisplay(expectedSettings);

            Assert.AreEqual(expectedSettings.First(x => x.Key == _currencyCodeKey).Value.ToLower().ToLower(), actualSettings.currencyCode.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _nextInvoiceNumberKey).Value.ToLower(), actualSettings.nextInvoiceNumber.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _nextOrderNumberkey).Value.ToLower(), actualSettings.nextOrderNumber.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _dateFormatKey).Value.ToLower(), actualSettings.dateFormat.ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _timeFormatKey).Value.ToLower(), actualSettings.timeFormat.ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalShippableKey).Value.ToLower(), actualSettings.globalShippable.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalShippingIsTaxableKey).Value.ToLower(), actualSettings.globalShippingIsTaxable.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalTaxableKey).Value.ToLower(), actualSettings.globalTaxable.ToString().ToLower());
            Assert.AreEqual(expectedSettings.First(x => x.Key == _globalTrackInventoryKey).Value.ToLower(), actualSettings.globalTrackInventory.ToString().ToLower());
        }
    }
}
