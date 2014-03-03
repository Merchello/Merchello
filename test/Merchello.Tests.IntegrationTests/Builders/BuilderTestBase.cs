using System;
using System.Collections.Generic;
using System.Web.Configuration;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Gateways.Shipping;
using Merchello.Core.Models;
using Merchello.Core.Models.Interfaces;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Sales;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web.Workflow;
using NUnit.Framework;
using Umbraco.Core;
using System.Linq;
using Merchello.Web;

namespace Merchello.Tests.IntegrationTests.Builders
{

    public class BuilderTestBase : DatabaseIntegrationTestBase
    {
        protected IItemCache ItemCache;
        protected ICustomerBase Customer;
        protected SalePreparationBase SalePreparationMock;
        protected IAddress BillingAddress;
        protected IBasket Basket;
        protected const int ProductCount = 5;
        protected const decimal WeightPerProduct = 3;
        protected const decimal PricePerProduct = 5;
        
        [TestFixtureSetUp]
        public override void FixtureSetup()
        {
            base.FixtureSetup();

            MerchelloContext = new MerchelloContext(new ServiceContext(new PetaPocoUnitOfWorkProvider()),
                new CacheHelper(new NullCacheProvider(),
                    new NullCacheProvider(),
                    new NullCacheProvider()));
            
            
        }

        [SetUp]
        public void Init()
        {
            Customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            Basket = Web.Workflow.Basket.GetBasket(MerchelloContext, Customer);

            var odd = true;
            for (var i = 0; i < ProductCount; i++)
            {
                
                var product = PreTestDataWorker.MakeExistingProduct(true, WeightPerProduct, PricePerProduct);
                product.AddToCatalogInventory(PreTestDataWorker.WarehouseCatalog);
                product.CatalogInventories.First().Count = odd ? 1 : 0;
                product.TrackInventory = true;
                PreTestDataWorker.ProductService.Save(product);
                Basket.AddItem(product, 2);

                odd = !odd;
            }

            BillingAddress = new Address()
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
            PreTestDataWorker.DeleteAllInvoices();

            Customer.ExtendedData.AddAddress(BillingAddress, AddressType.Billing);
            ItemCache = new Core.Models.ItemCache(Customer.EntityKey, ItemCacheType.Checkout);

            PreTestDataWorker.ItemCacheService.Save(ItemCache);

            foreach (var item in Basket.Items)
            {
                ItemCache.AddItem(item.AsLineItemOf<ItemCacheLineItem>());
            }


            // setup the checkout
            SalePreparationMock = new SalePreparationMock(MerchelloContext, ItemCache, Customer);

            // add the shipment rate quote
            var shipment = Basket.PackageBasket(MerchelloContext, BillingAddress).First();
            var shipRateQuote = new ShipmentRateQuote(shipment, new ShipMethod(Guid.NewGuid(), Guid.NewGuid())
            {
                Name = "Unit test rate quote",
                ServiceCode = "Test1"
            })
            {
                Rate = 5M
            };

            //_checkoutMock.ItemCache.Items.Add(shipRateQuote.AsLineItemOf<InvoiceLineItem>());
            SalePreparationMock.SaveShipmentRateQuote(shipRateQuote);
        }

        protected IMerchelloContext MerchelloContext { get; private set; }
    }
}