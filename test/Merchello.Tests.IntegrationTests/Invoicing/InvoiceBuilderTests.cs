using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Builders;
using Merchello.Core.Cache;
using Merchello.Core.Checkout;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web;
using Merchello.Web.Workflow;
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
        private IAddress _billingAddress;
        private IBasket _basket;
        private const decimal ProductCount = 5;
        private const decimal WeightPerProduct = 3;
        private const decimal PricePerProduct = 5;

        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            _merchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));
        }

        [SetUp]
        public void Init()
        {
            _customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            _basket = Basket.GetBasket(_merchelloContext, _customer);

            for (var i = 0; i < ProductCount; i++) _basket.AddItem(PreTestDataWorker.MakeExistingProduct(true, WeightPerProduct, PricePerProduct));
            
            _billingAddress = new Address()
                {
                    Name = "Out there",
                    Address1 = "some street",
                    Locality = "some city",
                    Region = "ST",
                    PostalCode = "98225",
                    CountryCode = "US"
                };

            var origin = new Address()
                {
                    Name = "Somewhere",
                    Address1 = "origin street",
                    Locality = "origin city",
                    Region = "ST",
                    PostalCode = "98225",
                    CountryCode = "US"
                };



            _customer.ExtendedData.AddAddress(_billingAddress, AddressType.Billing);
            _itemCache = new Core.Models.ItemCache(_customer.EntityKey, ItemCacheType.Checkout);

            foreach(var item in _basket.Items) _itemCache.AddItem(item as ItemCacheLineItem);


            // setup the checkout
            _checkoutMock = new CheckoutMock(_merchelloContext, _itemCache, _customer);

            // add the shipment rate quote
            var shipment = _basket.PackageBasket(_merchelloContext, _billingAddress).First();
            var shipRateQuote = new ShipmentRateQuote(shipment, new ShipMethod(Guid.NewGuid(), Guid.NewGuid())
            {
                Name = "Unit test rate quote",
                ServiceCode = "Test1"
            })
            {
                Rate = 5M
            };

            _checkoutMock.ItemCache.Items.Add(shipRateQuote.AsLineItemOf<InvoiceLineItem>());
        }

        /// <summary>
        /// Test verifies that the InvoiceBuilder can be instantiated with 4 tasks from the configuration file
        /// </summary>
        [Test]
        public void Can_Create_The_Default_Invoice_Builder_With_4_Tasks()
        {
            //// Arrange
            const int taskCount = 4;

            //// Act
            var invoiceBuild = new InvoiceBuilder(_checkoutMock);

            //// Assert
            Assert.NotNull(invoiceBuild);
            Assert.AreEqual(taskCount, invoiceBuild.TaskCount);
        }

        /// <summary>
        /// Test confirms that an address can be added to an invoice
        /// </summary>
        [Test]
        public void Can_Add_An_Address_To_An_Invoice()
        {
            //// Arrange
            var expected = _billingAddress;
            var invoiceBuilder = new InvoiceBuilder(_checkoutMock);

            //// Arrange
            var attempt = invoiceBuilder.BuildInvoice();
            Assert.IsTrue(attempt.Success);
            var invoice = attempt.Result;

            //// Assert
            Assert.NotNull(invoice);
            Assert.IsTrue(((Address)expected).Equals(invoice.GetBillingAddress()));
        }

        /// <summary>
        /// Test verifies that an invoice contains product and shipment line items
        /// </summary>
        [Test]
        public void Can_Verify_Invoice_Contains_Product_And_Shipment_LineItems()
        {
            //// Arrange
            const decimal expectedProducts = ProductCount;
            const int expectedShipments = 1;
            var invoiceBuilder = new InvoiceBuilder(_checkoutMock);

            //// Act
            var attempt = invoiceBuilder.BuildInvoice();
            Assert.IsTrue(attempt.Success);
            var invoice = attempt.Result;

            //// Assert
            Assert.NotNull(invoice, "Invoice is null");
            Assert.IsTrue(invoice.Items.Any(), "Invoice does not have any items");
            Assert.AreEqual(expectedProducts, invoice.Items.Count(x => x.LineItemType == LineItemType.Product), "Count of products does not match expected");
            Assert.AreEqual(expectedShipments, invoice.Items.Count(x => x.LineItemType == LineItemType.Shipping), "Count of shipments does not match expected");
            
        }

    }
}