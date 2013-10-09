using System;
using System.Linq;
using Merchello.Core;
using Merchello.Core.Models;
using Merchello.Core.Persistence;
using Merchello.Core.Services;
using Merchello.Tests.Base.DataMakers;
using Merchello.Tests.Base.Respositories;
using Merchello.Tests.Base.Respositories.UnitOfWork;
using Merchello.Tests.Base.Services;
using NUnit.Framework;
using Umbraco.Core.Events;

namespace Merchello.Tests.UnitTests.Services
{
    [TestFixture]
    [Category("Services")]
    public class CustomerRegistryServiceTests : ServiceTestsBase<ICustomerItemCache>
    {

        private CustomerItemCacheService _customerItemCacheService;
        private IAnonymousCustomer _anonymous;

        protected override void Initialize()
        {
            _customerItemCacheService = new CustomerItemCacheService(new MockUnitOfWorkProvider(), new RepositoryFactory());
            Before = null;
            After = null;

            _anonymous = MockAnonymousCustomerDataMaker
                         .AnonymousCustomerForInserting()
                         .MockSavedWithKey(Guid.NewGuid());

            CustomerItemCacheService.Saving += delegate(ICustomerItemCacheService sender, SaveEventArgs<ICustomerItemCache> args)
            {
                BeforeTriggered = true;
                Before = args.SavedEntities.FirstOrDefault();
            };

            CustomerItemCacheService.Saved += delegate(ICustomerItemCacheService sender, SaveEventArgs<ICustomerItemCache> args)
            {
                AfterTriggered = true;
                After = args.SavedEntities.FirstOrDefault();
            };


            CustomerItemCacheService.Created += delegate(ICustomerItemCacheService sender, Core.Events.NewEventArgs<ICustomerItemCache> args)
            {
                AfterTriggered = true;
                After = args.Entity;
            };

            CustomerItemCacheService.Deleting += delegate(ICustomerItemCacheService sender, DeleteEventArgs<ICustomerItemCache> args)
            {
                BeforeTriggered = true;
                Before = args.DeletedEntities.FirstOrDefault();
            };

            CustomerItemCacheService.Deleted += delegate(ICustomerItemCacheService sender, DeleteEventArgs<ICustomerItemCache> args)
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
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemCacheType.Basket);

            _customerItemCacheService.Save(basket);

            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.CustomerItemCacheType, After.CustomerItemCacheType);
        }

        [Test]
        public void Save_Is_Committed()
        {

            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemCacheType.Basket);
            _customerItemCacheService.Save(basket);


            Assert.IsTrue(CommitCalled);

        }

        [Test]
        public void Delete_Triggers_Events_And_Basket_Is_Passed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemCacheType.Basket);

            _customerItemCacheService.Delete(basket);
            
            Assert.IsTrue(BeforeTriggered);
            Assert.AreEqual(basket.ConsumerKey, Before.ConsumerKey);

            Assert.IsTrue(AfterTriggered);
            Assert.AreEqual(basket.ItemCacheTfKey, After.ItemCacheTfKey);
        }

        [Test]
        public void Delete_Is_Committed()
        {
            var basket = MockBasketDataMaker.AnonymousBasket(_anonymous, CustomerItemCacheType.Basket);

            _customerItemCacheService.Delete(basket);

            Assert.IsTrue(CommitCalled);
        }

    }
}
