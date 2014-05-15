using System;
using System.Runtime.InteropServices;
using Merchello.Core.Gateways.Notification.Smtp;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Notifications
{
    [TestFixture]
    public class SmtpNotificationProviderTests : DatabaseIntegrationTestBase
    {
        private SmtpNotificationGatewayProvider _provider;

        private Guid _key = new Guid("5F2E88D1-6D07-4809-B9AB-D4D6036473E9");

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _provider = MerchelloContext.Gateways.Notification.GetProviderByKey(_key, false) as SmtpNotificationGatewayProvider;

            Assert.NotNull(_provider, "Provider was not resolved");

            if (!_provider.Activated)
            {
                MerchelloContext.Gateways.Notification.ActivateProvider(_provider);
            }
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
    }
}