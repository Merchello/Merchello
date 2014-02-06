using System;
using Merchello.Core;
using Merchello.Core.Builders;
using Merchello.Core.Cache;
using Merchello.Core.Checkout;
using Merchello.Core.Models;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Invoicing
{

    [TestFixture]
    [Category("Builders")]
    public class InvoiceBuilderTests : DatabaseIntegrationTestBase
    {
        private IMerchelloContext _merchelloContext;
        private IItemCache _itemCache;
        private ICustomerBase _customer;
        private CheckoutBase _checkoutMock;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {

            _merchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));
        }


        [SetUp]
        public void Init()
        {
            _customer = MockAnonymousCustomerDataMaker.AnonymousCustomerForInserting();
            _customer.Key = Guid.NewGuid();

            _itemCache = new Core.Models.ItemCache(_customer.EntityKey, ItemCacheType.Checkout);
            _checkoutMock = new CheckoutMock(_merchelloContext, _itemCache, _customer);
        }

        [Test]
        public void Can_Create_The_Default_Invoice_Builder_Having_3_Tasks()
        {
            //// Arrange
            var taskCount = 3;

            //// Act
            var invoiceBuild = new InvoiceBuilder(_checkoutMock);

            //// Assert
            Assert.NotNull(invoiceBuild);
            Assert.AreEqual(taskCount, invoiceBuild.TaskCount);
        }
    }
}