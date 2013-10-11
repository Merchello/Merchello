using System.ComponentModel;
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
        private ICustomerItemCacheService _customerItemCacheService;   

        [SetUp]
        public void Initialize()
        {
            PreTestDataWorker.DeleteAllCustomerItemCaches();
            _customerItemCacheService = PreTestDataWorker.CustomerItemCacheService;
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
            var itemCache = _customerItemCacheService.GetCustomerItemCacheWithKey(_anonymous, itemCacheType);

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
            var existing = _customerItemCacheService.GetCustomerItemCacheWithKey(_anonymous, itemCacheType);
            Assert.NotNull(existing);

            //// Act
            var secondAttempt = _customerItemCacheService.GetCustomerItemCacheWithKey(_anonymous, itemCacheType);

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
            var basket = _customerItemCacheService.GetCustomerItemCacheWithKey(_anonymous, ItemCacheType.Basket);
            var wishlist = _customerItemCacheService.GetCustomerItemCacheWithKey(_anonymous, ItemCacheType.Wishlist);

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
            var basket = _customerItemCacheService.GetCustomerItemCacheWithKey(_anonymous, ItemCacheType.Basket);

            //// Act
            var lineItem = new OrderLineItem(basket.Id, "Kosher Salt", "KS", 1, 2.5M);            
            basket.Items.Add(lineItem);


            //// Assert
            Assert.IsTrue(basket.Items.Any());
            
        }
    }
}
