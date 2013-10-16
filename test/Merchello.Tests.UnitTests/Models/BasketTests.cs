using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Tests.Base.DataMakers;
using Merchello.Web.Models;
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
            _itemCache = new ItemCache(_customer.Key, ItemCacheType.Basket) {Id = 111};

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
    }
}