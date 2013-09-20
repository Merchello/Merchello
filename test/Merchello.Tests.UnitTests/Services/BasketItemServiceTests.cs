using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Events;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.Services;
using NUnit.Framework;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class BasketItemServiceTests : ServiceTestsBase<IBasketItem>
    {
        private BasketService _basketService;
        private BasketItemService _basketItemService;
        private IAnonymousCustomer _anonymous;

        protected override void Initialize()
        {
            _basketService = new BasketService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            _basketItemService = new BasketItemService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            Before = null;
            After = null;

            _anonymous = MockAnonymousCustomerDataMaker.AnonymousCustomerForInserting().MockSavedWithKey(Guid.NewGuid());

            BasketItemService.Saving += delegate(IBasketItemService sender, SaveEventArgs<IBasketItem> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            BasketItemService.Saved += delegate(IBasketItemService sender, SaveEventArgs<IBasketItem> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            BasketItemService.Created += delegate(IBasketItemService sender, NewEventArgs<IBasketItem> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            BasketItemService.Deleting += delegate(IBasketItemService sender, DeleteEventArgs<IBasketItem> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            BasketItemService.Deleted += delegate(IBasketItemService sender, DeleteEventArgs<IBasketItem> args)
            {
                AfterTriggered = true;
                After = args.DeletedEntities.FirstOrDefault();
            };

            // General tests
            MockDatabaseUnitOfWork.Committed += delegate(object sender)
            {
                CommitCalled = true;
            };


        }

        [Test]
        public void Save_Triggers_Events_And_BasketItem_Is_Passed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, BasketType.Basket);
            _basketService.Save(basket);

            var basketItem = _basketItemService.CreateBasketItem(basket, "sku", "demo", 1, 1, 10.00m);
            _basketItemService.Save(basketItem);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basketItem.Sku, Before.Sku);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basketItem.Id, After.Id);
        }

        [Test]
        public void Save_Is_Committed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, BasketType.Basket);
            _basketService.Save(basket);

            CommitCalled = false;

            var basketItem = _basketItemService.CreateBasketItem(basket, "sku", "demo", 1, 1, 10.00m);
            _basketItemService.Save(basketItem);

            Assert.IsTrue(CommitCalled);

        }

        //[Test]
        //public void Delete_Triggers_Events_And_Basket_Is_Passed()
        //{
        //    var basket = MockBasketDataMaker.AnonymousBasket(BasketType.Basket);

        //    _basketService.Delete(basket);
            
        //    Assert.IsTrue(BeforeTriggered);
        //    Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

        //    Assert.IsTrue(AfterTriggered);
        //    Assert.AreEqual(basket.BasketTypeFieldKey, After.BasketTypeFieldKey);
        //}

        [Test]
        public void Delete_Is_Committed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, BasketType.Basket);

            _basketService.Delete(basket);

            Assert.IsTrue(CommitCalled);
        }

    }
}
