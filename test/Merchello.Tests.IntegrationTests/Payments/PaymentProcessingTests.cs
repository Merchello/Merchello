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

            _merchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));

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
        /// Test confirms that 
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
            
        }
        
        
    }
}