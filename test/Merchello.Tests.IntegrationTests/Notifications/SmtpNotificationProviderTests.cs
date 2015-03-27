using System;
using System.Configuration;
using System.Linq;
using System.Threading;
using Merchello.Core.Gateways.Notification.Smtp;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core.Events;

namespace Merchello.Tests.IntegrationTests.Notifications
{
    using System.Net;
    using System.Net.Mail;

    [TestFixture]
    public class SmtpNotificationProviderTests : DatabaseIntegrationTestBase
    {
        private SmtpNotificationGatewayProvider _provider;

        private readonly Guid _key = new Guid("5F2E88D1-6D07-4809-B9AB-D4D6036473E9");

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _provider = MerchelloContext.Gateways.Notification.GetProviderByKey(_key, false) as SmtpNotificationGatewayProvider;

            Assert.NotNull(_provider, "Provider was not resolved");

            GatewayProviderService.Saving += GatewayProviderServiceOnSaved;
        }

        private void GatewayProviderServiceOnSaved(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
        {
            var key = new Guid("5F2E88D1-6D07-4809-B9AB-D4D6036473E9");
            var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
            if (provider == null) return;

            provider.ExtendedData.SaveSmtpProviderSettings(new SmtpNotificationGatewayProviderSettings());
        }

        [SetUp]
        public void Init()
        {
            if (!_provider.Activated)
            {
                MerchelloContext.Gateways.Notification.ActivateProvider(_provider);
            }

            PreTestDataWorker.DeleteAllNotificationMethods();

        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            GatewayProviderService.Saved -= GatewayProviderServiceOnSaved;
        }

        /// <summary>
        /// Test confirms that the provider can be deactivated
        /// </summary>
        [Test]
        public void Can_DeActivate_The_SmtpNotificationGatewayProvider()
        {
            //// Arrange
            // handled by setup

            //// Act
            MerchelloContext.Gateways.Notification.DeactivateProvider(_provider);
            _provider = MerchelloContext.Gateways.Notification.GetProviderByKey(_key, false) as SmtpNotificationGatewayProvider;

            //// Assert
            Assert.NotNull(_provider);
            Assert.IsFalse(_provider.Activated);            
        }

        /// <summary>
        /// Test confirms that SMTP Provider Settings can be retrieved from the extended data collection
        /// </summary>
        [Test]
        public void Can_Retrieve_SmtpProviderSettings_From_ExtendedData()
        {
            //// Arrange
            // handled in Setup

            //// Act
            var settings = _provider.ExtendedData.GetSmtpProviderSettings();

            //// Assert
            Assert.NotNull(settings);
            Assert.AreEqual("127.0.0.1", settings.Host);
        }

        [Test]
        public void Can_Create_A_SmtpNotificationGatewayMethod()
        {
            //// Arrange
            var resource = _provider.ListResourcesOffered().FirstOrDefault();
            Assert.NotNull(resource, "Smtp Provider returned null for GatewayResource");

            //// Act
            var method = _provider.CreateNotificationMethod(resource, resource.Name, "SMTP Relayed Email");

            //// Assert
            Assert.NotNull(method);
            Assert.IsTrue(method.NotificationMethod.HasIdentity);
        }


        /// <summary>
        /// Test verifies that a NotificationMessage can be saved
        /// </summary>
        [Test]
        public void Can_Save_A_NotificationMessage()
        {
            //// Arrange
            var resource = _provider.ListResourcesOffered().FirstOrDefault();
            Assert.NotNull(resource, "Smtp Provider returned null for GatewayResource");
            var method = _provider.CreateNotificationMethod(resource, resource.Name, "SMTP Relayed Email");
            Assert.NotNull(method, "method was null");

            //// Act
            var message = new NotificationMessage(method.NotificationMethod.Key, "Test email",
                "Can_Send_A_Test_Email@merchello.com")
            {
                Recipients = "rusty@mindfly.com",
                BodyText = "Successful test?"
            };

            method.SaveNotificationMessage(message);
            
            //// Assert
            Assert.IsTrue(message.HasIdentity);

        }

        /// <summary>
        /// Test verifies that a host value can be saved to Extended Data
        /// </summary>
        [Test]
        public void Can_Save_SmtpProviderSettings_ToExtendedData()
        {
            //// Arrange
            var host = "moria";
            var key = _provider.Key;

            //// Act
            var settings = _provider.ExtendedData.GetSmtpProviderSettings();
            settings.Host = host;
            _provider.ExtendedData.SaveSmtpProviderSettings(settings);

            PreTestDataWorker.GatewayProviderService.Save(_provider.GatewayProviderSettings);

            var smtpProviderSettings = PreTestDataWorker.GatewayProviderService.GetGatewayProviderByKey(key);

            //// Assert
            Assert.AreEqual(host, smtpProviderSettings.ExtendedData.GetSmtpProviderSettings().Host);

        }



        /// <summary>
        /// Test verifies that an email can be sent using the SMTP provider
        /// </summary>
        [Test]
        public void Can_Send_A_Test_Email()
        {
            // check configuration to see if we want to do this
            if (!bool.Parse(ConfigurationManager.AppSettings["sendTestEmail"])) Assert.Ignore("Skipping test");

            var recipients = "noreply@merchello.com";


            //// Arrange
            var settings = _provider.ExtendedData.GetSmtpProviderSettings();
            settings.Host = "127.0.0.1";
            _provider.ExtendedData.SaveSmtpProviderSettings(settings);

            var resource = _provider.ListResourcesOffered().FirstOrDefault();
            Assert.NotNull(resource, "Smtp Provider returned null for GatewayResource");

            var method = _provider.CreateNotificationMethod(resource, resource.Name, "Test email method");

            //// Act
            var message = new NotificationMessage(method.NotificationMethod.Key, "Test email", "Can_Send_A_Test_Email@merchello.com")
            {
                Recipients = "dina@mindfly.com,rusty@mindfly.com",
                BodyText = "Successful test?"
            };

            method.Send(message);

            //Thread.Sleep(2000);
        }

//        [Test]
//        public void Can_Trigger_An_Email_Through_Notifications()
//        {
//            // check configuration to see if we want to do this
//            if (!bool.Parse(ConfigurationManager.AppSettings["sendTestEmail"])) Assert.Ignore("Skipping test");

//            //// Arrange
//            var settings = _provider.ExtendedData.GetSmtpProviderSettings();
//            settings.Host = "moria";
//            _provider.ExtendedData.SaveSmtpProviderSettings(settings);

//            var resource = _provider.ListResourcesOffered().FirstOrDefault();
//            Assert.NotNull(resource, "Smtp Provider returned null for GatewayResource");

//            var method = _provider.CreateNotificationMethod(resource, resource.Name, "Test email method");
//            var message = new NotificationMessage(method.NotificationMethod.Key, "Test email", "Can_Send_A_Test_Email@merchello.com")
//            {
//                Recipients = "rusty@mindfly.com",
//                BodyText =  @"{{BillToName}}
//Your address
//{{BillToAddress1}}
//{{BillToAddress2}}
//{{BillToLocality}}, {{BillToRegion}} {{BillToPostalCode}}
//
//Email : {{BillToEmail}}
//Phone : {{BillToPhone}}
//
//Invoice Number : {{InvoiceNumber}}
//
//Items Purchased:
//
//{{IterationStart[Invoice.Items]}}
//+ {{Item.Name}} -> {{Item.Sku}} -> {{Item.UnitPrice}} -> {{Item.Quantity}} -> {{Item.TotalPrice}}
//{{IterationEnd[Invoice.Items]}}
//
//Thanks for the order.
//",
//                MonitorKey = new Guid("5DB575B5-0728-4B31-9B37-E9CF6C12E0AA") // OrderConfirmationMonitor
//            };

//            method.SaveNotificationMessage(message);

//            var monitor = MonitorResolver.Current.GetMonitorByKey<INotificationMonitorBase>(message.MethodKey);

//            monitor.CacheMessage(message);

//            // Assert
//            Notification.Trigger("OrderConfirmation");
//        }
    }
}