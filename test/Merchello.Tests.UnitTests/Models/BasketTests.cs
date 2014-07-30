using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Tests.Base.DataMakers;
using Merchello.Web;
using Merchello.Web.Models;
using Merchello.Web.Workflow;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Models
{
    [TestFixture]
    public class BasketTests
    {
        private ICustomerBase _customer;
        private IItemCache _itemCache;
        private IProduct _product;

        [SetUp]
        public void Init()
        {
            _customer = new AnonymousCustomer() { Key = Guid.NewGuid() };
            _itemCache = new ItemCache(_customer.Key, ItemCacheType.Basket) {Key = Guid.NewGuid()};

            _product = MockProductDataMaker.MockProductComplete(Guid.NewGuid());            

        }

        /// <summary>
        /// Test verifies an item can be added to a basket
        /// </summary>
        [Test]
        public void Can_Add_A_ProductVariant_To_A_Basket_And_IsEmpty_Returns_False()
        {
            //// Arrange                      
            var basket = Basket.GetBasket(_customer, _itemCache);
            var product = MockProductDataMaker.MockProductComplete(Guid.NewGuid());

            //// Act
            basket.AddItem(product.GetProductVariantForPurchase());

            //// Assert
            Assert.IsTrue(basket.Items.Any());
            Assert.IsFalse(basket.IsEmpty);
        }

        /// <summary>
        /// Test verifies a product can be removed from the basket
        /// </summary>
        [Test]
        public void Can_Add_A_Product_To_The_Basket()
        {
            //// Arrange
            var basket = Basket.GetBasket(_customer, _itemCache);            

            //// Act
            basket.AddItem(_product);

            //// Assert
            Assert.IsTrue(basket.Items.Any());
            Assert.IsFalse(basket.IsEmpty);
        }

        /// <summary>
        /// Test verifies that the basket quantity can be updated
        /// </summary>
        [Test]
        public void Can_Update_The_Quantity_Of_An_Item_In_The_Basket()
        {
            //// Arrange
            var sku = _product.Sku;
            const int count = 10;
            var basket = Basket.GetBasket(_customer, _itemCache);
            basket.AddItem(_product);

            //// Act
            basket.UpdateQuantity(sku, count);

            //// Assert
            Assert.AreEqual(count, basket.Items[sku].Quantity);
        }

        /// <summary>
        /// Test verifies an item can be removed from a basket
        /// </summary>
        [Test]
        public void Can_Remove_An_Item_From_A_Basket()
        {
            //// Arrange
            var sku = _product.Sku;
            var basket = Basket.GetBasket(_customer, _itemCache);
            basket.AddItem(_product);

            Assert.IsTrue(!basket.IsEmpty);

            //// Act
            basket.RemoveItem(sku);

            //// Assert
            Assert.IsTrue(basket.IsEmpty);
        }

        /// <summary>
        /// Test verifies the the sum of the quanities 
        /// </summary>
        [Test]
        public void Sum_Of_Basket_Item_Quanities_Equals_Baskets_TotalQuantityCount()
        {
            //// Arrange
            const int expected = 10;
            var basket = Basket.GetBasket(_customer, _itemCache);
            var product1 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            var product2 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            var product3 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            var product4 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            
            basket.AddItem(product1, product1.Name, 1);
            basket.AddItem(product2, product2.Name, 2);
            basket.AddItem(product3, product3.Name, 3);
            basket.AddItem(product4, product4.Name, 4);

            //// Act
            var sum = basket.TotalQuantityCount;

            //// Assert
            Assert.AreEqual(expected, sum);
        }

        /// <summary>
        /// Test verifies basket total price calculation
        /// </summary>
        [Test]
        public void Sum_Of_Product_Prices_Mulitplied_By_Quantity_Equals_Baskets_TotalBasketPrice()
        {
            //// Arrange
            decimal expectedPrice = 0;
            var basket = Basket.GetBasket(_customer, _itemCache);
            var product1 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            var product2 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            var product3 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());
            var product4 = MockProductDataMaker.MockProductComplete(Guid.NewGuid());

            basket.AddItem(product1, product1.Name, 1);
            expectedPrice += product1.Price;

            basket.AddItem(product2, product2.Name, 2);
            expectedPrice += (product2.Price * 2);

            basket.AddItem(product3, product3.Name, 3);
            expectedPrice += (product3.Price * 3);

            basket.AddItem(product4, product4.Name, 4);
            expectedPrice += (product4.Price * 4);

            //// Act
            var basketTotal = basket.TotalBasketPrice;

            //// Assert
            Assert.AreEqual(expectedPrice, basketTotal);
        }

    }
}