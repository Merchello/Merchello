using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Builders;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Sales;
using Merchello.Tests.Base.DataMakers;
using Merchello.Web;
using Merchello.Web.Workflow;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Builders
{

    [TestFixture]
    [Category("Builders")]
    public class InvoiceBuilderTests : BuilderTestBase
    {
        private IItemCache _itemCache;
        private ICustomerBase _customer;
        private SalesManagerBase _salesManagerMock;
        private IAddress _billingAddress;
        private IBasket _basket;
        private const decimal ProductCount = 5;
        private const decimal WeightPerProduct = 3;
        private const decimal PricePerProduct = 5;


        [SetUp]
        public void Init()
        {
            _customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            _basket = Basket.GetBasket(MerchelloContext, _customer);

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



            PreTestDataWorker.DeleteAllItemCaches();

            _customer.ExtendedData.AddAddress(_billingAddress, AddressType.Billing);
            _itemCache = new Core.Models.ItemCache(_customer.EntityKey, ItemCacheType.Checkout);
            
            PreTestDataWorker.ItemCacheService.Save(_itemCache);

            foreach (var item in _basket.Items)
            {
                _itemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
            }


            // setup the checkout
            _salesManagerMock = new SalesManagerMock(MerchelloContext, _itemCache, _customer);

            // add the shipment rate quote
            var shipment = _basket.PackageBasket(MerchelloContext, _billingAddress).First();
            var shipRateQuote = new ShipmentRateQuote(shipment, new ShipMethod(Guid.NewGuid(), Guid.NewGuid())
            {
                Name = "Unit test rate quote",
                ServiceCode = "Test1"
            })
            {
                Rate = 5M
            };

            //_checkoutMock.ItemCache.Items.Add(shipRateQuote.AsLineItemOf<InvoiceLineItem>());
            _salesManagerMock.SaveShipmentRateQuote(shipRateQuote);
        }

        /// <summary>
        /// Test verifies that the InvoiceBuilder can be instantiated with 3 tasks from the configuration file
        /// </summary>
        [Test]
        public void Can_Create_The_Default_Invoice_Builder_With_3_Tasks()
        {
            //// Arrange
            const int taskCount = 3;

            //// Act
            var invoiceBuild = new InvoiceBuilderChain(_salesManagerMock);

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
            var invoiceBuilder = new InvoiceBuilderChain(_salesManagerMock);

            //// Arrange
            var attempt = invoiceBuilder.Build();
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
            var invoiceBuilder = new InvoiceBuilderChain(_salesManagerMock);

            //// Act
            var attempt = invoiceBuilder.Build();
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