using System;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting;
using Merchello.Core;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Plugin.Payments.Stripe;
using Merchello.Plugin.Payments.Stripe.Models;
using Merchello.Tests.Stripe.Integration.TestHelpers;
using NUnit.Framework;
using Constants  = Merchello.Plugin.Payments.Stripe.Constants;

namespace Merchello.Tests.Stripe.Integration.Tests
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
                Organization = "Flightpath",
                Address1 = "36 West 25th Street",
                Locality = "New York",
                Region = "NY",
                PostalCode = "10010",
                CountryCode = "US",
                Email = "someone@flightpath.com",
                Phone = "212-555-5555"
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

            var processorSettings = new StripeProcessorSettings
            {
                ApiKey = ConfigurationManager.AppSettings["stripeApiKey"]
            };

            Provider.GatewayProviderSettings.ExtendedData.SaveProcessorSettings(processorSettings);

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
                CardholderName = "Alex Lindgren",
                CardNumber = "4012888888881881",
                CardCode = "111",
                ExpireMonth = "09",
                ExpireYear = "15"
            };

            //// Act
            var result = creditCardMethod.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());

            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Payment.Success);
            var payment = result.Payment.Result;
        }

        [Test]
        public void Can_AuthFail_A_Payment()
        {
            //// Arrange
            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = new CreditCardFormData()
            {
                CreditCardType = "VISA",
                CardholderName = "Alex Lindgren",
                CardNumber = "1234123412341234",
                CardCode = "111",
                ExpireMonth = "09",
                ExpireYear = "15"
            };

            //// Act
            var result = creditCardMethod.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());

            //// Assert
            Assert.NotNull(result);
            Assert.IsFalse(result.Payment.Success);
            Assert.IsTrue(result.Payment.Exception.Message == "Your card number is incorrect.");
            
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
                CardholderName = "Alex Lindgren",
                CardNumber = "4012888888881881",
                CardCode = "111",
                ExpireMonth = "09",
                ExpireYear = "15"
            };

            //// Act
            var result = creditCardMethod.AuthorizeCapturePayment(_invoice, _invoice.Total, ccEntry.AsProcessorArgumentCollection());

            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Payment.Success);
            var payment = result.Payment.Result;

            Assert.IsFalse(_invoice.IsDirty());
            Assert.AreEqual(Core.Constants.DefaultKeys.InvoiceStatus.Paid, _invoice.InvoiceStatusKey);
        }

        /// <summary>
        /// Test verifies the prior auth then capture
        /// </summary>
        [Test]
        public void Can_Authorize_And_Then_Later_Capture_A_Payment()
        {
            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = new CreditCardFormData()
            {
                CreditCardType = "VISA",
                CardholderName = "Alex Lindgren",
                CardNumber = "4012888888881881",
                CardCode = "111",
                ExpireMonth = "09",
                ExpireYear = "15"
            };

            var authorizes = creditCardMethod.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            Assert.IsTrue(authorizes.Payment.Success, "authorize call failed");
            Assert.AreNotEqual(Core.Constants.DefaultKeys.InvoiceStatus.Paid, _invoice.InvoiceStatusKey, "invoice is marked as paid and is only authorized");

            //// Act
            var authorizedPayment = authorizes.Payment.Result;

            var result = creditCardMethod.CapturePayment(_invoice, authorizedPayment, _invoice.Total, new ProcessorArgumentCollection());


            //// Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Payment.Success);
            var payment = result.Payment.Result;

            Assert.IsFalse(_invoice.IsDirty());
            Assert.AreEqual(Core.Constants.DefaultKeys.InvoiceStatus.Paid, _invoice.InvoiceStatusKey);
        }

        // This test does not work - I think it is because MerchelloContext.Current is null and PaymentExtentions.AppliedPayments() requires it
        ///// <summary>
        ///// Test verifies the AuthCapture and then refunding a payment
        ///// </summary>
        //[Test]
        //public void Can_AuthorizeAndCapture_And_Then_Later_Refund_A_Payment()
        //{
        //    //// Arrange
        //    var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
        //    Assert.NotNull(creditCardMethod);

        //    var ccEntry = new CreditCardFormData()
        //    {
        //        CreditCardType = "VISA",
        //        CardholderName = "Alex Lindgren",
        //        CardNumber = "4012888888881881",
        //        CardCode = "111",
        //        ExpireMonth = "09",
        //        ExpireYear = "15",
        //        CustomerIp = "10.0.0.15"
        //    };

        //    //// Act
        //    var result = creditCardMethod.AuthorizeCapturePayment(_invoice, _invoice.Total, ccEntry.AsProcessorArgumentCollection());

        //    //// Assert
        //    Assert.NotNull(result);
        //    Assert.IsTrue(result.Payment.Success);
        //    var payment = result.Payment.Result;

        //    Assert.IsFalse(_invoice.IsDirty());
        //    Assert.AreEqual(Core.Constants.DefaultKeys.InvoiceStatus.Paid, _invoice.InvoiceStatusKey);
        //    Assert.NotNull(MerchelloContext.Current);


        //    creditCardMethod.RefundPayment(_invoice, payment, _invoice.Total, ccEntry.AsProcessorArgumentCollection());
        //    Console.WriteLine("invoice #: {0}", _invoice.InvoiceNumber);
        //}
    }
}

