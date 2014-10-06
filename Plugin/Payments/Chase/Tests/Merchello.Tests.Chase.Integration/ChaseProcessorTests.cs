using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Merchello.Core;
using Merchello.Core.Gateways;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Chase;
using Merchello.Plugin.Payments.Chase.Models;
using Merchello.Tests.Chase.Integration.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.Chase.Integration
{
    [TestFixture]
    public class ChaseProcessorTests : ChaseTestBase
    {
        private IInvoice _invoice;
        private IPaymentGatewayMethod _payment;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            base.TestFixtureSetup();
        }

        [SetUp]
        public void Init()
        {
            var billTo = new Address()
            {
                Organization = "Proworks",
                Address1 = "777 NE 2nd St.",
                Locality = "Corvallis",
                Region = "OR",
                PostalCode = "97330",
                CountryCode = "US",
                Email = "someone@proworks.com",
                Phone = "555-555-5555"
            };

            // create an invoice
            var invoiceService = new InvoiceService();

            _invoice = invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);
                                                  
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            var processorSettings = new ChaseProcessorSettings
            {
                MerchantId = ConfigurationManager.AppSettings["merchantId"],
                Bin = ConfigurationManager.AppSettings["bin"],
                Username = ConfigurationManager.AppSettings["username"],
                Password = ConfigurationManager.AppSettings["password"]
            };

            Provider.GatewayProviderSettings.ExtendedData.SaveProcessorSettings(processorSettings);

            if (Provider.PaymentMethods.Any()) return;
            var resource = new GatewayResource("CreditCard", "Credit Card");
            _payment = Provider.CreatePaymentMethod(resource, "Credit Card", "Credit Card");
        }

        [Test]
        public void TestCaseNumber_1a()
        {
            //Arrange

            // Card - 4011 3611 0000 0012
            var card = "4011361100000012";

            var cardCode = "111";

            // AVS Zip - 22222
            var postalCode = "22222";

            // Amount - 25.00
            var amount = 25;            
            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", amount, 1, extendedData);

            _invoice.Items.Add(l1);
            
            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = new CreditCardFormData()
            {
                CreditCardType = "ChaseNet",
                CardholderName = "Test User",
                CardNumber = card,
                CardCode = cardCode,
                ExpireMonth = "09",
                ExpireYear = "15",
                CustomerIp = "10.0.0.15"
            };


            // Act
            IPaymentResult result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert

            //Assert.IsTrue(result.Payment.Success);
            //Assert.IsTrue(result.ApproveOrderCreation);
        }
        [Test]
        public void TestCaseNumber_2a()
        {
            //Arrange

            // Card - 4011 3611 0000 0012
            var card = "4788250000028291";    
            var cardCode = "111";


            // AVS Zip - 22222
            var postalCode = "11111";

            // Amount - 25.00
            var amount = 30;
            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", amount, 1, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = new CreditCardFormData()
            {
                CreditCardType = "VISA",
                CardholderName = "Test User",
                CardNumber = card,
                CardCode = cardCode,
                ExpireMonth = "09",
                ExpireYear = "15",
                CustomerIp = "10.0.0.15"
            };


            // Act
            IPaymentResult result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);
        }
        [Test]
        public void TestCaseNumber_2b()
        {
            //Arrange

            // Card - 4011 3611 0000 0012
            var card = "4788250000028291";
            var cardCode = "111";


            // AVS Zip - 22222
            var postalCode = "11111";

            // Amount - 25.00
            var amount = 30;
            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", amount, 1, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = new CreditCardFormData()
            {
                CreditCardType = "VISA",
                CardholderName = "Test User",
                CardNumber = card,
                CardCode = cardCode,
                ExpireMonth = "09",
                ExpireYear = "15",
                CustomerIp = "10.0.0.15"
            };


            // Act
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.VoidPayment(_invoice, result.Payment.Result, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);
        }
    }
}
