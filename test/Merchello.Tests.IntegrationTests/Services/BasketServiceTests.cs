using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.Data;
using Merchello.Tests.Base.SqlSyntax;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class BasketServiceTests : ServiceIntegrationTestBase
    {
        private IAnonymousCustomer _anonymous;
        private BasketService _basketService;   

        [SetUp]
        public void Initialize()
        {
            _basketService = new BasketService();

            _anonymous = CustomerData.AnonymousCustomerMock();

        }


        [Test]
        public void Can_Delete_All_Consumers_Baskets()
        {
            var baskets = _basketService.GetByConsumer(_anonymous);
            if (baskets.Any())
            {
                Console.Write(baskets.Count().ToString());
                _basketService.Delete(baskets);
            }
            var count = _basketService.GetByConsumer(_anonymous).Count();

            Assert.IsTrue(0 == count);
        }

        [Test]
        public void Can_Save_A_Basket()
        {
            var basket = BasketData.AnonymousBasketForInserting(BasketType.Basket);

            _basketService.Save(basket);

            Console.Write(basket.Id.ToString());
            Assert.IsTrue(basket.Id > 0);

        }


        [Test]
        public void Creating_A_Second_Basket_Results_In_The_First_Being_Returned()
        {
            var basket = BasketData.AnonymousBasketForInserting(BasketType.Wishlist);
            _basketService.Save(basket);

            var id = basket.Id;
            Assert.IsTrue(id > 0);

            var basket2 = _basketService.CreateBasket(_anonymous, BasketType.Wishlist);

            Assert.IsTrue(id == basket2.Id);

        }



       
    }
}
