namespace Merchello.Tests.IntegrationTests.Services.Shipping
{
    using System;
    using System.Linq;
    using System.Runtime.InteropServices;

    using Merchello.Core;
    using Merchello.Core.Builders;
    using Merchello.Core.Models;
    using Merchello.Core.Services;
    using Merchello.Tests.Base.DataMakers;
    using Merchello.Tests.Base.TestHelpers;
    using Merchello.Web.Models.ContentEditing;

    using NUnit.Framework;

    [TestFixture]
    public class ShippingServiceTests : MerchelloAllInTestBase
    {
        private IShipmentService _shipmentService;

        [SetUp]
        public void Init()
        {
            _shipmentService = MerchelloContext.Current.Services.ShipmentService;
        }

        /// <summary>
        /// Test asserts that all shipment statuses can be queried
        /// </summary>
        [Test]
        public void Can_Query_All_ShipmentStatuses()
        {
            //// Arrange
            
            //// Act
            var statuses = _shipmentService.GetAllShipmentStatuses().ToArray();

            //// Assert
            Assert.AreEqual(5, statuses.Count());
            foreach (var status in statuses.Select(x => x.ToShipmentStatusDisplay()))
            {
                Console.WriteLine(status.Name + ' ' + status.SortOrder);
            }

        }

        public void Can_Detect_A_ShipmentStatusChange()
        {
            //// Arrange
            var address = new Address()
                              {
                                  Address1 = "111 Somewhere St.",
                                  Locality = "Bellingham",
                                  Region = "WA",
                                  PostalCode = "98225",
                                  CountryCode = "US",
                                  Name = "Merchello"
                              };

            var invoice = MockInvoiceDataMaker.GetMockInvoiceForTaxation();

            var payment = new Payment(PaymentMethodType.Cash, invoice.Total) { Collected = true, Authorized = true };

            MerchelloContext.Current.Services.PaymentService.Save(payment);

            MerchelloContext.Current.Services.InvoiceService.Save(invoice);

            var appliedPaymentService = ((ServiceContext)(MerchelloContext.Current.Services)).AppliedPaymentService;

            var appliedPayment = appliedPaymentService.CreateAppliedPaymentWithKey(
                payment.Key,
                invoice.Key,
                AppliedPaymentType.Debit,
                "Payment applied",
                payment.Amount);

            invoice = MerchelloContext.Current.Services.InvoiceService.GetByKey(invoice.Key);

            Assert.NotNull(invoice);

            var order = invoice.PrepareOrder();

            MerchelloContext.Current.Services.OrderService.Save(order);

            var builder = new ShipmentBuilderChain(MerchelloContext.Current, order, order.Items.Select(x => x.Key), Guid.NewGuid(), Constants.DefaultKeys.ShipmentStatus.Packaging, "Track", "http://url.com", "carrier");

            var attempt = builder.Build();

            Assert.IsTrue(attempt.Success);
        }
         
    }
}