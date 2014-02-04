using Merchello.Core.Models;
using Merchello.Core.Models.TypeFields;
using Merchello.Tests.Base.TypeFields;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.TypeFields
{
    [TestFixture]
    [Category("TypeField")]
    public class BasketTypeFieldTests
    {
        private ITypeField _mockBasket;
        private ITypeField _mockWishlist;

        [SetUp]
        public void Setup()
        {
            _mockBasket = TypeFieldMock.ItemCacheBasket;
            _mockWishlist = TypeFieldMock.ItemCacheWishlist;
        }


        /// <summary>
        /// Asserts the BasketType class returns the expected basket configuration
        /// </summary>
        [Test]
        public void BasketType_basket_matches_configuration()
        {
            var type = EnumTypeFieldConverter.ItemItemCache.Basket;

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
            var type = EnumTypeFieldConverter.ItemItemCache.Wishlist;

            Assert.AreEqual(_mockWishlist.Alias, type.Alias);
            Assert.AreEqual(_mockWishlist.Name, type.Name);
            Assert.AreEqual(_mockWishlist.TypeKey, type.TypeKey);

        }
    }
}
