using System;
using System.Collections.Generic;
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
    public class CustomerItemRegisterServiceTests : ServiceIntegrationTestBase
    {
        private IAnonymousCustomer _anonymous;
        private ICustomerItemRegisterService _customerItemRegisterService;   

        [SetUp]
        public void Initialize()
        {
            _customerItemRegisterService = PreTestDataWorker.CustomerItemRegisterService;
            _anonymous = PreTestDataWorker.MakeExistingAnonymousCustomer();
        }


        [Test]
        public void Can_Delete_All_Consumers_Baskets()
        {
            //// Arrange
            const int expected = 0;

            var baskets = _customerItemRegisterService.GetRegisterByConsumer(_anonymous);
            var enumerable = baskets as ICustomerItemRegister[] ?? baskets.ToArray();

            if (enumerable.Any())
            {
                Console.Write(enumerable.Count().ToString());
                _customerItemRegisterService.Delete(enumerable);
            }
            var count = _customerItemRegisterService.GetRegisterByConsumer(_anonymous).Count();

            Assert.IsTrue(expected == count);
        }

        [Test]
        public void Can_Save_A_Basket()
        {
            var basket = MockBasketDataMaker.ConsumerBasketForInserting(_anonymous, CustomerItemRegisterType.Basket);
            
            _customerItemRegisterService.Save(basket);

            Console.Write(basket.Id.ToString());
            Assert.IsTrue(basket.Id > 0);

        }


        [Test]
        public void Creating_A_Second_Basket_Results_In_The_First_Being_Returned()
        {
            var basket = MockBasketDataMaker.ConsumerBasketForInserting(_anonymous, CustomerItemRegisterType.Wishlist);
            
            _customerItemRegisterService.Save(basket);

            var id = basket.Id;
            Assert.IsTrue(id > 0);

            var basket2 = _customerItemRegisterService.CreateCustomerItemRegister(_anonymous, CustomerItemRegisterType.Wishlist);

            Assert.IsTrue(id == basket2.Id);

        }
        
        /// <summary>
        /// Verifies that a basket can be retrieved for a consumer (anonymous customer or customer)
        /// </summary>
        [Test]
        public void Can_Retrieve_A_Collection_Of_Baskets_For_A_Consumer()
        {
            //// Arrange
            var count = 2;
            var customer = PreTestDataWorker.MakeExistingCustomer();
            var baskets = new List<ICustomerItemRegister>()
            {
                PreTestDataWorker.MakeExistingBasket(customer, CustomerItemRegisterType.Basket),
                PreTestDataWorker.MakeExistingBasket(customer, CustomerItemRegisterType.Wishlist)
            };

            //// Act
            var retreived = _customerItemRegisterService.GetRegisterByConsumer(customer);

            //// Assert
            Assert.NotNull(retreived);
            Assert.AreEqual(count, retreived.Count());
        }

        [Test]
        public void Can_Retrieve_A_BasketByType_For_A_Consumer()
        {
            //// Arrange
            var count = 1;
            var customer = PreTestDataWorker.MakeExistingCustomer();
            var expectedBasketType = CustomerItemRegisterType.Basket;
            var baskets = new List<ICustomerItemRegister>()
            {
                PreTestDataWorker.MakeExistingBasket(customer, CustomerItemRegisterType.Basket),
                PreTestDataWorker.MakeExistingBasket(customer, CustomerItemRegisterType.Wishlist)
            };

            //// Act
            var retreived = _customerItemRegisterService.GetRegisterByConsumer(customer, expectedBasketType);

            //// Assert
            Assert.NotNull(retreived);
            Assert.AreEqual(expectedBasketType, retreived.CustomerItemRegisterType);
        }

       
    }
}
