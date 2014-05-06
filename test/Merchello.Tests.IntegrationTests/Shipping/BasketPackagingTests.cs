using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Cache;
using Merchello.Core.Models;
using Merchello.Core.Persistence.UnitOfWork;
using Merchello.Core.Services;
using Merchello.Core.Strategies.Packaging;
using Merchello.Tests.IntegrationTests.TestHelpers;
using Merchello.Web;
using Merchello.Web.Workflow;
using NUnit.Framework;
using Umbraco.Core;

namespace Merchello.Tests.IntegrationTests.Shipping
{
    public class BasketPackagingTests : DatabaseIntegrationTestBase
    {
        
        private ICustomerBase _customer;
        private IBasket _basket;
        private const int ProductCount = 4;

        [SetUp]
        public void Init()
        {
            PreTestDataWorker.DeleteAllProducts();
            PreTestDataWorker.DeleteAllItemCaches();


            _customer = PreTestDataWorker.MakeExistingAnonymousCustomer();
            _basket = Basket.GetBasket(MerchelloContext, _customer);

            for(var i = 0; i < ProductCount; i++) _basket.AddItem(PreTestDataWorker.MakeExistingProduct());
            _basket.AddItem(PreTestDataWorker.MakeExistingProduct(false));

            Basket.Save(MerchelloContext, _basket);
        }

        /// <summary>
        /// Test verifies that the <see cref="ShippableProductVisitor"/> returns only shippable products
        /// </summary>
        [Test]
        public void ShippableProductVisitor_Returns_Only_Shippable_Products()
        {
            //// Arrange
            const int expected = 4;
            var visitor = new ShippableProductVisitor();

            //// Act
            _basket.Accept(visitor);

            //// Assert
            Assert.AreEqual(expected, visitor.ShippableItems.Count());
        }

        /// <summary>
        /// Test verifies that the BasketPackagingStrategy returns a collection containing a single shipment
        /// </summary>
        [Test]
        public void Default_BasketPackagingStrategy_Returns_A_Shipment()
        {
            //// Arrange
            var destination = new Address()
            {
                Name = "San Diego Zoo",
                Address1 = "2920 Zoo Dr",
                Locality = "San Diego",
                Region = "CA",
                PostalCode = "92101",
                CountryCode = "US"
            };
            
            //// Act
            var strategy = new DefaultWarehousePackagingStrategy(MerchelloContext, _basket.Items, destination, Guid.NewGuid());
            var shipments = strategy.PackageShipments();

            //// Assert
            Assert.NotNull(shipments);
            Assert.AreEqual(1, shipments.Count());

        }

        /// <summary>
        /// Test verifies that the extension method
        /// </summary>
        [Test]
        public void Can_Package_A_Basket_Using_Basket_ExtensionMethod()
        {
            //// Arrange
            var destination = new Address()
            {
                Name = "San Diego Zoo",
                Address1 = "2920 Zoo Dr",
                Locality = "San Diego",
                Region = "CA",
                PostalCode = "92101",
                CountryCode = "US"
            };
            
            //// Act
            var shipments = _basket.PackageBasket(MerchelloContext, destination);

            //// Assert
            Assert.NotNull(shipments);
            Assert.AreEqual(1, shipments.Count());
        }

    }
}