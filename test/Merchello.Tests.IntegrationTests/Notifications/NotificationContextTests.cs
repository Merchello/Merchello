namespace Merchello.Tests.IntegrationTests.Notifications
{
    using System;
    using System.Configuration;
    using System.Linq;
    using System.Text;

    using Merchello.Core;
    using Merchello.Core.Gateways.Notification;
    using Merchello.Core.Gateways.Notification.Monitors;
    using Merchello.Core.Gateways.Notification.Smtp;
    using Merchello.Core.Gateways.Payment;
    using Merchello.Core.Models;
    using Merchello.Core.Models.MonitorModels;
    using Merchello.Core.Services;
    using Merchello.Examine.DataServices;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.Base.TestHelpers;

    using Moq;

    using MySql.Data.MySqlClient;

    using NUnit.Framework;

    using Umbraco.Core;
    using Umbraco.Core.Events;

    [TestFixture]
    public class NotificationContextTests : MerchelloAllInTestBase
    {
        private SmtpNotificationGatewayProvider _provider;

        private readonly Guid _key = new Guid("5F2E88D1-6D07-4809-B9AB-D4D6036473E9");

        // OrderConfirmationMonitor
        private readonly Guid _monitorKey = new Guid("5DB575B5-0728-4B31-9B37-E9CF6C12E0AA");

        private Guid _paymentMethodKey;

        private INotificationMessage _message;

        #region Setup

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _provider = MerchelloContext.Current.Gateways.Notification.GetProviderByKey(_key, false) as SmtpNotificationGatewayProvider;

            Assert.NotNull(_provider, "Provider was not resolved");

            GatewayProviderService.Saving += GatewayProviderServiceOnSaved;
        }

        private void GatewayProviderServiceOnSaved(IGatewayProviderService sender, SaveEventArgs<IGatewayProviderSettings> args)
        {
            var key = new Guid("5F2E88D1-6D07-4809-B9AB-D4D6036473E9");
            var provider = args.SavedEntities.FirstOrDefault(x => key == x.Key && !x.HasIdentity);
            if (provider == null) return;

            var settings = new SmtpNotificationGatewayProviderSettings("smtp.gmail.com")
                               {
                                    Port = 587,
                                    EnableSsl = true,
                                    UserName = "[username]",
                                    Password = "[password]"
                               };

            provider.ExtendedData.SaveSmtpProviderSettings(settings);
        }


        [SetUp]
        public void Init()
        {
            if (!_provider.Activated)
            {
                MerchelloContext.Current.Gateways.Notification.ActivateProvider(_provider);
            }

            DbPreTestDataWorker.DeleteAllNotificationMethods();

            //// Arrange
            var resource = _provider.ListResourcesOffered().FirstOrDefault();
            Assert.NotNull(resource, "Smtp Provider returned null for GatewayResource");
            var method = _provider.CreateNotificationMethod(resource, resource.Name, "SMTP Relayed Email");
            _provider.SaveNotificationMethod(method);
            Assert.NotNull(method, "method was null");

            //// Act
            _message = new NotificationMessage(method.NotificationMethod.Key, "Test email",
                "Can_Send_A_Test_Email@merchello.com")
            {
                Recipients = "test@test.com",
                BodyText = "Successful test?",
                MonitorKey = _monitorKey,
                SendToCustomer = true
            };

            method.SaveNotificationMessage(_message);

            

            //// Assert
            Assert.IsTrue(_message.HasIdentity);

        }

        [TestFixtureTearDown]
        public override void TestFixtureTearDown()
        {
            base.TestFixtureTearDown();
           
            GatewayProviderService.Saved -= GatewayProviderServiceOnSaved;
        }
        
        #endregion

        /// <summary>
        /// Test asserts that a notification message can be sent through the notification context
        /// </summary>
        [Test]
        public void Can_Send_A_Test_Message_Using_The_NotificationContext()
        {
            // check configuration to see if we want to do this
            if (!bool.Parse(ConfigurationManager.AppSettings["sendTestEmail"])) Assert.Ignore("Skipping test");

            //// Arrange
            var method = _provider.NotificationMethods.FirstOrDefault();
            Assert.NotNull(method, "method was null");

            //// Act
            MerchelloContext.Current.Gateways.Notification.Send(_message);

            //// Assert
            // check your inbox
        }

        [Test]
        public void Can_Simulate_A_TriggeredOrderConfirmation()
        {
            // check configuration to see if we want to do this
            if (!bool.Parse(ConfigurationManager.AppSettings["sendTestEmail"])) Assert.Ignore("Skipping test");

            //// Arrange
            var monitor = new OrderConfirmationMonitor(MerchelloContext.Current.Gateways.Notification);

            var invoice = MockInvoiceDataMaker.GetMockInvoiceForTaxation();
            MerchelloContext.Current.Services.InvoiceService.Save(invoice);

            var paymentMethods = MerchelloContext.Current.Gateways.Payment.GetPaymentGatewayMethods().ToArray();
            Assert.IsTrue(paymentMethods.Any(), "There are no payment methods");


            var paymentResult = invoice.AuthorizeCapturePayment(
                MerchelloContext.Current,
                paymentMethods.FirstOrDefault().PaymentMethod.Key,
                new ProcessorArgumentCollection());

            //// Act
            Notification.Trigger("OrderConfirmation", paymentResult, new [] { "rs@test.com" });
            Notification.Trigger("OrderConfirmation", paymentResult, new[] { "rs2@test.com" });

            //// Assert

            // check email

        }

        /// <summary>
        /// http://issues.merchello.com/youtrack/issue/M-591
        /// </summary>
        [Test]
        public void Can_Trigger_Two_Emails_Without_Having_RecipientsCollections_Combining()
        {
            // check configuration to see if we want to do this
            if (!bool.Parse(ConfigurationManager.AppSettings["sendTestEmail"])) Assert.Ignore("Skipping test");

            //// Arrange
            var monitor = new OrderConfirmationMonitor(MerchelloContext.Current.Gateways.Notification);

            var invoice = MockInvoiceDataMaker.GetMockInvoiceForTaxation();
            MerchelloContext.Current.Services.InvoiceService.Save(invoice);

            var paymentMethods = MerchelloContext.Current.Gateways.Payment.GetPaymentGatewayMethods().ToArray();
            Assert.IsTrue(paymentMethods.Any(), "There are no payment methods");


            var paymentResult = invoice.AuthorizeCapturePayment(
                MerchelloContext.Current,
                paymentMethods.FirstOrDefault().PaymentMethod.Key,
                new ProcessorArgumentCollection());

            //// Act
            var contact1 = "[email1]";
            Notification.Trigger("OrderConfirmation", paymentResult, new [] {contact1});

            var contact2 = "[email2]";
            Notification.Trigger("OrderConfirmation", paymentResult, new [] {contact2});
        }
        
    }
}