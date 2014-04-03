using System;
using System.Configuration;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.AuthorizeNet.Models;
using Merchello.Tests.AuthorizeNet.Integration.TestHelpers;
using NUnit.Framework;

namespace Merchello.Tests.AuthorizeNet.Integration.Authorization
{
    [TestFixture]
    public class AuthorizationTests : ProviderTestsBase
    {
        private IInvoice _invoice;

        [SetUp]
        public void Init()
        {

            var billTo = new Address()
            {
                Organization = "Mindfly Web Design Studios",
                Address1 = "114 W. Magnolia St. Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                PostalCode = "98225",
                CountryCode = "US",
                Email = "someone@mindfly.com",
                Phone = "555-555-5555"
            };

            // create an invoice
            var invoiceService = new InvoiceService();

            _invoice = invoiceService.CreateInvoice(Core.Constants.DefaultKeys.InvoiceStatus.Unpaid);

            _invoice.SetBillingAddress(billTo);

            _invoice.Total = 120M;
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");
            
            // make up some line items
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 10, 1, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Product, "Item 2", "I2", 2, 40, extendedData);
            var l3 = new InvoiceLineItem(LineItemType.Shipping, "Shipping", "shipping", 1, 10M, extendedData);
            var l4 = new InvoiceLineItem(LineItemType.Tax, "Tax", "tax", 1, 10M, extendedData);

            _invoice.Items.Add(l1);
            _invoice.Items.Add(l2);
            _invoice.Items.Add(l3);
            _invoice.Items.Add(l4);

            var processorSettings = new AuthorizeNetProcessorSettings
            {
                LoginId = ConfigurationManager.AppSettings["xlogin"],
                TransactionKey = ConfigurationManager.AppSettings["xtrankey"],
                UseSandbox = true
            };

            Provider.GatewayProvider.ExtendedData.SaveProcessorSettings(processorSettings);

            if (Provider.PaymentMethods.Any()) return;
            var resource = Provider.ListResourcesOffered().ToArray();
            Provider.CreatePaymentMethod(resource.First(), "Credit Card", "Credit Card");
        }

        /// <summary>
        /// Testing Sandbox Authorize method
        /// </summary>
        [Test]
        public void Can_Authorize_A_Payment()
        {
            //// Arrange
            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = new CreditCardFormData()
            {
                CreditCardType = "VISA",
                CardholderName = "Rusty Swayne",
                CardNumber = "4111111111111111",
                CardCode = "111",
                ExpireMonth = "09",
                ExpireYear = "15",
                CustomerIp = "10.0.0.15"
            };

            //// Act
            var result = creditCardMethod.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());

            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Payment.Success);
            var payment = result.Payment.Result;
            Console.WriteLine(payment.ExtendedData.GetValue(Plugin.Payments.AuthorizeNet.Constants.ExtendedDataKeys.AuthorizationTransactionCode));
            Console.WriteLine(payment.ExtendedData.GetValue(Plugin.Payments.AuthorizeNet.Constants.ExtendedDataKeys.AuthorizationTransactionResult));
            Console.WriteLine(payment.ExtendedData.GetValue(Plugin.Payments.AuthorizeNet.Constants.ExtendedDataKeys.AvsResult));
        }

        /// <summary>
        /// Testing Sandbox and Authorize and Capture a Payment
        /// </summary>
        [Test]
        public void Can_AuthorizeAndCapture_A_Payment()
        {
            //// Arrange
            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = new CreditCardFormData()
            {
                CreditCardType = "VISA",
                CardholderName = "Rusty Swayne",
                CardNumber = "4111111111111111",
                CardCode = "111",
                ExpireMonth = "09",
                ExpireYear = "15",
                CustomerIp = "10.0.0.15"
            };

            //// Act
            var result = creditCardMethod.AuthorizeCapturePayment(_invoice, _invoice.Total, ccEntry.AsProcessorArgumentCollection());

            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Payment.Success);
            var payment = result.Payment.Result;
            Console.WriteLine(payment.ExtendedData.GetValue(Plugin.Payments.AuthorizeNet.Constants.ExtendedDataKeys.AuthorizationTransactionCode));
            Console.WriteLine(payment.ExtendedData.GetValue(Plugin.Payments.AuthorizeNet.Constants.ExtendedDataKeys.AuthorizationTransactionResult));
            Console.WriteLine(payment.ExtendedData.GetValue(Plugin.Payments.AuthorizeNet.Constants.ExtendedDataKeys.AvsResult));
        }
    }
}
