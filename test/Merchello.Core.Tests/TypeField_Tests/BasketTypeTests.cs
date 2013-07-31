using System;
using System.Configuration;
using Merchello.Core.Configuration.Outline;
using Merchello.Core.Models;
using NUnit.Framework;

namespace Merchello.Core.Tests.TypeField_Tests
{
    [TestFixture]
    public class BasketTypeTests
    {
        private ITypeField _mockBasket;
        private ITypeField _mockWishlist;

        [SetUp]
        public void Setup()
        {
            _mockBasket = new TypeField("Basket", "Standard Basket", new Guid("C53E3100-2DFD-408A-872E-4380383FAD35"));
            _mockWishlist = new TypeField("Wishlist", "Wishlist", new Guid("B3EBB9E0-C7CE-4BA6-B379-BEDA3465D6D5"));
        }


        /// <summary>
        /// Verifies baskets should have two configuration options
        /// </summary>
        [Test]
        public void BasketType_should_have_2_options()
        {
            var fields =
                ((MerchelloSection)ConfigurationManager.GetSection("merchello")).TypeFields.Basket;

            Assert.AreEqual(2, fields.Count);
        }

        /// <summary>
        /// Asserts the BasketType class returns the expected basket configuration
        /// </summary>
        [Test]
        public void BasketType_basket_matches_configuration()
        {
            var type = BasketType.Basket;

            Assert.AreEqual(_mockBasket.Alias, type.Alias);
            Assert.AreEqual(_mockBasket.Name, type.Name);
            Assert.AreEqual(_mockBasket.TypeKey, type.TypeKey);

        }


        /// <summary>
        /// Asserts the BasketType class returns the expected wishlist configuration
        /// </summary>
        [Test]
        public void BasketType_wishlist_matches_configuration()
        {
            var type = BasketType.Wishlist;

            Assert.AreEqual(_mockWishlist.Alias, type.Alias);
            Assert.AreEqual(_mockWishlist.Name, type.Name);
            Assert.AreEqual(_mockWishlist.TypeKey, type.TypeKey);

        }
    }
}
