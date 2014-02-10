using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Builders;
using Merchello.Core.Checkout;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Tests.Base.DataMakers;
using Merchello.Web;
using Merchello.Web.Workflow;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Builders
{
    [TestFixture]
    [Category("Builders")]
    public class TaxationQuoteBuilderTests : BuilderTestBase
    {
        private IItemCache _itemCache;
        private ICustomerBase _customer;
        private IBasket _basket;
        private IAddress _billingAddress;
        private IAddress _originAddress;
        private CheckoutPreparationBase _checkoutPreparationMock;
        private const decimal ProductCount = 5;
        private const decimal WeightPerProduct = 3;
        private const decimal PricePerProduct = 5;

        [TestFixtureSetUp]
        public override void FixtureSetup()
         {
             base.FixtureSetup();

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

             _originAddress = new Address()
             {
                 Name = "Somewhere",
                 Address1 = "origin street",
                 Locality = "origin city",
                 Region = "ST",
                 PostalCode = "98225",
                 CountryCode = "US"
             };

             _customer.ExtendedData.AddAddress(_billingAddress, AddressType.Billing);             

         }

        [SetUp]
        public void Init()
        {
            _itemCache = new Core.Models.ItemCache(_customer.EntityKey, ItemCacheType.Checkout);


            // setup the checkout
            _checkoutPreparationMock = new CheckoutPreparationMock(MerchelloContext, _itemCache, _customer);

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

            _checkoutPreparationMock.ItemCache.Items.Add(shipRateQuote.AsLineItemOf<InvoiceLineItem>());
        }

        ///// <summary>
        ///// Test verifies that the builder instantiates with the number of expected tasks
        ///// </summary>
        //[Test]
        //public void TaxationQuoteBuilder_Instaniates_With_Expected_Number_Of_Tasks()
        //{
        //    //// Arrage
        //    const int expected = 1;
            
        //    //// Act
        //    var builder = new TaxationQuoteBuilderChain();

        //    //// Assert
        //    Assert.AreEqual(expected, builder.TaskCount);
        //}

    }
}