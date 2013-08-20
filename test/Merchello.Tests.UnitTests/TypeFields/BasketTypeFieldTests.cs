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
            _mockBasket = TypeFieldMock.BasketBasket;
            _mockWishlist = TypeFieldMock.BasketWishlist;
        }


        /// <summary>
        /// Asserts the BasketType class returns the expected basket configuration
        /// </summary>
        [Test]
        public void BasketType_basket_matches_configuration()
        {
            var type = TypeFieldProvider.Basket().Basket;

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
            var type = TypeFieldProvider.Basket().Wishlist;

            Assert.AreEqual(_mockWishlist.Alias, type.Alias);
            Assert.AreEqual(_mockWishlist.Name, type.Name);
            Assert.AreEqual(_mockWishlist.TypeKey, type.TypeKey);

        }
    }
}
