using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Gateways.Notification.Triggering;
using Merchello.Core.Models;
using Merchello.Core.Observation;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.TestHelpers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Notifications
{
    [TestFixture]
    [Category("Notification")]
    public class NotificationObservationTests : MerchelloAllInTestBase
    {
        private IInvoice _invoice;
        private IAddress _address;
        private Guid _paymentMethodKey;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            var defaultCatalog = DbPreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");

            var us = MerchelloContext.Current.Services.StoreSettingService.GetCountryByCode("US");
            var usCountry = new ShipCountry(defaultCatalog.Key, us);
            ((ServiceContext)MerchelloContext.Current.Services).ShipCountryService.Save(usCountry);

            #region Settings -> Taxation

            var taxProvider = MerchelloContext.Current.Gateways.Taxation.CreateInstance(Core.Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);

            taxProvider.DeleteAllTaxMethods();

            var gwTaxMethod = taxProvider.CreateTaxMethod("US", 0);

            gwTaxMethod.TaxMethod.Provinces["WA"].PercentAdjustment = 8.7M;

            taxProvider.SaveTaxMethod(gwTaxMethod);


            #endregion

            _address = new Address()
            {
                Address1 = "114 W. Magnolia St.",
                Address2 = "Suite 300",
                Locality = "Bellingham",
                Region = "WA",
                CountryCode = "US"

            };

            var gateway = MerchelloContext.Current.Gateways.Payment.GetAllActivatedProviders().FirstOrDefault();
            var provider = MerchelloContext.Current.Gateways.Payment.GetProviderByKey(gateway.Key);
            var resource = provider.ListResourcesOffered().FirstOrDefault(x => x.ServiceCode == "Cash");
            var method = provider.CreatePaymentMethod(resource, "Cash", "Cash Payments");
            provider.SavePaymentMethod(method);

            _paymentMethodKey = method.PaymentMethod.Key;
        }
        
        [SetUp]
        public void Init()
        {
            DbPreTestDataWorker.DeleteAllInvoices();
            _invoice = MockInvoiceDataMaker.InvoiceForInserting(_address, 150);
            DbPreTestDataWorker.InvoiceService.Save(_invoice);
        }

        /// <summary>
        /// Test confirms that notification triggers can be resolved
        /// </summary>
        [Test]
        public void Can_Resolve_NotificationTriggers()
        {
             //// Arrange
             // 

            //// Act
            var triggers = TriggerResolver.Current.GetTriggersByArea(Topic.Notifications);

            //// Assert
            Assert.IsTrue(triggers.Any());
        }

        /// <summary>
        /// Test confirms that a collection of triggers can be retrieved by alias
        /// </summary>
        [Test]
        public void Can_Resolve_A_Collection_Of_Triggers_By_Alias()
        {
            //// Arrange
            const string alias = "OrderConfirmation";

            //// Act
            var triggers = TriggerResolver.Current.GetTriggersByAlias(alias);

            //// Asssert
            Assert.IsTrue(triggers.Any());
        }

        /// <summary>
        /// Test confirms that a trigger can be resolved by it's type T
        /// </summary>
        [Test]
        public void Can_Resolve_Trigger_Of_Type_T()
        {
            //// Arrange
            // handled in setup

            //// Act
            var trigger = TriggerResolver.Current.GetTrigger<OrderConfirmationTrigger>();

            //// Assert
            Assert.NotNull(trigger);
        }

        /// <summary>
        /// Test confirms that a collection of all monitors can be resolved
        /// </summary>
        [Test]
        public void Can_Resolve_A_Collection_Of_All_Monitors()
        {
            //// Arrange
            // handled in setup

            //// Act
            var monitor = MonitorResolver.Current.GetAllMonitors();

            //// Assert
            Assert.IsTrue(monitor.Any());
        }
    }
}