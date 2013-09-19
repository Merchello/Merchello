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
    public class BasketServiceTests : ServiceTestsBase<IBasket>
    {

        private BasketService _basketService;
        private IAnonymousCustomer _anonymous;

        protected override void Initialize()
        {
            _basketService = new BasketService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            Before = null;
            After = null;

            _anonymous = MockAnonymousCustomerDataMaker.AnonymousCustomerForUpdating();

            BasketService.Saving += delegate(IBasketService sender, SaveEventArgs<IBasket> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            BasketService.Saved += delegate(IBasketService sender, SaveEventArgs<IBasket> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            BasketService.Created += delegate(IBasketService sender, NewEventArgs<IBasket> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            BasketService.Deleting += delegate(IBasketService sender, DeleteEventArgs<IBasket> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            BasketService.Deleted += delegate(IBasketService sender, DeleteEventArgs<IBasket> args)
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
        public void Save_Triggers_Events_And_Basket_Is_Passed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, BasketType.Basket);

            _basketService.Save(basket);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.BasketType, After.BasketType);
        }

        [Test]
        public void Save_Is_Committed()
        {

            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, BasketType.Basket);
            _basketService.Save(basket);


            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Basket_Is_Passed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, BasketType.Basket);

            _basketService.Delete(basket);
            
            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.BasketTypeFieldKey, After.BasketTypeFieldKey);
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, BasketType.Basket);

            _basketService.Delete(basket);

            Assert.IsTrue(CommitCalled);
        }

    }
}
