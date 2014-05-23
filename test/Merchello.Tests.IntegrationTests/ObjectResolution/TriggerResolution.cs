using System;
using Merchello.Core.Triggers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.ObjectResolution
{
    [TestFixture]
    public class TriggerResolution : MerchelloAllInTestBase
    {

        private readonly Guid _key = new Guid("2DA5CE92-E388-4788-A647-CDEA82EE6C9F");

        [SetUp]
        public void Init()
        {
            MockInvoiceTrigger.EventInvoked = false;
        }

        /// <summary>
        /// Simple test resolution of a mock trigger
        /// </summary>
        [Test]
        public void Can_Resolve_MockInvoiceTrigger()
        {
            //// Arrange
            // resolver is setup in the bootstrapper

            //// Act
            var trigger = TriggerResolver.Current.TryGetTrigger(_key);

            //// Assert
            Assert.NotNull(trigger, "Trigger was not resolved");
        }

        /// <summary>
        /// Test binding to a service event and handling of event
        /// </summary>
        [Test]
        public void Can_Show_Resolved_Trigger_Invokes_On_Event()
        {
            //// Arrage
            var invoiceService = DbPreTestDataWorker.InvoiceService;

            //// Act
            var invoice = invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            //// Assert
            Assert.IsTrue(MockInvoiceTrigger.EventInvoked);
        }
    }
}