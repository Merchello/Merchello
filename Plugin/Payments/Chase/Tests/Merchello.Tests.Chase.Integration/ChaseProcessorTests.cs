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
using Constants = Merchello.Plugin.Payments.Chase.Constants;

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
        public void TestCaseNumber_1_Section_A_A_And_B()
        {
            //Arrange            
            const string cardType = "ChaseNet";
            const string card = "4011361100000012";
            const string cardCode = "";
            const string postalCode = "22222";
            const int amount = 25;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.VoidPayment(_invoice, result.Payment.Result, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);
            
            Assert.AreEqual("0", result2.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.VoidProcStatus));

            // Log Results for certification      
            TestHelper.LogInformation("Test 1A Section A", result);
            TestHelper.LogInformation("Test 1B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_2_Section_A_A_And_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 30;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);  

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.VoidPayment(_invoice, result.Payment.Result, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.AreEqual("0", result2.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.VoidProcStatus));

            // Log Results for certification    
            TestHelper.LogInformation("Test 2A Section A", result);
            TestHelper.LogInformation("Test 2B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_3_Section_A()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "";
            const string postalCode = "L6L2X9";
            const decimal amount = 38.01M;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                     
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                                     
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());

            // Assert                                                                                                 
            Assert.IsFalse(result.Payment.Success);
            Assert.AreEqual("05", result.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode).Split(',')[1]);

            // Log Results for certification
            TestHelper.LogInformation("Test 3 Section A", result);
        }                              

        [Test]
        public void TestCaseNumber_4_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "222";
            const string postalCode = "22222";
            const int amount = 85;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                   
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Assert                 
            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification        
            TestHelper.LogInformation("Test 4A Section A", result);
            TestHelper.LogInformation("Test 4B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_5_Section_A()
        {
            //Arrange       
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "";
            const string postalCode = "66666";
            const int amount = 0;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                   
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 5 Section A", result);
        }

        [Test]
        public void TestCaseNumber_6_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "555";
            const string postalCode = "11111";
            const int amount1 = 125;
            const int amount2 = 75;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount1 + amount2;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount1, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount2, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 6A Section A", result);
            TestHelper.LogInformation("Test 6B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_7_Section_A_A_And_B()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "";
            const string postalCode = "L6L2X9";
            const int amount = 41;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.VoidPayment(_invoice, result.Payment.Result, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.AreEqual("0", result2.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.VoidProcStatus));

            // Log Results for certification    
            TestHelper.LogInformation("Test 7a Section A", result);
            TestHelper.LogInformation("Test 7b Section A", result2);
        }

        [Test]
        public void TestCaseNumber_8_Section_A()
        {
            //Arrange       
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "666";
            const string postalCode = "88888";
            const decimal amount = 11.02M;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsFalse(result.Payment.Success);
            Assert.IsFalse(result.ApproveOrderCreation);
            Assert.AreEqual("01", result.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.AuthorizationTransactionCode).Split(',')[1]);

            // Log Results for certification
            TestHelper.LogInformation("Test 8 Section A", result);
        }                             

        [Test]
        public void TestCaseNumber_9_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "666";
            const string postalCode = "L6L2X9";
            const int amount = 75;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 9A Section A", result);
            TestHelper.LogInformation("Test 9B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_10_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "222";
            const string postalCode = "55555";
            const int amount1 = 100;
            const int amount2 = 60;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount1 + amount2;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount1, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount2, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 10A Section A", result);
            TestHelper.LogInformation("Test 10B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_11_Section_A()
        {
            //Arrange       
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "";
            const string postalCode = "88888";
            const int amount = 0;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                   
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 11 Section A", result);
        }                             

        [Test]
        public void TestCaseNumber_24_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "VISA";
            const string card = "4055011111111111";
            const string cardCode = "333";
            const string postalCode = "22222";
            const int amount = 90;
            const int taxAmount = 10;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");
            
            _invoice.BillToPostalCode = postalCode;
            
            // Set the total value for the invoice.
            _invoice.Total = amount + taxAmount;
            _invoice.PoNumber = "PW0001";
            
            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount - taxAmount, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Tax, "Item 2", "I2", 1, taxAmount, extendedData);
            
            _invoice.Items.Add(l1);
            _invoice.Items.Add(l2);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            
            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 24A Section A", result);
            TestHelper.LogInformation("Test 24B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_25_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "VISA";
            const string card = "4055011111111111";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 110;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 25A Section A", result);
            TestHelper.LogInformation("Test 25B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_26_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "MasterCard";
            const string card = "5405222222222226";
            const string cardCode = "222";
            const string postalCode = "22222";
            const int amount = 110;
            const int taxAmount = 10;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount + taxAmount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount - taxAmount, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Tax, "Item 2", "I2", 1, taxAmount, extendedData);

            _invoice.Items.Add(l1);
            _invoice.Items.Add(l2);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 26A Section A", result);
            TestHelper.LogInformation("Test 26B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_27_Section_A_A_And_B()
        {
            //Arrange       
            const string cardType = "MasterCard";
            const string card = "5405222222222226";
            const string cardCode = "555";
            const string postalCode = "55555";
            const int amount = 130;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.CapturePayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert                 
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);
            Assert.IsTrue(result2.ApproveOrderCreation);

            // Log Results for certification
            TestHelper.LogInformation("Test 27A Section A", result);
            TestHelper.LogInformation("Test 27B Section A", result2);
        }

        [Test]
        public void TestCaseNumber_1_Section_B_A_And_B()
        {
            //Arrange            
            const string cardType = "ChaseNet";
            const string card = "4011361100000012";
            const string cardCode = "";
            const string postalCode = "22222";
            const int amount = 25;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.VoidPayment(_invoice, result.Payment.Result, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.AreEqual("0", result2.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.VoidProcStatus));

            // Log Results for certification       
            TestHelper.LogInformation("Test 1A Section B", result);
            TestHelper.LogInformation("Test 1B Section B", result2);
        }

        [Test]
        public void TestCaseNumber_2_Section_B_A_And_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 30;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.VoidPayment(_invoice, result.Payment.Result, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.AreEqual("0", result2.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.VoidProcStatus));

            // Log Results for certification       
            TestHelper.LogInformation("Test 2A Section B", result);
            TestHelper.LogInformation("Test 2B Section B", result2);
        }

        [Test]
        public void TestCaseNumber_3_Section_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "";
            const string postalCode = "11111";
            const decimal amount = 38.01M;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsFalse(result.Payment.Success);
            Assert.IsFalse(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 3 Section B", result);
        }                             

        [Test]                  
        public void TestCaseNumber_4_Section_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "222";
            const string postalCode = "22222";
            const decimal amount = 85;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 4 Section B", result);
        }                            

        [Test]
        public void TestCaseNumber_5_Section_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "555";
            const string postalCode = "11111";
            const decimal amount = 125;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 5 Section B", result);
        }                              

        [Test]
        public void TestCaseNumber_6_Section_B_A_And_B()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "";
            const string postalCode = "L6L2X9";
            const int amount = 41;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.VoidPayment(_invoice, result.Payment.Result, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.AreEqual("0", result2.Payment.Result.ExtendedData.GetValue(Constants.ExtendedDataKeys.VoidProcStatus));

            // Log Results for certification       
            TestHelper.LogInformation("Test 6A Section B", result);
            TestHelper.LogInformation("Test 6B Section B", result2);
        }

        [Test]
        public void TestCaseNumber_7_Section_B()
        {                                                                                                                                                                     
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "666";
            const string postalCode = "88888";
            const decimal amount = 11.02M;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsFalse(result.Payment.Success);
            Assert.IsFalse(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 7 Section B", result);
        }                              

        [Test]
        public void TestCaseNumber_8_Section_B()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "666";
            const string postalCode = "L6L2X9";
            const int amount = 70;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 8 Section B", result);
        }                             

        [Test]
        public void TestCaseNumber_9_Section_B()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "222";
            const string postalCode = "55555";
            const int amount = 100;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 9A Section B", result);
        }                             

        [Test]
        public void TestCaseNumber_25_Section_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4055011111111111";
            const string cardCode = "333";
            const string postalCode = "22222";
            const int amount = 90;
            const int taxAmount = 10;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount + taxAmount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Tax, "Item 2", "I2", 1, taxAmount, extendedData);

            _invoice.Items.Add(l1);
            _invoice.Items.Add(l2);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, _invoice.Total, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 25 Section B", result);
        }                              

        [Test]
        public void TestCaseNumber_26_Section_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4055011111111111";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 110;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 26 Section B", result);
        }                              

        [Test]
        public void TestCaseNumber_27_Section_B()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5405222222222226";
            const string cardCode = "222";
            const string postalCode = "22222";
            const int amount = 120;
            const int taxAmount = 10;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount + taxAmount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);
            var l2 = new InvoiceLineItem(LineItemType.Tax, "Item 2", "I2", 1, taxAmount, extendedData);

            _invoice.Items.Add(l1);
            _invoice.Items.Add(l2);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, _invoice.Total, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 27 Section B", result);
        }

        [Test]
        public void TestCaseNumber_28_Section_B()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5405222222222226";
            const string cardCode = "555";
            const string postalCode = "55555";
            const int amount = 130;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, _invoice.Total, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 28 Section B", result);
        } 

        [Test]
        public void TestCaseNumber_1_Section_C_Visa_AuthOnly()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 30;
            const string cavv = "AAABAIcJIoQDIzAgVAklAAAAAAA=";
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 1 Section C Visa Auth Only", result);
        }                             

        [Test]
        public void TestCaseNumber_2_Section_C_Visa_AuthOnly()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "222";
            const string postalCode = "L6L2X9";
            const int amount = 38;
            const string cavv = "AAABCCMRERI0VniQEhERAAAAAAA=";
            //const string cavv = "AAABCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "BwAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA="
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 2 Section C Visa Auth Only", result);
        }

        [Test]
        public void TestCaseNumber_3_Section_C_Visa_AuthOnly()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "333";
            const string postalCode = "44444";
            const int amount = 41;
            const string cavv = "AAABCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "BwAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA="
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 3 Section C Visa Auth Only", result);
        }

        [Test]
        public void TestCaseNumber_4_Section_C_Visa_AuthOnly()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "666";
            const string postalCode = "88888";
            const int amount = 11;
            const string cavv = "BwAQCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA="
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 4 Section C Visa Auth Only", result);
        }
        [Test]
        public void TestCaseNumber_5_Section_C_Visa_AuthOnly()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "111";
            const string postalCode = "55555";
            const int amount = 1055;
            const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA=";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 5 Section C Visa Auth Only", result);
        }
        [Test]
        public void TestCaseNumber_6_Section_C_Visa_AuthOnly()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "555";
            const string postalCode = "666666";
            const int amount = 75;
            const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA=";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 6 Section C Visa Auth Only", result);
        }
        [Test]
        public void TestCaseNumber_7_Section_C_Visa_AuthOnly()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "666";
            const string postalCode = "11111";
            const int amount = 40;
            const string cavv = "";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;                                              

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 7 Section C Visa Auth Only", result);
        }

        [Test]
        public void TestCaseNumber_1_Section_C_MasterCard_AuthOnly()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 30;
            const string aav = "hsjuQljfl86bAAAAAACm9zU6aqY=";
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = aav;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 1 Section C MasterCard Auth Only", result);
        }

        [Test]
        public void TestCaseNumber_2_Section_C_MasterCard_AuthOnly()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "222";
            const string postalCode = "L6L2X9";
            const int amount = 38;
            const string aav = "";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = aav;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 2 Section C MasterCard Auth Only", result);
        }

        [Test]
        public void TestCaseNumber_3_Section_C_MasterCard_AuthOnly()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "333";
            const string postalCode = "44444";
            const int amount = 41;
            const string aav = "Asju1ljfl86bAAAAAACm9zU6aqY";
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = aav;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 3 Section C MasterCard Auth Only", result);
        }

        [Test]
        public void TestCaseNumber_1_Section_C_Visa_AuthCapture()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 30;
            const string cavv = "AAABAIcJIoQDIzAgVAklAAAAAAA=";
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 1 Section C Visa Auth Capture", result);
        }

        [Test]
        public void TestCaseNumber_2_Section_C_Visa_AuthCapture()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "222";
            const string postalCode = "L6L2X9";
            const int amount = 38;
            const string cavv = "AAABCCMRERI0VniQEhERAAAAAAA=";
            //const string cavv = "AAABCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "BwAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA="
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                          
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 2 Section C Visa Auth Capture", result);
        }

        [Test]
        public void TestCaseNumber_3_Section_C_Visa_AuthCapture()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "333";
            const string postalCode = "44444";
            const int amount = 41;
            const string cavv = "AAABCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "BwAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA="
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                         
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 3 Section C Visa Auth Capture", result);
        }

        [Test]
        public void TestCaseNumber_4_Section_C_Visa_AuthCapture()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "666";
            const string postalCode = "88888";
            const int amount = 11;
            const string cavv = "BwAQCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA="
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA="
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 4 Section C Visa Auth Capture", result);
        }
        [Test]
        public void TestCaseNumber_5_Section_C_Visa_AuthCapture()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "111";
            const string postalCode = "55555";
            const int amount = 1055;
            const string cavv = "CAAQCZkRERI0VniQEhERAAAAAAA=";
            //const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA=";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                          
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 5 Section C Visa Auth Capture", result);
        }
        [Test]
        public void TestCaseNumber_6_Section_C_Visa_AuthCapture()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "555";
            const string postalCode = "666666";
            const int amount = 75;
            const string cavv = "BwABA4kRERI0VniQEhERAAAAAAA=";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                       
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 6 Section C Visa Auth Capture", result);
        }
        [Test]
        public void TestCaseNumber_7_Section_C_Visa_AuthCapture()
        {
            //Arrange            
            const string cardType = "Visa";
            const string card = "4788250000028291";
            const string cardCode = "666";
            const string postalCode = "11111";
            const int amount = 40;
            const string cavv = "";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = cavv;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                         
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 7 Section C Visa Auth Capture", result);
        }

        [Test]
        public void TestCaseNumber_1_Section_C_MasterCard_AuthCapture()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "111";
            const string postalCode = "11111";
            const int amount = 30;
            const string aav = "hsjuQljfl86bAAAAAACm9zU6aqY=";
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = aav;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                       
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 1 Section C MasterCard Auth Capture", result);
        }

        [Test]
        public void TestCaseNumber_2_Section_C_MasterCard_AuthCapture()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "222";
            const string postalCode = "L6L2X9";
            const int amount = 38;
            const string aav = "";
            const string eci = "6";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = aav;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                       
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 2 Section C MasterCard Auth Capture", result);
        }

        [Test]
        public void TestCaseNumber_3_Section_C_MasterCard_AuthCapture()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "333";
            const string postalCode = "44444";
            const int amount = 41;
            const string aav = "Asju1ljfl86bAAAAAACm9zU6aqY";
            const string eci = "5";

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            ccEntry.AuthenticationVerification = aav;
            ccEntry.AuthenticationVerificationEci = eci;

            // Act                                                                                         
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            // Assert

            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification       
            TestHelper.LogInformation("Test 3 Section C MasterCard Auth Capture", result);
        }

        [Test]
        public void TestCaseNumber_1_Section_D_A_And_B()
        {
            //Arrange            
            const string cardType = "ChaseNet";
            const string card = "4011361100000012";
            const string cardCode = "";
            const string postalCode = "22222";
            const int amount = 25;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.RefundPayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);

            // Log Results for certification      
            TestHelper.LogInformation("Test 1A Section D", result);
            TestHelper.LogInformation("Test 1B Section D", result2);
        }

        [Test]
        public void TestCaseNumber_2_Section_D_A_And_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "";
            const string postalCode = "22222";
            const int amount = 12;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.RefundPayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);

            // Log Results for certification      
            TestHelper.LogInformation("Test 2A Section D", result);
            TestHelper.LogInformation("Test 2B Section D", result2);
        }

        [Test]
        public void TestCaseNumber_3_Section_D()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "";
            const string postalCode = "22222";
            const int amount = 11;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification      
            TestHelper.LogInformation("Test 3 Section D", result);
        }                              

        [Test]
        public void TestCaseNumber_7_Section_D_A_And_B()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "";
            const string postalCode = "22222";
            const int amount = 25;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.RefundPayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);

            // Log Results for certification      
            TestHelper.LogInformation("Test 7A Section D", result);
            TestHelper.LogInformation("Test 7B Section D", result2);
        }

        [Test]
        public void TestCaseNumber_8_Section_D_A_And_B()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "";
            const string postalCode = "22222";
            const int amount = 26;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            // Act                                                                                                        
            var result = _payment.AuthorizePayment(_invoice, ccEntry.AsProcessorArgumentCollection());
            var result2 = _payment.RefundPayment(_invoice, result.Payment.Result, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result2.Payment.Success);

            // Log Results for certification      
            TestHelper.LogInformation("Test 8A Section D", result);
            TestHelper.LogInformation("Test 8B Section D", result2);
        }

        [Test]
        public void TestCaseNumber_1_Section_F()
        {
            //Arrange            
            const string cardType = "ChaseNet";
            const string card = "4011361100000012";
            const string cardCode = "";
            const string postalCode = "";
            const int amount = 20;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");
            
            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);


            /////// 1a

            // Act                                                                                                        
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result.Payment.Success);

            // Log Results for certification      
            TestHelper.LogInformation("Test 1a Section F", result);


            /////// 1b

            // Act                                                                                                        
            result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            Assert.IsTrue(result.Payment.Success);

            // Log Results for certification      
            TestHelper.LogInformation("Test 1b Section F", result);

        }                            

        [Test]
        public void TestCaseNumber_2_Section_F()
        {
            //Arrange            
            const string cardType = "VISA";
            const string card = "4788250000028291";
            const string cardCode = "";
            const string postalCode = "";
            const int amount = 10;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);
            
            /////// 2a
            
            // Act                                                                                            
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification      
            TestHelper.LogInformation("Test 2a Section F", result);

            /////// 2b
            // Act                                                                                            
            result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification      
            TestHelper.LogInformation("Test 2b Section F", result);

        }

        [Test]
        public void TestCaseNumber_3_Section_F()
        {
            //Arrange            
            const string cardType = "MasterCard";
            const string card = "5454545454545454";
            const string cardCode = "";
            const string postalCode = "";
            const int amount = 15;

            // Setup extended data for invoice
            var extendedData = new ExtendedDataCollection();

            // this is typically added automatically in the checkout workflow
            extendedData.SetValue(Core.Constants.ExtendedDataKeys.CurrencyCode, "USD");

            _invoice.BillToPostalCode = postalCode;

            // Set the total value for the invoice.
            _invoice.Total = amount;

            // make up some line items for the invoice                                                    
            var l1 = new InvoiceLineItem(LineItemType.Product, "Item 1", "I1", 1, amount, extendedData);

            _invoice.Items.Add(l1);

            var creditCardMethod = Provider.GetPaymentGatewayMethodByPaymentCode("CreditCard");
            Assert.NotNull(creditCardMethod);

            var ccEntry = TestHelper.GetCreditCardFormData(cardType, card, cardCode);

            ////////// 3a
            
            // Act                                                                                            
            var result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification      
            TestHelper.LogInformation("Test 3a Section F", result);

            ////////// 3b

            // Act                                                                                            
            result = _payment.AuthorizeCapturePayment(_invoice, amount, ccEntry.AsProcessorArgumentCollection());

            // Assert
            Assert.IsTrue(result.Payment.Success);
            Assert.IsTrue(result.ApproveOrderCreation);

            // Log Results for certification      
            TestHelper.LogInformation("Test 3b Section F", result);
        }            
    }
}
