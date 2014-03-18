using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways.Payment;
using Merchello.Core.Models;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;
using Constants = Merchello.Core.Constants;

namespace Merchello.Tests.IntegrationTests.Payments
{
    [TestFixture]
    public class PaymentProcessingTests : DatabaseIntegrationTestBase
    {
        private IAddress _address;
        private IInvoice _invoice;
        private IMerchelloContext _merchelloContext;
        private Guid _paymentMethodKey;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();
          
            PreTestDataWorker.DeleteAllPaymentMethods();
            PreTestDataWorker.DeleteAllShipCountries();

            _merchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));

            var defaultCatalog = PreTestDataWorker.WarehouseService.GetDefaultWarehouse().WarehouseCatalogs.FirstOrDefault();
            if (defaultCatalog == null) Assert.Ignore("Default WarehouseCatalog is null");

            var us = _merchelloContext.Services.StoreSettingService.GetCountryByCode("US");
            var usCountry = new ShipCountry(defaultCatalog.Key, us);
            ((ServiceContext)_merchelloContext.Services).ShipCountryService.Save(usCountry);

            #region Settings -> Taxation

            var taxProvider = _merchelloContext.Gateways.Taxation.ResolveByKey(Constants.ProviderKeys.Taxation.FixedRateTaxationProviderKey);

            taxProvider.DeleteAllTaxMethods();

            var gwTaxMethod = taxProvider.CreateTaxMethod("US", 0);

            gwTaxMethod.TaxMethod.Provinces["WA"].PercentRateAdjustment = 8.7M;

            taxProvider.SaveTaxMethod(gwTaxMethod);


            #endregion

            _address = new Address()
            {
                Address1 = "114 W. Magnolia St.",
                Address2 = "Suite 504",
                Locality = "Bellingham",
                Region = "WA",
                CountryCode = "US"

            };

            var gateway = _merchelloContext.Gateways.Payment.GetAllGatewayProviders().FirstOrDefault();
            var provider = _merchelloContext.Gateways.Payment.ResolveByKey(gateway.Key);
            var method = provider.CreatePaymentMethod("Cash", "Cash Payments");
            provider.SavePaymentMethod(method);

            _paymentMethodKey = method.PaymentMethod.Key;
        }
        
        [SetUp]
        public void Init()
        {
            
            PreTestDataWorker.DeleteAllInvoices();
            _invoice = MockInvoiceDataMaker.InvoiceForInserting(_address, 150);
            PreTestDataWorker.InvoiceService.Save(_invoice);
        }

        /// <summary>
        /// Test confirms that a payment can be authorized
        /// </summary>
        [Test]
        public void Can_Authorize_Payment_On_Invoice()
        {
            //// Arrange
            
            //// Act
            var authorized = _invoice.AuthorizePayment(_merchelloContext, _paymentMethodKey, new ProcessorArgumentCollection());

            //// Assert
            Assert.IsTrue(authorized.Payment.Success);
            Assert.IsTrue(authorized.Payment.Result.HasIdentity);
            Assert.IsTrue(authorized.Payment.Result.AppliedPayments(_merchelloContext).Any());
            Assert.AreEqual(Constants.DefaultKeys.InvoiceStatus.Unpaid, _invoice.InvoiceStatusKey);
        }

        /// <summary>
        /// Test verifies that a collection of payments can be retrieved for an invoice
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Collection_Of_Payments_For_An_Invoice()
        {
            //// Arrange
            var authorized = _invoice.AuthorizePayment(_merchelloContext, _paymentMethodKey, new ProcessorArgumentCollection());

            //// Act
            var payments = _invoice.Payments(_merchelloContext);

            //// Assert
            Assert.NotNull(payments);
            Assert.AreEqual(1, payments.Count());
        }

        /// <summary>
        /// Test verifies that unique payments are retrieved when more than one "applied payment" exists
        /// </summary>
        [Test]
        public void Retrieving_A_Collection_Of_Payments_For_An_Invoice_After_Capture_Returns_Unique_Payments()
        {
            //// Arrange
            var authorized = _invoice.AuthorizePayment(_merchelloContext, _paymentMethodKey, new ProcessorArgumentCollection());
            var caputered = _invoice.CapturePayment(_merchelloContext, authorized.Payment.Result, _paymentMethodKey, _invoice.Total, new ProcessorArgumentCollection());

            //// Act
            var payments = _invoice.Payments(_merchelloContext);

            //// Assert
            Assert.NotNull(payments);
            Assert.AreEqual(1, payments.Count());
        }
        
        /// <summary>
        /// Test confirms that a payment can be captured
        /// </summary>
        [Test]
        public void Can_Capture_A_Payment_For_An_Invoice()
        {
            //// Arrange
            var payment = _invoice.AuthorizePayment(_merchelloContext, _paymentMethodKey,new ProcessorArgumentCollection()).Payment.Result;

            //// Act
            var capture = _invoice.CapturePayment(_merchelloContext, payment, _paymentMethodKey, _invoice.Total, new ProcessorArgumentCollection());

            //// Assert
            Assert.IsTrue(capture.Payment.Success);
            Assert.AreEqual(Constants.DefaultKeys.InvoiceStatus.Paid, _invoice.InvoiceStatusKey);
        }
        
        /// <summary>
        /// Test confirms that a partial payment can be captured for the invoice
        /// </summary>
        [Test]
        public void Can_Capture_A_Partial_Payment_For_An_Invoice()
        {
            //// Arrange
            var payment = _invoice.AuthorizePayment(_merchelloContext, _paymentMethodKey, new ProcessorArgumentCollection()).Payment.Result;

            //// Act
            var capture = _invoice.CapturePayment(_merchelloContext, payment, _paymentMethodKey, _invoice.Total / 2, new ProcessorArgumentCollection());

            //// Assert
            Assert.IsTrue(capture.Payment.Success);
            Assert.AreEqual(Constants.DefaultKeys.InvoiceStatus.Partial, _invoice.InvoiceStatusKey);
        }

        /// <summary>
        /// Test confirms that a payment can be authorized and captured
        /// </summary>
        [Test]
        public void Can_Authorize_And_Capture_A_Payment_For_An_Invoice()
        {
            //// Arrange
            
            //// Act   
            var authCapture = _invoice.AuthorizeCapturePayment(_merchelloContext, _paymentMethodKey, new ProcessorArgumentCollection());

            //// Assert
            Assert.IsTrue(authCapture.Payment.Success);
            Assert.AreEqual(Constants.DefaultKeys.InvoiceStatus.Paid, _invoice.InvoiceStatusKey);
        }
    }
}