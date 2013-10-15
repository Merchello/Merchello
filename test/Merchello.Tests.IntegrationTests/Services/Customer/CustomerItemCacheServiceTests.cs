﻿using System.ComponentModel;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Core.Services;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services.Customer
{
    [TestFixture]
    [NUnit.Framework.Category("Service Integration")]
    public class CustomerItemCacheServiceTests : ServiceIntegrationTestBase
    {
        private IAnonymousCustomer _anonymous;
        private IItemCacheService _itemCacheService;   

        [SetUp]
        public void Initialize()
        {
            PreTestDataWorker.DeleteAllCustomerItemCaches();
            _itemCacheService = PreTestDataWorker.ItemCacheService;
            _anonymous = PreTestDataWorker.MakeExistingAnonymousCustomer();
        }

        /// <summary>
        /// Test verifies that a customer item cache can be created
        /// </summary>
        [Test]
        public void Can_Create_And_Retrieve_A_CustomerItemCache()
        {
            //// Arrange
            const ItemCacheType itemCacheType = ItemCacheType.Basket;

            //// Act
            var itemCache = _itemCacheService.GetItemCacheWithId(_anonymous, itemCacheType);

            //// Assert
            Assert.NotNull(itemCache);
            Assert.IsTrue(itemCache.HasIdentity);
            Assert.IsTrue(itemCache.Items.IsEmpty);
        }

        /// <summary>
        /// Test verifies that calling create on an existing item returns the existing item rather than creating a
        /// new one
        /// </summary>
        [Test]
        public void Calling_Create_Returns_An_Item_If_Exists_Rather_Than_Creating_A_New_One()
        {
            //// Arrange
            const ItemCacheType itemCacheType = ItemCacheType.Basket;
            var existing = _itemCacheService.GetItemCacheWithId(_anonymous, itemCacheType);
            Assert.NotNull(existing);

            //// Act
            var secondAttempt = _itemCacheService.GetItemCacheWithId(_anonymous, itemCacheType);

            //// Assert
            Assert.NotNull(secondAttempt);
            Assert.IsTrue(existing.Key == secondAttempt.Key);
        }

        /// <summary>
        /// Test verifies that two item caches can be created of different types for a customer
        /// </summary>
        [Test]
        public void Can_Create_Two_Seperate_Caches_Of_Different_Types()
        {
            //// Arrange
            
            //// Act
            var basket = _itemCacheService.GetItemCacheWithId(_anonymous, ItemCacheType.Basket);
            var wishlist = _itemCacheService.GetItemCacheWithId(_anonymous, ItemCacheType.Wishlist);

            //// Assert
            Assert.NotNull(basket);
            Assert.NotNull(wishlist);
            Assert.AreNotEqual(basket.Key, wishlist.Key);
        }

        /// <summary>
        /// Test verifies that an item can be added to an item cache
        /// </summary>
        [Test]
        public void Can_Add_An_Item_To_An_ItemCache()
        {
            //// Arrange
            var basket = _itemCacheService.GetItemCacheWithId(_anonymous, ItemCacheType.Basket);

            //// Act
            var lineItem = new ItemCacheLineItem(basket.Id, "Kosher Salt", "KS", 1, 2.5M);            
            basket.Items.Add(lineItem);


            //// Assert
            Assert.IsTrue(basket.Items.Any());
            
        }

        /// <summary>
        /// Test verifies that than item can be added to an item cache and persisted
        /// </summary>
        [Test]
        public void Can_Add_And_Save_An_Item_To_ItemCache()
        {
            //// Arrange
            var basket = _itemCacheService.GetItemCacheWithId(_anonymous, ItemCacheType.Basket);
            var lineItem = new ItemCacheLineItem(basket.Id, "Kosher Salt", "KS", 1, 2.5M);
            basket.Items.Add(lineItem);
            
            //// Act
            _itemCacheService.Save(basket);            

            //// Assert            
            Assert.IsFalse(basket.IsDirty());
            Assert.IsTrue(basket.HasIdentity);
            Assert.IsFalse(basket.Items.IsEmpty);
        }

        /// <summary>
        /// Test confirms that two baskets can be managed independently
        /// </summary>
        [Test]
        public void Can_Manage_Two_Baskets_Independently()
        {
            //// Arrange
            var customer1 = PreTestDataWorker.MakeExistingAnonymousCustomer();
            var customer2 = PreTestDataWorker.MakeExistingAnonymousCustomer();
            var basket1 = _itemCacheService.GetItemCacheWithId(customer1, ItemCacheType.Basket);
            var basket2 = _itemCacheService.GetItemCacheWithId(customer2, ItemCacheType.Basket);

            //// Act            
            basket1.Items.Add(new ItemCacheLineItem(basket1.Id, "Kosher Salt", "KS", 1, 2.5M));
            _itemCacheService.Save(basket1);
            
            basket2.Items.Add(new ItemCacheLineItem(basket2.Id, "Kosher Salt", "KS", 1, 2.5M));
            basket2.Items.Add(new ItemCacheLineItem(basket2.Id, "Pickle dust", "PD", 20, 25.50M));
            _itemCacheService.Save(basket2);

            //// Assert
            Assert.IsTrue(basket1.HasIdentity);
            Assert.IsTrue(basket2.HasIdentity);
            Assert.AreNotEqual(basket1.Id, basket2.Id);
            Assert.AreEqual(1, basket1.Items.Count);
            Assert.AreEqual(2, basket2.Items.Count);
        }

        /// <summary>
        /// 
        /// </summary>
        [Test]
        public void Can_Update_An_Item_In_An_ItemCache()
        {
            //// Arrange
            var basket1 = _itemCacheService.GetItemCacheWithId(_anonymous, ItemCacheType.Basket);
            basket1.Items.Add(new ItemCacheLineItem(basket1.Id, "Kosher Salt", "KS", 1, 2.5M));
            _itemCacheService.Save(basket1);

            //// Act
            basket1.Items["KS"].Amount = 35M;
            _itemCacheService.Save(basket1);

            //// Assert
            Assert.IsFalse(basket1.IsDirty());
        }
    }
}
