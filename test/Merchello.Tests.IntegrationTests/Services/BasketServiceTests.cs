using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using NUnit.Framework;

namespace Merchello.Tests.IntegrationTests.Services
{
    [TestFixture]
    [Category("Service Integration")]
    public class BasketServiceTests : ServiceIntegrationTestBase
    {
        private IAnonymousCustomer _anonymous;
        private IBasketService _basketService;   

        [SetUp]
        public void Initialize()
        {
            _basketService = PreTestDataWorker.BasketService;
            _anonymous = PreTestDataWorker.MakeExistingAnonymousCustomer();
        }


        [Test]
        public void Can_Delete_All_Consumers_Baskets()
        {
            //// Arrange
            const int expected = 0;

            var baskets = _basketService.GetByConsumer(_anonymous);
            var enumerable = baskets as IBasket[] ?? baskets.ToArray();

            if (enumerable.Any())
            {
                Console.Write(enumerable.Count().ToString());
                _basketService.Delete(enumerable);
            }
            var count = _basketService.GetByConsumer(_anonymous).Count();

            Assert.IsTrue(expected == count);
        }

        [Test]
        public void Can_Save_A_Basket()
        {
            var basket = MockBasketDataMaker.AnonymousBasketForInserting(_anonymous, BasketType.Basket);

            _basketService.Save(basket);

            Console.Write(basket.Id.ToString());
            Assert.IsTrue(basket.Id > 0);

        }


        [Test]
        public void Creating_A_Second_Basket_Results_In_The_First_Being_Returned()
        {
            var basket = MockBasketDataMaker.AnonymousBasketForInserting(_anonymous, BasketType.Wishlist);
            _basketService.Save(basket);

            var id = basket.Id;
            Assert.IsTrue(id > 0);

            var basket2 = _basketService.CreateBasket(_anonymous, BasketType.Wishlist);

            Assert.IsTrue(id == basket2.Id);

        }



       
    }
}
